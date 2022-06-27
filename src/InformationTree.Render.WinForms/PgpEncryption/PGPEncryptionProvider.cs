using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using NLog;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;
using System.Text;

namespace InformationTree.PgpEncryption
{
    public class PGPEncryptionProvider : IPGPEncryptionProvider
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IPopUpService _popUpService;

        #region Constants

        protected const int BufferSize = 0x10000; // should always be power of 2

        #endregion Constants

        public PGPEncryptionProvider(IPopUpService popUpService)
        {
            _popUpService = popUpService;
        }
        
        #region Decrypt

        public string GetDecryptedStringFromString(string encryptedText, string privateKey, string pgpPassword)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                return string.Empty;
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                _logger.Error("Private key is empty. Cannot decrypt.");
                _popUpService.ShowError("Private key is empty. Keeping text encrypted.");
                return encryptedText;
            }
            
            try
            {
                using (var privateKeyStream = privateKey.ToStream())
                {
                    return GetDecryptedStringFromStream(encryptedText, privateKeyStream, pgpPassword);
                }
            }
            catch (Exception ex)
            {
                var errorNotDecryptedMessage = "The message was not decrypted.";
                _popUpService.ShowWarning(ex.Message, errorNotDecryptedMessage);
                
                _logger.Error(ex, errorNotDecryptedMessage);
                return encryptedText;
            }
        }

        public string GetDecryptedStringFromFile(string encryptedText, string privateKeyFile, string pgpPassword)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                return string.Empty;
            if (string.IsNullOrWhiteSpace(privateKeyFile))
            {
                _logger.Error("Private key file is empty. Cannot decrypt.");
                _popUpService.ShowError("Private key file is empty. Keeping text encrypted.");
                return encryptedText;
            }
            
            try
            {
                using (Stream privateKeyStream = File.OpenRead(privateKeyFile))
                {
                    return GetDecryptedStringFromStream(encryptedText, privateKeyStream, pgpPassword);
                }
            }
            catch (Exception ex)
            {
                var errorNotDecryptedMessage = "The message was not decrypted.";
                _logger.Error(ex, errorNotDecryptedMessage);
                _popUpService.ShowWarning(ex.Message, errorNotDecryptedMessage);
                return encryptedText;
            }
        }

        private string GetDecryptedStringFromStream(string encryptedText, Stream privateKeyStream, string pgpPassword)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                return string.Empty;
            
            try
            {
                using (var inputStream = encryptedText.ToStream())
                {
                    using (var outputStream = new MemoryStream())
                    {
                        Decrypt(inputStream,
                            privateKeyStream,
                            pgpPassword,
                            outputStream);

                        outputStream.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(outputStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorNotDecryptedMessage = "The message was not decrypted.";
                _logger.Error(ex, errorNotDecryptedMessage);
                _popUpService.ShowWarning(ex.Message, errorNotDecryptedMessage);
                
                return encryptedText;
            }
        }

        public void Decrypt(string inputfile, string privateKeyFile, string passPhrase, string outputFile)
        {
            if (!File.Exists(inputfile))
                throw new FileNotFoundException($"Encrypted File [{inputfile}] not found.");

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException($"Private Key File [{privateKeyFile}] not found.");

            if (string.IsNullOrEmpty(outputFile))
                throw new ArgumentNullException("Invalid Output file path.");

            using (Stream inputStream = File.OpenRead(inputfile))
            using (Stream keyIn = File.OpenRead(privateKeyFile))
            using (Stream outputStream = File.Create(outputFile))
            {
                Decrypt(inputStream, keyIn, passPhrase, outputStream);
            }
        }

        private void Decrypt(Stream inputStream, Stream privateKeyStream, string passPhrase, Stream outputStream)
        {
            try
            {
                PgpObjectFactory pgpF = null;
                PgpEncryptedDataList enc = null;
                PgpObject o = null;
                PgpPrivateKey sKey = null;
                PgpPublicKeyEncryptedData pbe = null;
                PgpSecretKeyRingBundle pgpSec = null;

                pgpF = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputStream));
                // find secret key
                pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));

                if (pgpF != null)
                    o = pgpF.NextPgpObject();

                // the first object might be a PGP marker packet.
                if (o is PgpEncryptedDataList)
                    enc = (PgpEncryptedDataList)o;
                else
                    enc = (PgpEncryptedDataList)pgpF.NextPgpObject();

                if (enc == null)
                {
                    var noEncryptedDataMessage = "Encrypted data not found.";
                    _logger.Warn($"{nameof(PgpEncryptedDataList)} is null. {noEncryptedDataMessage}");
                    _popUpService.ShowWarning(noEncryptedDataMessage);
                    return;
                }

                // decrypt
                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    sKey = FindSecretKey(pgpSec, pked.KeyId, passPhrase.ToCharArray());

                    if (sKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }

                if (sKey == null)
                    throw new ArgumentException("Secret key for message not found.");

                PgpObjectFactory plainFact = null;

                using (var clear = pbe.GetDataStream(sKey))
                {
                    plainFact = new PgpObjectFactory(clear);
                }
                
                var message = plainFact.NextPgpObject();

                if (message is PgpCompressedData)
                {
                    var cData = (PgpCompressedData)message;
                    PgpObjectFactory of = null;

                    using (Stream compDataIn = cData.GetDataStream())
                    {
                        of = new PgpObjectFactory(compDataIn);
                    }

                    message = of.NextPgpObject();
                    if (message is PgpOnePassSignatureList)
                    {
                        message = of.NextPgpObject();
                        PgpLiteralData Ld = null;
                        Ld = (PgpLiteralData)message;
                        Stream unc = Ld.GetInputStream();
                        Streams.PipeAll(unc, outputStream);
                    }
                    else
                    {
                        PgpLiteralData Ld = null;
                        Ld = (PgpLiteralData)message;
                        Stream unc = Ld.GetInputStream();
                        Streams.PipeAll(unc, outputStream);
                    }
                }
                else if (message is PgpLiteralData ld)
                {
                    var outFileName = ld.FileName;

                    var unc = ld.GetInputStream();
                    Streams.PipeAll(unc, outputStream);
                }
                else if (message is PgpOnePassSignatureList)
                    throw new PgpException("Encrypted message contains a signed message - not literal data.");
                else
                    throw new PgpException("Message is not a simple encrypted file - type unknown.");

                if (pbe.IsIntegrityProtected())
                {
                    if (!pbe.Verify())
                    {
                        var integrityCheckFailedMessage = "Message failed integrity check.";
                        _popUpService.ShowWarning(integrityCheckFailedMessage);
                        _logger.Warn($"{nameof(pbe)}.{nameof(pbe.Verify)}() failed. {integrityCheckFailedMessage}");
                    }
                    else
                    {
                        var messageIntegrityCheckPassed = "Message integrity check passed.";
                        _logger.Info($"{nameof(pbe)}.{nameof(pbe.Verify)}() succeeded. {messageIntegrityCheckPassed}");
                        _popUpService.ShowInfo(messageIntegrityCheckPassed);
                    }
                }
                else
                {
                    var noMessageIntegrityCheck = "No message integrity check.";
                    _logger.Warn($"{nameof(pbe)}.{nameof(pbe.IsIntegrityProtected)}() returned false. {noMessageIntegrityCheck}");
                    _popUpService.ShowInfo(noMessageIntegrityCheck);
                }
            }
            catch (PgpException ex)
            {
                _popUpService.ShowError(ex.Message);
                _logger.Error(ex);
            }
        }

        #endregion Decrypt

        #region Test for existing key

        public bool IsPgpSecretKey(string privateKeyFile)
        {
            if (string.IsNullOrWhiteSpace(privateKeyFile))
                return false;
            
            var privateKeyStream = File.OpenRead(privateKeyFile);

            if (privateKeyStream == null)
                return false;

            try
            {
                var pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));
            }
            catch (PgpException ex)
            {
                _logger.Error(ex);
                if (ex.Message == "Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKeyRing found where PgpSecretKeyRing expected")
                {
                    _popUpService.ShowWarning("Public key selected instead of a private key. Please select a private key.");
                    return false;
                }
                else
                {
                    _popUpService.ShowError(ex.Message);
                    return false;
                }
            }

            return true;
        }

        private bool ExistsPassword(PgpSecretKeyRingBundle pgpSec, char[] password)
        {
            foreach (PgpSecretKeyRing keyRing in pgpSec.GetKeyRings())
            {
                if (keyRing != null)
                    foreach (PgpSecretKey key in keyRing.GetSecretKeys())
                    {
                        if (key.ExtractPrivateKey(password) != null)
                            return true;
                    }
            }

            return false;
        }

        public bool ExistsPasswordFromString(string privateKeyString, char[] password)
        {
            if (string.IsNullOrEmpty(privateKeyString))
            {
                _popUpService.ShowError("Private key is missing. No private key selected.");
                return false;
            }
            
            using (var privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKeyString)))
            {
                return ExistsPasswordFromStream(privateKeyStream, password);
            }
        }

        private bool ExistsPasswordFromStream(Stream privateKeyStream, char[] password)
        {
            if (privateKeyStream == null)
                return false;
            try
            {
                var pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));
                return ExistsPassword(pgpSec, password);
            }
            catch (PgpException ex)
            {
                _logger.Error(ex);
                if (ex.Message == "Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKeyRing found where PgpSecretKeyRing expected")
                {
                    _popUpService.ShowWarning("Public key selected instead of a private key. Please select a private key.");
                }
                else if (!ex.Message.StartsWith("Checksum mismatch at "))
                {
                    _popUpService.ShowError(ex.Message);
                }
            }
            return false;
        }

        public bool ExistsPassword(string privateKeyFile, char[] password)
        {
            if (string.IsNullOrEmpty(privateKeyFile))
            {
                _popUpService.ShowError("Private key file is missing. No private key selected.");
                return false;
            }

            return ExistsPasswordFromStream(File.OpenRead(privateKeyFile), password);
        }

        #endregion Test for existing key

        #region Encrypt

        public string GetEncryptedStringFromString(string decryptedText, string publicKeyText, bool armor, bool withIntegrityCheck)
        {
            if (publicKeyText.IsEmpty())
                return decryptedText;
            
            try
            {
                using (var publicKeyStream = publicKeyText.ToStream())
                {
                    return GetEncryptedStringFromStream(decryptedText, publicKeyStream, armor, withIntegrityCheck);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _popUpService.ShowWarning(ex.Message, "Message not decrypted because of error");
                return decryptedText;
            }
        }

        private string GetEncryptedStringFromStream(string inputText, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
        {
            if (inputText.IsEmpty())
                return string.Empty;

            try
            {
                using (var inputStream = inputText.ToStream())
                {
                    using (var outputStream = new MemoryStream())
                    {
                        EncryptFromStream(
                            inputStream,
                            outputStream,
                            publicKeyStream,
                            armor,
                            withIntegrityCheck);

                        outputStream.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(outputStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _popUpService.ShowWarning(ex.Message, "Message not decrypted because of error");
                throw;
            }
        }

        private void WriteStringToLiteralData(Stream outp, char fileType, string name, string buffer)
        {
            var lData = new PgpLiteralDataGenerator();
            var pOut = lData.Open(outp, fileType, name, buffer.Length, DateTime.Now);
            pOut.Write(Encoding.UTF8.GetBytes(buffer), 0, buffer.Length);
        }

        public void EncryptFromFile(Stream inputStream, Stream outputStream, string publicKeyFile, bool armor, bool withIntegrityCheck)
        {
            if (string.IsNullOrWhiteSpace(publicKeyFile))
            {
                _popUpService.ShowError("Public key file is missing. Cannot encrypt.");
                return;
            }
            
            try
            {
                using (var publicKeyStreamReader = new StreamReader(publicKeyFile))
                {
                    EncryptFromStream(inputStream, outputStream, publicKeyStreamReader.BaseStream, armor, withIntegrityCheck);
                }
            }
            catch (PgpException ex)
            {
                _logger.Error(ex);
                _popUpService.ShowWarning(ex.Message, "Message not encrypted because of error");
            }
        }

        private void EncryptFromStream(Stream inputStream, Stream outputStream, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
        {
            try
            {
                var encKey = ReadPublicKey(publicKeyStream);

                using (var bOut = new MemoryStream())
                {
                    var comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

                    using (var reader = new StreamReader(inputStream))
                    {
                        WriteStringToLiteralData(comData.Open(bOut), PgpLiteralData.Binary, string.Empty, reader.ReadToEnd());
                    }
                    comData.Close();
                    var cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());

                    cPk.AddMethod(encKey);
                    byte[] bytes = bOut.ToArray();

                    if (armor)
                    {
                        using (var armoredStream = new ArmoredOutputStream(outputStream))
                        {
                            using (Stream cOut = cPk.Open(armoredStream, bytes.Length))
                            {
                                cOut.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    else
                    {
                        using (Stream cOut = cPk.Open(outputStream, bytes.Length))
                        {
                            cOut.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
            catch (PgpException ex)
            {
                _logger.Error(ex);
                _popUpService.ShowWarning(ex.Message, "Message not encrypted because of error");
            }
        }

        #endregion Encrypt

        #region Private helpers

        /*
        * A simple routine that opens a key ring file and loads the first available key suitable for encryption.
        */

        private PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);

            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            // iterate through the key rings.
            foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey k in kRing.GetPublicKeys())
                {
                    if (k.IsEncryptionKey)
                        return k;
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        /*
        * Search a secret key ring collection for a secret key corresponding to keyId if it exists.
        */

        private PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] password)
        {
            PgpSecretKey keyRing = pgpSec.GetSecretKey(keyId);

            if (keyRing == null)
                return null;

            return keyRing.ExtractPrivateKey(password);
        }

        #endregion Private helpers
    }
}
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
    [Obsolete("Break into many classes")]
    public class PGPEncryptionProvider : IPGPEncryptionProvider
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPopUpService _popUpService;

        #region Constants

        private const int BufferSize = 0x10000; // should always be power of 2

        #endregion Constants

        public PGPEncryptionProvider(IPopUpService popUpService)
        {
            _popUpService = popUpService;
        }
        
        #region Decrypt

        public string GetDecryptedStringFromString(string encryptedText, string privateKey, string pgpPassword)
        {
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
                throw new FileNotFoundException(String.Format("Encrypted File [{0}] not found.", inputfile));

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException(String.Format("Private Key File [{0}] not found.", privateKeyFile));

            if (string.IsNullOrEmpty(outputFile))
                throw new ArgumentNullException("Invalid Output file path.");

            using (Stream inputStream = File.OpenRead(inputfile))
            {
                using (Stream keyIn = File.OpenRead(privateKeyFile))
                {
                    using (Stream outputStream = File.Create(outputFile))
                    {
                        Decrypt(inputStream, keyIn, passPhrase, outputStream);
                    }
                }
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
            using (var privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKeyString)))
            {
                return ExistsPasswordFromStream(privateKeyStream, password);
            }
        }

        public bool ExistsPasswordFromStream(Stream privateKeyStream, char[] password)
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

        public string GetEncryptedStringFromStream(string inputFile, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
        {
            try
            {
                using (var inputStream = inputFile.ToStream())
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
                return File.ReadAllText(inputFile);
            }
        }

        public void WriteStringToLiteralData(Stream outp, char fileType, String name, String buffer)
        {
            var lData = new PgpLiteralDataGenerator();
            var pOut = lData.Open(outp, fileType, name, buffer.Length, DateTime.Now);
            pOut.Write(Encoding.UTF8.GetBytes(buffer), 0, buffer.Length);
        }

        public void EncryptFromFile(Stream inputStream, Stream outputStream, string publicKeyFile, bool armor, bool withIntegrityCheck)
        {
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
                _popUpService.ShowWarning(ex.Message, "Message not decrypted because of error");
            }
        }

        public void EncryptFromStream(Stream inputStream, Stream outputStream, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
        {
            try
            {
                var encKey = ReadPublicKey(publicKeyStream);

                using (MemoryStream bOut = new MemoryStream())
                {
                    var comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

                    using (var reader = new StreamReader(inputStream))
                    {
                        WriteStringToLiteralData(comData.Open(bOut), PgpLiteralData.Binary, string.Empty, reader.ReadToEnd());
                    }
                    comData.Close();
                    PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());

                    cPk.AddMethod(encKey);
                    byte[] bytes = bOut.ToArray();

                    if (armor)
                    {
                        using (ArmoredOutputStream armoredStream = new ArmoredOutputStream(outputStream))
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
                _popUpService.ShowWarning(ex.Message, "Message not decrypted because of error");
            }
        }

        #endregion Encrypt

        #region Encrypt and Sign

        public void EncryptAndSign(string inputFile, string outputFile, string publicKeyFile, string privateKeyFile, string passPhrase, bool armor)
        {
            PGPEncryptionKeys encryptionKeys = new PGPEncryptionKeys(publicKeyFile, privateKeyFile, passPhrase);

            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Input file [{inputFile}] does not exist.");

            if (!File.Exists(publicKeyFile))
                throw new FileNotFoundException($"Public Key file [{publicKeyFile}] does not exist.");

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException($"Private Key file [{privateKeyFile}] does not exist.");

            if (String.IsNullOrEmpty(passPhrase))
                throw new ArgumentNullException("Invalid Pass Phrase.");

            if (encryptionKeys == null)
                throw new ArgumentNullException("Encryption Key not found.");

            using (Stream outputStream = File.Create(outputFile))
            {
                if (armor)
                    using (ArmoredOutputStream armoredOutputStream = new ArmoredOutputStream(outputStream))
                    {
                        OutputEncrypted(inputFile, armoredOutputStream, encryptionKeys);
                    }
                else
                    OutputEncrypted(inputFile, outputStream, encryptionKeys);
            }
        }

        private void OutputEncrypted(string inputFile, Stream outputStream, PGPEncryptionKeys encryptionKeys)
        {
            using (Stream encryptedOut = ChainEncryptedOut(outputStream, encryptionKeys))
            {
                FileInfo unencryptedFileInfo = new FileInfo(inputFile);
                using (Stream compressedOut = ChainCompressedOut(encryptedOut))
                {
                    PgpSignatureGenerator signatureGenerator = InitSignatureGenerator(compressedOut, encryptionKeys);
                    using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedFileInfo))
                    {
                        using (FileStream inputFileStream = unencryptedFileInfo.OpenRead())
                        {
                            WriteOutputAndSign(compressedOut, literalOut, inputFileStream, signatureGenerator);
                            inputFileStream.Close();
                        }
                    }
                }
            }
        }

        private void WriteOutputAndSign(Stream compressedOut, Stream literalOut, FileStream inputFile, PgpSignatureGenerator signatureGenerator)
        {
            int length = 0;
            byte[] buf = new byte[BufferSize];
            while ((length = inputFile.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, length);
                signatureGenerator.Update(buf, 0, length);
            }
            signatureGenerator.Generate().Encode(compressedOut);
        }

        private Stream ChainEncryptedOut(Stream outputStream, PGPEncryptionKeys m_encryptionKeys)
        {
            PgpEncryptedDataGenerator encryptedDataGenerator;
            encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.TripleDes, new SecureRandom());
            encryptedDataGenerator.AddMethod(m_encryptionKeys.PublicKey);
            return encryptedDataGenerator.Open(outputStream, new byte[BufferSize]);
        }

        private Stream ChainCompressedOut(Stream encryptedOut)
        {
            PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            return compressedDataGenerator.Open(encryptedOut);
        }

        private Stream ChainLiteralOut(Stream compressedOut, FileInfo file)
        {
            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, file);
        }

        private PgpSignatureGenerator InitSignatureGenerator(Stream compressedOut, PGPEncryptionKeys m_encryptionKeys)
        {
            const bool IsCritical = false;
            const bool IsNested = false;
            PublicKeyAlgorithmTag tag = m_encryptionKeys.SecretKey.PublicKey.Algorithm;
            PgpSignatureGenerator pgpSignatureGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
            pgpSignatureGenerator.InitSign(PgpSignature.BinaryDocument, m_encryptionKeys.PrivateKey);
            foreach (string userId in m_encryptionKeys.SecretKey.PublicKey.GetUserIds())
            {
                PgpSignatureSubpacketGenerator subPacketGenerator = new PgpSignatureSubpacketGenerator();
                subPacketGenerator.SetSignerUserId(IsCritical, userId);
                pgpSignatureGenerator.SetHashedSubpackets(subPacketGenerator.Generate());
                // Just the first one!
                break;
            }
            pgpSignatureGenerator.GenerateOnePassVersion(IsNested).Encode(compressedOut);
            return pgpSignatureGenerator;
        }

        #endregion Encrypt and Sign

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
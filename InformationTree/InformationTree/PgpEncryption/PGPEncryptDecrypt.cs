using System;
using System.IO;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System.Windows.Forms;
using System.Text;

namespace InformationTree.PgpEncryption
{
    public static class PGPEncryptDecrypt
    {
        #region Constants
        private const int BufferSize = 0x10000; // should always be power of 2
        #endregion Constants

        #region GenerateStreamFromString
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        #endregion GenerateStreamFromString

        #region Keys from file dialog
        public static string GetKeyFile(string fileType)
        {
            var lowerFileType = fileType.ToLower();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open " + lowerFileType + " key file";
            dlg.Filter = fileType + " Key Files|*.asc;*.skr;*." + lowerFileType + "|All files (*.*)|*.*";
            dlg.InitialDirectory = Application.StartupPath;
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;
            return null;
        }

        public static string GetPrivateKeyFile()
        {
            return GetKeyFile("Private");
        }

        public static string GetPublicKeyFile()
        {
            return GetKeyFile("Public");
        }

        #endregion Keys from file dialog

        #region Decrypt

        public static string GetDecryptedStringFromString(string encryptedText, string privateKey, string pgpPassword)
        {
            try
            {
                using (Stream privateKeyStream = GenerateStreamFromString(privateKey))
                {
                    return GetDecryptedStringFromStream(encryptedText, privateKeyStream, pgpPassword);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message not decrypted because of error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return encryptedText;
            }
        }

        public static string GetDecryptedStringFromFile(string encryptedText, string privateKeyFile, string pgpPassword)
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
                MessageBox.Show(ex.Message, "Message not decrypted because of error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return encryptedText;
            }
        }

        public static string GetDecryptedStringFromStream(string encryptedText, Stream privateKeyStream, string pgpPassword)
        {
            try
            {
                using (var inputStream = GenerateStreamFromString(encryptedText))
                {
                    using (var outputStream = new MemoryStream())
                    {

                        PGPEncryptDecrypt.Decrypt(inputStream,
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
                MessageBox.Show(ex.Message, "Message not decrypted because of error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return encryptedText;
            }
        }

        public static void Decrypt(string inputfile, string privateKeyFile, string passPhrase, string outputFile)
        {
            if (!File.Exists(inputfile))
                throw new FileNotFoundException(String.Format("Encrypted File [{0}] not found.", inputfile));

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException(String.Format("Private Key File [{0}] not found.", privateKeyFile));

            if (String.IsNullOrEmpty(outputFile))
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

        public static void Decrypt(Stream inputStream, Stream privateKeyStream, string passPhrase, Stream outputStream)
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
                    MessageBox.Show("No encrypted message", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                using (Stream clear = pbe.GetDataStream(sKey))
                {
                    plainFact = new PgpObjectFactory(clear);
                }

                PgpObject message = plainFact.NextPgpObject();

                if (message is PgpCompressedData)
                {
                    PgpCompressedData cData = (PgpCompressedData)message;
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
                else if (message is PgpLiteralData)
                {
                    PgpLiteralData ld = (PgpLiteralData)message;
                    string outFileName = ld.FileName;

                    Stream unc = ld.GetInputStream();
                    Streams.PipeAll(unc, outputStream);
                }
                else if (message is PgpOnePassSignatureList)
                    throw new PgpException("Encrypted message contains a signed message - not literal data.");
                else
                    throw new PgpException("Message is not a simple encrypted file - type unknown.");

                if (pbe.IsIntegrityProtected())
                {
                    if (!pbe.Verify())
                        MessageBox.Show("Message failed integrity check.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //else
                    //    MessageBox.Show("Message integrity check passed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //else
                //    MessageBox.Show("No message integrity check.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (PgpException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Test for existing key
        public static bool IsPgpSecretKey(string privateKeyFile)
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
                if (ex.Message == "Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKeyRing found where PgpSecretKeyRing expected")
                    return false;
                else
                {
                    MessageBox.Show(ex.Message, "Error");
                    return false;
                }
            }

            return true;
        }


        public static bool ExistsPassword(PgpSecretKeyRingBundle pgpSec, char[] password)
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
        public static bool ExistsPasswordFromString(string privateKeyString, char[] password)
        {
            using (var privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKeyString)))
            {
                return ExistsPasswordFromStream(privateKeyStream, password);
            }
        }

        public static bool ExistsPasswordFromStream(Stream privateKeyStream, char[] password)
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
                if (ex.Message == "Org.BouncyCastle.Bcpg.OpenPgp.PgpPublicKeyRing found where PgpSecretKeyRing expected")
                    MessageBox.Show("Public key given instead of private key", "Error");
                else if (!ex.Message.StartsWith("Checksum mismatch at "))
                    MessageBox.Show(ex.Message, "Error");
            }
            return false;
        }

        public static bool ExistsPassword(string privateKeyFile, char[] password)
        {
            if (string.IsNullOrEmpty(privateKeyFile))
            {
                MessageBox.Show("Private key file is missing (not chosen)", "Error");
                return false;
            }

            return ExistsPasswordFromStream(File.OpenRead(privateKeyFile), password);
        }

        #endregion

        #region Encrypt


        public static string GetEncryptedStringFromString(string decryptedText, string publicKeyText, bool armor, bool withIntegrityCheck)
        {
            try
            {
                using (Stream publicKeyStream = GenerateStreamFromString(publicKeyText))
                {
                        return GetEncryptedStringFromStream(decryptedText, publicKeyStream, armor, withIntegrityCheck);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message not decrypted because of error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return decryptedText;
            }
        }

        public static string GetEncryptedStringFromStream(string inputFile, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
        {
            try
            {
                using (var inputStream = GenerateStreamFromString(inputFile))
                {
                    using (var outputStream = new MemoryStream())
                    {

                        PGPEncryptDecrypt.EncryptFromStream(
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
                MessageBox.Show(ex.Message, "Message not encrypted because of error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return File.ReadAllText(inputFile);
            }
        }

        public static void Encrypt(string inputFile, string outputFile, string publicKeyFile, bool armor, bool withIntegrityCheck)
        {
            try
            {
                using (Stream publicKeyStream = File.OpenRead(publicKeyFile))
                {
                    using (Stream outputStream = File.Create(outputFile))
                    {
                        Encrypt(inputFile, outputFile, publicKeyFile, armor, withIntegrityCheck);
                    }
                }
            }
            catch (PgpException e)
            {
                throw e;
            }
        }

        public static void WriteStringToLiteralData(Stream outp, char fileType, String name, String buffer)
        {
            PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
            Stream pOut = lData.Open(outp, fileType, name, buffer.Length, DateTime.Now);
            pOut.Write(Encoding.UTF8.GetBytes(buffer), 0, buffer.Length);
        }

        public static void EncryptFromFile(Stream inputStream, Stream outputStream, string publicKeyFile, bool armor, bool withIntegrityCheck)
        {
            try
            {
                using (var publicKeyStreamReader = new StreamReader(publicKeyFile))
                {
                    EncryptFromStream(inputStream, outputStream, publicKeyStreamReader.BaseStream, armor, withIntegrityCheck);
                }
            }
            catch (PgpException e)
            {
                throw e;
            }
        }

        public static void EncryptFromStream(Stream inputStream, Stream outputStream, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
        {
            try
            {
                PgpPublicKey encKey = ReadPublicKey(publicKeyStream);

                using (MemoryStream bOut = new MemoryStream())
                {
                    PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

                    ////PgpUtilities.WriteFileToLiteralData(comData.Open(bOut), PgpLiteralData.Binary, new FileInfo(publicKeyFile));
                    using (var reader = new StreamReader(inputStream))
                    {
                        WriteStringToLiteralData(comData.Open(bOut), PgpLiteralData.Binary, /*to do: check if ok*/ string.Empty, reader.ReadToEnd());
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
            catch (PgpException e)
            {
                throw e;
            }
        }

        #endregion Encrypt

        #region Encrypt and Sign

        /*
         * Encrypt and sign the file pointed to by unencryptedFileInfo and
         */

        public static void EncryptAndSign(string inputFile, string outputFile, string publicKeyFile, string privateKeyFile, string passPhrase, bool armor)
        {
            PGPEncryptionKeys encryptionKeys = new PGPEncryptionKeys(publicKeyFile, privateKeyFile, passPhrase);

            if (!File.Exists(inputFile))
                throw new FileNotFoundException(String.Format("Input file [{0}] does not exist.", inputFile));

            if (!File.Exists(publicKeyFile))
                throw new FileNotFoundException(String.Format("Public Key file [{0}] does not exist.", publicKeyFile));

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException(String.Format("Private Key file [{0}] does not exist.", privateKeyFile));

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

        private static void OutputEncrypted(string inputFile, Stream outputStream, PGPEncryptionKeys encryptionKeys)
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

        private static void WriteOutputAndSign(Stream compressedOut, Stream literalOut, FileStream inputFile, PgpSignatureGenerator signatureGenerator)
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

        private static Stream ChainEncryptedOut(Stream outputStream, PGPEncryptionKeys m_encryptionKeys)
        {
            PgpEncryptedDataGenerator encryptedDataGenerator;
            encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.TripleDes, new SecureRandom());
            encryptedDataGenerator.AddMethod(m_encryptionKeys.PublicKey);
            return encryptedDataGenerator.Open(outputStream, new byte[BufferSize]);
        }

        private static Stream ChainCompressedOut(Stream encryptedOut)
        {
            PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            return compressedDataGenerator.Open(encryptedOut);
        }

        private static Stream ChainLiteralOut(Stream compressedOut, FileInfo file)
        {
            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, file);
        }

        private static PgpSignatureGenerator InitSignatureGenerator(Stream compressedOut, PGPEncryptionKeys m_encryptionKeys)
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

        private static PgpPublicKey ReadPublicKey(Stream inputStream)
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

        private static PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] password)
        {
            PgpSecretKey keyRing = pgpSec.GetSecretKey(keyId);

            if (keyRing == null)
                return null;

            return keyRing.ExtractPrivateKey(password);
        }

        #endregion Private helpers
    }
}
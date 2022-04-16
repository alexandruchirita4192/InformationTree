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
    public class PGPEncryptionAndSigningProvider : PGPEncryptionProvider, IPGPEncryptionAndSigningProvider
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PGPEncryptionAndSigningProvider(IPopUpService popUpService)
            : base(popUpService)
        {
        }
        
        #region Encrypt and Sign

        public void EncryptAndSignFile(string inputFile, string outputFile, string publicKeyFile, string privateKeyFile, string passPhrase, bool armor)
        {
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Input file [{inputFile}] does not exist.");

            if (!File.Exists(publicKeyFile))
                throw new FileNotFoundException($"Public Key file [{publicKeyFile}] does not exist.");

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException($"Private Key file [{privateKeyFile}] does not exist.");

            if (string.IsNullOrEmpty(passPhrase))
                throw new ArgumentNullException("Invalid passphrase.");

            var encryptionKeys = new PGPEncryptionKeys(publicKeyFile, privateKeyFile, passPhrase, true);
            if (encryptionKeys == null)
                throw new ArgumentNullException("Encryption Key not found.");

            var unencryptedFileInfo = new FileInfo(inputFile);
            var inputBytes = File.ReadAllBytes(inputFile);
            var inputName = unencryptedFileInfo.Name;
            var inputModificationDate = unencryptedFileInfo.LastWriteTime;

            using (var outputStream = File.Create(outputFile))
            {
                EncryptAndSign(armor, encryptionKeys, inputBytes, inputName, inputModificationDate, outputStream);
            }
        }

        public string EncryptAndSignString(string inputText, string publicKey, string privateKey, string passPhrase, bool armor)
        {
            if (string.IsNullOrEmpty(inputText))
                throw new ArgumentNullException("Invalid input text.");

            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentNullException("Invalid public key.");
            
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentNullException("Invalid private key.");

            if (string.IsNullOrEmpty(passPhrase))
                throw new ArgumentNullException("Invalid passphrase.");

            var encryptionKeys = new PGPEncryptionKeys(publicKey, privateKey, passPhrase, false);
            if (encryptionKeys == null)
                throw new ArgumentNullException("Encryption Key not found.");

            using (var outputStream = new MemoryStream())
            {
                EncryptAndSign(armor, encryptionKeys, Encoding.UTF8.GetBytes(inputText), "InputData", DateTime.Now, outputStream);
                return outputStream.ToString();
            }
        }

        private void EncryptAndSign(bool armor, PGPEncryptionKeys encryptionKeys, byte[] inputBytes, string inputName, DateTime inputModificationDate, Stream outputStream)
        {
            if (armor)
                using (var armoredOutputStream = new ArmoredOutputStream(outputStream))
                {
                    OutputEncrypted(inputBytes, inputName, inputModificationDate, outputStream, encryptionKeys);
                }
            else
                OutputEncrypted(inputBytes, inputName, inputModificationDate, outputStream, encryptionKeys);
        }

        private void OutputEncrypted(byte[] inputBytes, string inputName, DateTime inputModificationDate, Stream outputStream, PGPEncryptionKeys encryptionKeys)
        {
            using (var encryptedOut = ChainEncryptedOut(outputStream, encryptionKeys))
            {
                using (var compressedOut = ChainCompressedOut(encryptedOut))
                {
                    var signatureGenerator = InitSignatureGenerator(compressedOut, encryptionKeys);
                    using (var literalOut = ChainLiteralOut(compressedOut, inputBytes, inputName, inputModificationDate))
                    {
                        // Old code (if this code fails because of some stream issues): var inputFileStream = unencryptedFileInfo.OpenRead()
                        using (var inputMemoryStream = new MemoryStream(inputBytes))
                        {
                            WriteOutputAndSign(compressedOut, literalOut, inputMemoryStream, signatureGenerator);
                            inputMemoryStream.Close();
                        }
                    }
                }
            }
        }

        private void WriteOutputAndSign(Stream compressedOut, Stream literalOut, Stream inputStream, PgpSignatureGenerator signatureGenerator)
        {
            int length;
            var buf = new byte[BufferSize];
            while ((length = inputStream.Read(buf, 0, buf.Length)) > 0)
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
            var compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            return compressedDataGenerator.Open(encryptedOut);
        }

        private Stream ChainLiteralOut(Stream compressedOut, byte[] inputBytes, string inputName, DateTime inputModificationDate)
        {
            var pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, inputName, inputModificationDate, inputBytes);
        }

        private PgpSignatureGenerator InitSignatureGenerator(Stream compressedOut, PGPEncryptionKeys m_encryptionKeys)
        {
            const bool IsCritical = false;
            const bool IsNested = false;
            var tag = m_encryptionKeys.SecretKey.PublicKey.Algorithm;
            var pgpSignatureGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
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
    }
}
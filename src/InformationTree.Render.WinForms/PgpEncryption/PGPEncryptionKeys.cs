using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace InformationTree.PgpEncryption
{
    public class PGPEncryptionKeys
    {
        public PgpPublicKey PublicKey { get; private set; }

        public PgpPrivateKey PrivateKey { get; private set; }

        public PgpSecretKey SecretKey { get; private set; }

        /// <summary>
        /// Initializes a new instance of the EncryptionKeys class.
        /// Two keys are required to encrypt and sign data. Your private key and the recipients public key.
        /// The data is encrypted with the recipients public key and signed with your private key.
        /// </summary>
        /// <param name="recipientPublicKey">The recipient key used to encrypt the data</param>
        /// <param name="signerPrivateKey">The signer (your) key used to sign the data.</param>
        /// <param name="signerPassPhrase">The signer (your) password required to access the private key</param>
        /// <param name="isPathReceived">
        /// True if the parameters <paramref name="recipientPublicKey"/> and <paramref name="signerPrivateKey"/> are files 
        /// containing the public and private keys, false if the parameters are the public and private key string directly.</param>
        /// <exception cref="ArgumentException">Public key not found. Private key not found. Missing password</exception>
        public PGPEncryptionKeys(string recipientPublicKey, string signerPrivateKey, string signerPassPhrase, bool isPathReceived = true)
        {
            if (string.IsNullOrEmpty(recipientPublicKey))
                throw new ArgumentException("Public key info not found.", nameof(recipientPublicKey));
            if (string.IsNullOrEmpty(signerPrivateKey))
                throw new ArgumentException("Private key info not found.", nameof(signerPrivateKey));
            if (string.IsNullOrEmpty(signerPassPhrase))
                throw new ArgumentException("passPhrase is null or empty.", nameof(signerPassPhrase));

            // Note: There couldn't be another constructor because of the parameters with same types for public key path and private key path
            // vs public key input text and private key input text so another parameter isPathReceived was introduced
            // to choose based on what is the class is instantiated (input files or input text).

            if (isPathReceived)
            {
                if (!File.Exists(recipientPublicKey))
                    throw new ArgumentException("Public key file not found", nameof(recipientPublicKey));
                if (!File.Exists(signerPrivateKey))
                    throw new ArgumentException("Private key file not found", nameof(signerPrivateKey));
                PublicKey = ReadPublicKeyFromFile(recipientPublicKey);
                SecretKey = ReadSecretKeyFromFile(signerPrivateKey);
                PrivateKey = ReadPrivateKey(signerPassPhrase);
            }
            else
            {
                PublicKey = ReadPublicKeyFromString(recipientPublicKey);
                SecretKey = ReadSecretKeyFromString(signerPrivateKey);
                PrivateKey = ReadPrivateKey(signerPassPhrase);
            }
        }

        #region Secret Key

        private PgpSecretKey ReadSecretKeyFromFile(string signerPrivateKeyPath)
        {
            using (var keyIn = File.OpenRead(signerPrivateKeyPath))
            {
                return ReadSecretKey(keyIn);
            }
        }

        private PgpSecretKey ReadSecretKeyFromString(string signerPrivateKeyText)
        {
            using (var keyIn = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(signerPrivateKeyText)))
            {
                return ReadSecretKey(keyIn);
            }
        }

        private PgpSecretKey ReadSecretKey(Stream keyIn)
        {
            using (var inputStream = PgpUtilities.GetDecoderStream(keyIn))
            {
                var secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                var foundKey = GetFirstSecretKey(secretKeyRingBundle);
                if (foundKey != null)
                    return foundKey;
            }
            throw new ArgumentException("Can't find signing key in key ring.");
        }

        /// <summary>
        /// Return the first key we can use to encrypt.
        /// Note: A file can contain multiple keys (stored in "key rings")
        /// </summary>
        private PgpSecretKey GetFirstSecretKey(PgpSecretKeyRingBundle secretKeyRingBundle)
        {
            foreach (PgpSecretKeyRing kRing in secretKeyRingBundle.GetKeyRings())
            {
                var key = kRing.GetSecretKeys()
                    .Cast<PgpSecretKey>()
                    .Where(k => k.IsSigningKey)
                    .FirstOrDefault();
                if (key != null)
                    return key;
            }
            return null;
        }

        #endregion Secret Key

        #region Public Key

        private PgpPublicKey ReadPublicKeyFromFile(string recipientPublicKeyPath)
        {
            using (var keyIn = File.OpenRead(recipientPublicKeyPath))
            {
                return ReadPublicKey(keyIn);
            }
        }

        private PgpPublicKey ReadPublicKeyFromString(string recipientPublicKeyText)
        {
            using (var keyIn = new MemoryStream(Encoding.UTF8.GetBytes(recipientPublicKeyText)))
            {
                return ReadPublicKey(keyIn);
            }
        }

        private PgpPublicKey ReadPublicKey(Stream publicKeyStream)
        {
            using (var inputStream = PgpUtilities.GetDecoderStream(publicKeyStream))
            {
                var publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                var foundKey = GetFirstPublicKey(publicKeyRingBundle);
                if (foundKey != null)
                    return foundKey;
            }
            throw new ArgumentException("No encryption key found in public key ring.");
        }
        
        private PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
        {
            foreach (PgpPublicKeyRing kRing in publicKeyRingBundle.GetKeyRings())
            {
                var key = kRing.GetPublicKeys()
                    .Cast<PgpPublicKey>()
                    .Where(k => k.IsEncryptionKey)
                    .FirstOrDefault();
                if (key != null)
                    return key;
            }
            return null;
        }

        #endregion Public Key

        #region Private Key

        private PgpPrivateKey ReadPrivateKey(string signerPassPhrase)
        {
            var privateKey = SecretKey.ExtractPrivateKey(signerPassPhrase.ToCharArray());
            if (privateKey != null)
                return privateKey;
            throw new ArgumentException("No private key found in secret key.");
        }

        #endregion Private Key
    }
}
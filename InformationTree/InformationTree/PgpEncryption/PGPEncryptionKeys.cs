using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InformationTree.PgpEncryption
{
    // TODO: Use private key only to sign data (public key can be generated based on private key, in C# it's a property of the private key)
    // TODO: Integrate this class inside PGPEncryptionAndSigningProvider
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
        /// <param name="publicKey">The key used to encrypt the data</param>
        /// <param name="privateKey">The key used to sign the data.</param>
        /// <param name="passPhrase">The (your) password required to access the private key</param>
        /// <param name="isPathReceived">
        /// True if the parameters <paramref name="publicKey"/> and <paramref name="privateKey"/> are files 
        /// containing the public and private keys, false if the parameters are the public and private key string directly.</param>
        /// <exception cref="ArgumentException">Public key not found. Private key not found. Missing password</exception>
        public PGPEncryptionKeys(string publicKey, string privateKey, string passPhrase, bool isPathReceived = true)
        {
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("Public key info not found.", nameof(publicKey));
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("Private key info not found.", nameof(privateKey));
            if (string.IsNullOrEmpty(passPhrase))
                throw new ArgumentException("passPhrase is null or empty.", nameof(passPhrase));

            // Note: There couldn't be another constructor because of the parameters with same types for public key path and private key path
            // vs public key input text and private key input text so another parameter isPathReceived was introduced
            // to choose based on what is the class is instantiated (input files or input text).

            if (isPathReceived)
            {
                if (!File.Exists(publicKey))
                    throw new ArgumentException("Public key file not found", nameof(publicKey));
                if (!File.Exists(privateKey))
                    throw new ArgumentException("Private key file not found", nameof(privateKey));
                PublicKey = ReadPublicKeyFromFile(publicKey);
                SecretKey = ReadSecretKeyFromFile(privateKey);
                PrivateKey = ReadPrivateKey(passPhrase);
            }
            else
            {
                PublicKey = ReadPublicKeyFromString(publicKey);
                SecretKey = ReadSecretKeyFromString(privateKey);
                PrivateKey = ReadPrivateKey(passPhrase);
            }
        }

        #region Secret Key

        private PgpSecretKey ReadSecretKeyFromFile(string privateKeyPath)
        {
            using (Stream keyIn = File.OpenRead(privateKeyPath))
            {
                return ReadSecretKey(keyIn);
            }
        }

        private PgpSecretKey ReadSecretKeyFromString(string privateKey)
        {
            using (var keyIn = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(privateKey)))
            {
                return ReadSecretKey(keyIn);
            }
        }

        private PgpSecretKey ReadSecretKey(Stream keyIn)
        {
            using (var inputStream = PgpUtilities.GetDecoderStream(keyIn))
            {
                PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                PgpSecretKey foundKey = GetFirstSecretKey(secretKeyRingBundle);
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
                PgpSecretKey key = kRing.GetSecretKeys()
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

        private PgpPublicKey ReadPublicKeyFromFile(string publicKeyPath)
        {
            using (Stream keyIn = File.OpenRead(publicKeyPath))
            {
                return ReadPublicKey(keyIn);
            }
        }

        private PgpPublicKey ReadPublicKeyFromString(string publicKeyText)
        {
            using (Stream keyIn = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(publicKeyText)))
            {
                return ReadPublicKey(keyIn);
            }
        }

        private PgpPublicKey ReadPublicKey(Stream publicKeyStream)
        {
            using (Stream inputStream = PgpUtilities.GetDecoderStream(publicKeyStream))
            {
                PgpPublicKeyRingBundle publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                PgpPublicKey foundKey = GetFirstPublicKey(publicKeyRingBundle);
                if (foundKey != null)
                    return foundKey;
            }
            throw new ArgumentException("No encryption key found in public key ring.");
        }
        
        private PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
        {
            foreach (PgpPublicKeyRing kRing in publicKeyRingBundle.GetKeyRings())
            {
                PgpPublicKey key = kRing.GetPublicKeys()
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

        private PgpPrivateKey ReadPrivateKey(string passPhrase)
        {
            PgpPrivateKey privateKey = SecretKey.ExtractPrivateKey(passPhrase.ToCharArray());
            if (privateKey != null)
                return privateKey;
            throw new ArgumentException("No private key found in secret key.");
        }

        #endregion Private Key
    }
}
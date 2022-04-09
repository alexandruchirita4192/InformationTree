using System.IO;

namespace InformationTree.Domain.Services
{
    public interface IPGPEncryptionProvider
    {
        void EncryptFromFile(Stream inputStream, Stream outputStream, string publicKeyFile, bool armor, bool withIntegrityCheck);

        bool ExistsPassword(string privateKeyFile, char[] password);

        bool ExistsPasswordFromString(string privateKeyString, char[] password);

        string GetDecryptedStringFromFile(string encryptedText, string privateKeyFile, string pgpPassword);

        string GetDecryptedStringFromString(string encryptedText, string privateKey, string pgpPassword);

        string GetEncryptedStringFromString(string decryptedText, string publicKeyText, bool armor, bool withIntegrityCheck);
    }
}
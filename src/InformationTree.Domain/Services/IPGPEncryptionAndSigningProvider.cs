using System.IO;

namespace InformationTree.Domain.Services
{
    public interface IPGPEncryptionAndSigningProvider : IPGPEncryptionProvider
    {
        void EncryptAndSignFile(string inputFile, string outputFile, string publicKeyFile, string privateKeyFile, string passPhrase, bool armor);
        string EncryptAndSignString(string inputText, string publicKey, string privateKey, string passPhrase, bool armor);
    }
}
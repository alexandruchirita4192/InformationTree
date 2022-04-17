using System.IO;

namespace InformationTree.Domain.Services
{
    public interface IPGPEncryptionAndSigningProvider : IPGPEncryptionProvider
    {
        // TODO: Use private key only to sign data (public key can be generated based on private key, in C# it's a property of the private key)
        void EncryptAndSignFile(string inputFile, string outputFile, string publicKeyFile, string privateKeyFile, string passPhrase, bool armor);
        string EncryptAndSignString(string inputText, string publicKey, string privateKey, string passPhrase, bool armor);
    }
}
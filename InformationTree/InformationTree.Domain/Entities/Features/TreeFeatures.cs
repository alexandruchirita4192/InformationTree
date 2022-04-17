namespace InformationTree.Domain.Entities.Features
{
    public class TreeFeatures
    {
        public TreeFeatures(
            bool enableAutomaticCompression,
            bool enableManualEncryption,
            bool enableEncryptionSigning)
        {
            EnableAutomaticCompression = enableAutomaticCompression;
            EnableManualEncryption = enableManualEncryption;
            EnableEncryptionSigning = enableEncryptionSigning;
        }

        public bool EnableAutomaticCompression { get; private set; }
        public bool EnableManualEncryption { get; private set; }
        public bool EnableEncryptionSigning { get; private set; }
    }
}
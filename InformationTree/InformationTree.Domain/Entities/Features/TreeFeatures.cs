namespace InformationTree.Domain.Entities.Features
{
    public class TreeFeatures
    {
        public TreeFeatures(
            bool enableAutomaticCompression,
            bool enableManualEncryption)
        {
            EnableAutomaticCompression = enableAutomaticCompression;
            EnableManualEncryption = enableManualEncryption;
        }
        
        public bool EnableAutomaticCompression { get; set; }
        public bool EnableManualEncryption { get; set; }
    }
}
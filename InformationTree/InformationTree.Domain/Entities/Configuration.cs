using InformationTree.Domain.Entities.Features;

namespace InformationTree.Domain.Entities
{
    public class Configuration
    {
        public ApplicationFeatures ApplicationFeatures { get; private set; }
        public RicherTextBoxFeatures RicherTextBoxFeatures { get; private set; }
        public TreeFeatures TreeFeatures { get; private set; }
        
        public Configuration()
        {
            ApplicationFeatures = new ApplicationFeatures();
            RicherTextBoxFeatures = new RicherTextBoxFeatures();
            TreeFeatures = new TreeFeatures();
        }
    }
}
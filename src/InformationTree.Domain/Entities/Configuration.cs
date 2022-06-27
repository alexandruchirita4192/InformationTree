using System;
using InformationTree.Domain.Entities.Features;

namespace InformationTree.Domain.Entities
{
    public class Configuration
    {
        public ApplicationFeatures ApplicationFeatures { get; private set; }
        public RicherTextBoxFeatures RicherTextBoxFeatures { get; private set; }
        public TreeFeatures TreeFeatures { get; private set; }
        
        public Configuration(
            ApplicationFeatures applicationFeatures,
            RicherTextBoxFeatures richerTextBoxFeatures,
            TreeFeatures treeFeatures)
        {
            ApplicationFeatures = applicationFeatures ?? throw new ArgumentNullException(nameof(applicationFeatures));
            RicherTextBoxFeatures = richerTextBoxFeatures ?? throw new ArgumentNullException(nameof(richerTextBoxFeatures));
            TreeFeatures = treeFeatures ?? throw new ArgumentNullException(nameof(treeFeatures));
        }
    }
}
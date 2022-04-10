namespace InformationTree.Domain.Entities.Features
{
    public class RicherTextBoxFeatures
    {
        public RicherTextBoxFeatures(
            bool enableRtfLoading,
            bool enableRtfSaving,
            bool enableTable,
            bool enableCalculation,
            bool enableExtraNotImplementedMenus
            )
        {
            EnableRtfLoading = enableRtfLoading;
            EnableRtfSaving = enableRtfSaving;
            EnableTable = enableTable;
            EnableCalculation = enableCalculation;
            EnableExtraNotImplementedMenus = enableExtraNotImplementedMenus;
        }
        
        public bool EnableRtfLoading { get; private set; }
        public bool EnableRtfSaving { get; private set; }
        public bool EnableTable { get; private set; }
        public bool EnableCalculation { get; private set; }
        public bool EnableExtraNotImplementedMenus { get; private set; } // TODO: Break into separate features to be clear what is disabled if any
    }
}
namespace InformationTree.Domain.Entities.Features
{
    public class RicherTextBoxFeatures
    {
        public RicherTextBoxFeatures(
            bool enableRtfLoading,
            bool enableRtfSaving,
            bool enableTable,
            bool enableCalculation
            )
        {
            EnableRtfLoading = enableRtfLoading;
            EnableRtfSaving = enableRtfSaving;
            EnableTable = enableTable;
            EnableCalculation = enableCalculation;
        }
        
        public bool EnableRtfLoading { get; private set; }
        public bool EnableRtfSaving { get; private set; }
        public bool EnableTable { get; private set; }
        public bool EnableCalculation { get; private set; }
    }
}
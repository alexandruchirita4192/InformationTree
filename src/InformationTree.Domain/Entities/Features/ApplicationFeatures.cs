namespace InformationTree.Domain.Entities.Features
{
    public class ApplicationFeatures
    {
        public ApplicationFeatures(
            bool enableExtraSound,
            bool enableExtraGraphics,
            bool enableAlerts,
            bool enableProfiling)
        {
            EnableExtraSound = enableExtraSound;
            EnableExtraGraphics = enableExtraGraphics;
            EnableAlerts = enableAlerts;
            EnableProfiling = enableProfiling;
        }

        public bool EnableExtraSound { get; private set; }
        public bool EnableExtraGraphics { get; private set; }
        public bool EnableAlerts { get; private set; }
        public bool EnableProfiling { get; private set; }
    }
}
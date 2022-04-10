namespace InformationTree.Domain.Entities.Features
{
    public class ApplicationFeatures
    {
        public ApplicationFeatures(bool enableExtraSound, bool enableExtraGraphics)
        {
            EnableExtraSound = enableExtraSound;
            EnableExtraGraphics = enableExtraGraphics;
        }

        public bool EnableExtraSound { get; private set; }
        public bool EnableExtraGraphics { get; private set; }
    }
}
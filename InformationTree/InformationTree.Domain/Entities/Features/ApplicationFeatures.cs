namespace InformationTree.Domain.Entities.Features
{
    public class ApplicationFeatures
    {
        public ApplicationFeatures(bool enableExtraSound, bool enableExtraGraphics, bool mediatRSelfTest)
        {
            EnableExtraSound = enableExtraSound;
            EnableExtraGraphics = enableExtraGraphics;
            MediatorSelfTest = mediatRSelfTest;
        }

        public bool EnableExtraSound { get; private set; }
        public bool EnableExtraGraphics { get; private set; }
        public bool MediatorSelfTest { get; private set; }
    }
}
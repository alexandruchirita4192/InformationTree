namespace InformationTree.Domain.Services
{
    public interface ISoundProvider
    {
        void PlaySound(string file);

        void PlaySystemSound(string soundName);

        void PlaySystemSound(int soundNumber);
    }
}
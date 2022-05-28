namespace InformationTree.Domain.Services
{
    public interface ICachingService
    {
        void Set<T>(string key, T item);
        object Get(string key);
        T Get<T>(string key);
    }
}
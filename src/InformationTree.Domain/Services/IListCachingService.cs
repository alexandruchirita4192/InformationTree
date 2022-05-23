using System.Collections;

namespace InformationTree.Domain.Services
{
    public interface IListCachingService
    {
        void Set(string key, IList collection);

        IList Get(string key);
    }
}
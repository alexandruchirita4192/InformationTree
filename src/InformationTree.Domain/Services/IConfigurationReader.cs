using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IConfigurationReader
    {
        Configuration GetConfiguration();
    }
}
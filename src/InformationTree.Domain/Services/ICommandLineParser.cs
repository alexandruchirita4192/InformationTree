using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface ICommandLineParser
    {
        void Parse(string[] args, Configuration configuration);
    }
}
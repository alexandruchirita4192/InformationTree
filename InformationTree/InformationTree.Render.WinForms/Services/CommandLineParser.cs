using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class CommandLineParser : ICommandLineParser
    {
        public void Parse(string[] args, Configuration configuration)
        {
            // TODO: Maybe parse at least for a file and set it to configuration or something??
        }
    }
}
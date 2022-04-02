using System.Configuration;

namespace InformationTree.Render.WinForms.Services
{
    public class ConfigurationReader : Domain.Services.IConfigurationReader
    {
        public Domain.Entities.Configuration GetConfiguration()
        {
            var configuration = new Domain.Entities.Configuration();

            // TODO: Read configuration from configuration file
            //configuration.ApplicationFeatures.EnableExtraGraphics = ConfigurationManager.AppSettings[nameof(configuration.ApplicationFeatures.EnableExtraGraphics)];

            return configuration;
        }
    }
}
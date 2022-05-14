using System;
using System.Collections.Specialized;
using System.Configuration;
using NLog;

namespace InformationTree.Render.WinForms.Services
{
    public class ConfigurationReader : Domain.Services.IConfigurationReader
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Domain.Entities.Configuration _configuration;

        public Domain.Entities.Configuration GetConfiguration()
        {
            if (_configuration == null)
            {
                var appSettings = ConfigurationManager.AppSettings;

                _configuration = new Domain.Entities.Configuration(
                    new Domain.Entities.Features.ApplicationFeatures(
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.ApplicationFeatures.EnableExtraGraphics)),
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.ApplicationFeatures.EnableExtraSound)),
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.ApplicationFeatures.MediatorSelfTest))
                        ),
                    new Domain.Entities.Features.RicherTextBoxFeatures(
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.RicherTextBoxFeatures.EnableRtfLoading)),
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.RicherTextBoxFeatures.EnableRtfSaving)),
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.RicherTextBoxFeatures.EnableTable)),
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.RicherTextBoxFeatures.EnableCalculation))
                        ),
                    new Domain.Entities.Features.TreeFeatures(
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.TreeFeatures.EnableManualEncryption)),
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.TreeFeatures.EnableAutomaticCompression)),
                        ReadConfiguration(appSettings, nameof(Domain.Entities.Features.TreeFeatures.EnableEncryptionSigning))
                        )
                    );
            }

            return _configuration;
        }

        private static bool ReadConfiguration(NameValueCollection appSettings, string configurationName, bool defaultValue = false)
        {
            if (appSettings == null
                || string.IsNullOrWhiteSpace(configurationName)
                || appSettings[configurationName] == null)
            {
                return defaultValue;
            }

            try
            {
                var configurationValue = Convert.ToBoolean(appSettings[configurationName]);
                return configurationValue;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error reading {configurationName} configuration value.");
                return defaultValue;
            }
        }
    }
}
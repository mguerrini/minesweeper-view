using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.IO;
using System.Xml.Serialization;

namespace Minesweeper.Core.Configurations.Configurations
{
    [Serializable]
    [XmlRoot("AppConfigProvider")]
    public class AppConfigConfigurationProviderConfiguration : FileConfigurationProviderConfiguration
    {
        #region -- Constructors --

        public AppConfigConfigurationProviderConfiguration()
        {
        }

        public AppConfigConfigurationProviderConfiguration(string fileName)
        {
            this.FileName = fileName;
        }

        #endregion


        public override IConfigurationProvider CreateConfigurationProvider(object globalConfig)
        {
            if (this.RefreshTime <= TimeSpan.Zero)
                this.RefreshTime = new TimeSpan(0, 1, 0);

            if (string.IsNullOrEmpty(this.FileName))
            {
                AppConfigConfigurationProvider output;

                if (HttpContext.Current == null && OperationContext.Current == null)
                    output = new AppConfigConfigurationProvider();
                else
                    output = new WebConfigConfigurationProvider();

                output.RefreshTime = this.RefreshTime;

                return output;
            }
            else
            {
                AppConfigConfigurationProvider local = new AppConfigConfigurationProvider();
                string baseDirectory = local.ApplicationPath;

                if (globalConfig != null && globalConfig is CompositeConfigurationProviderConfiguration) 
                {
                    CompositeConfigurationProviderConfiguration cComp = globalConfig as CompositeConfigurationProviderConfiguration;

                    if (!string.IsNullOrEmpty(cComp.BaseDirectory))
                    {
                        baseDirectory = Path.Combine(baseDirectory, cComp.BaseDirectory);
                        baseDirectory = Path.GetFullPath(baseDirectory);
                    }
                }

                baseDirectory = Path.Combine(baseDirectory, this.FileName);
                local.ConfigurationFileName = baseDirectory;
                local.RefreshTime = this.RefreshTime;

                return local;
            }
        }
    }
}

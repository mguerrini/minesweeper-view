namespace Minesweeper.Core.Configurations.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;
    using System.Xml.Serialization;
    using Minesweeper.Core.Configurations.Configurations;
    using Minesweeper.Core.Configurations.Providers;

    [Serializable]
    public class CompositeConfigurationProviderConfiguration : ConfigurationProviderConfiguration
    {
        [XmlAttribute("baseDirectory")]
        public string BaseDirectory { get; set; }

        [XmlArray("Providers")]
        [XmlArrayItem("AppConfigProvider", Type = typeof(AppConfigConfigurationProviderConfiguration))]
        [XmlArrayItem("LocalConfigProvider", Type = typeof(LocalConfigurationProviderConfiguration))]
        public virtual List<IConfigurationProviderConfiguration> Providers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalConfig"></param>
        /// <returns></returns>
        public override IConfigurationProvider CreateConfigurationProvider(object globalConfig)
        {
            CompositeConfigurationProvider output = new CompositeConfigurationProvider(Minesweeper.Core.Configurations.ConfigurationProvider.CreateLocalProvider());
            CompositeConfigurationProviderConfiguration config = this;

            if (config.Providers != null && config.Providers.Count > 0)
            {
                foreach (var c in config.Providers)
                {
                    IConfigurationProvider provider = c.CreateConfigurationProvider(globalConfig);
                    output.AddProvider(provider);
                }
            }

            return output;
        }
    }
}

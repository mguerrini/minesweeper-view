namespace Minesweeper.Core.Configurations.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ConfigurationProviderConfiguration : IConfigurationProviderConfiguration
    {
        public ConfigurationProviderConfiguration()
        {
            this.IsReadOnly = true;
        }


        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        [XmlAttribute("isReadOnly")]
        public bool IsReadOnly { get; set; }

        public abstract IConfigurationProvider CreateConfigurationProvider(object globalConfig);
    }
}

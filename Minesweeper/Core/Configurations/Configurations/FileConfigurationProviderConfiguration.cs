using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Minesweeper.Core.Configurations.Configurations;
using System.Xml.Serialization;

namespace Minesweeper.Core.Configurations.Configurations
{
    [Serializable]
    public abstract class FileConfigurationProviderConfiguration : ConfigurationProviderConfiguration
    {
        public FileConfigurationProviderConfiguration()
        {
            this.RefreshTime = TimeSpan.Zero;
        }

        [XmlAttribute("filename")]
        public string FileName { get; set; }

        [XmlAttribute("refreshTime")]
        public TimeSpan RefreshTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web;
using System.ServiceModel;

namespace Minesweeper.Core.Configurations.Configurations
{
    [Serializable]
    [XmlRoot("LocalConfigProvider")]
    public class LocalConfigurationProviderConfiguration : AppConfigConfigurationProviderConfiguration
    {
        public override IConfigurationProvider CreateConfigurationProvider(object globalConfig)
        {
            if (HttpContext.Current == null && OperationContext.Current == null)
                return new AppConfigConfigurationProvider();
            else
                return new WebConfigConfigurationProvider();
        }
    }
}

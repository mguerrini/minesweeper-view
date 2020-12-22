using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper.Core.Configurations.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConfigurationProviderConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalConfig"></param>
        /// <returns></returns>
        IConfigurationProvider CreateConfigurationProvider(object globalConfig);
    }
}

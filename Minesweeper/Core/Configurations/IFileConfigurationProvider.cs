using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper.Core.Configurations
{
    public interface IFileConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        void Load(string filePath);
    }
}

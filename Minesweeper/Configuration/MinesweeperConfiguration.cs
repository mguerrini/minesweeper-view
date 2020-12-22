using Minesweeper.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Minesweeper.Configuration
{
    [Serializable]
    public class MinesweeperConfiguration
    {
        [XmlAttribute("minesweeperServiceFactoryName")]
        public string MinesweeperServiceFactoryName { get; set; }
    }


    [Serializable]
    public class MinesweeperConfigurationSection : ConfigurationSectionAdapter<MinesweeperConfiguration>
    {

    }


}

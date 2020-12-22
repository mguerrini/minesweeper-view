using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Minesweeper.Services
{
    [Serializable]
    public class MinesweeperServicesConfiguration
    {
        [XmlAttribute("baseUrl")]
        public string BaseUrl { get; set; }
    }
}

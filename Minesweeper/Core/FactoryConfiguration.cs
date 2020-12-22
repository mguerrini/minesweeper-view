using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    [Serializable]
    public class FactoryConfiguration : ComponentConfiguration
    {
        public FactoryConfiguration(object config) : base(config) { }
    }
}

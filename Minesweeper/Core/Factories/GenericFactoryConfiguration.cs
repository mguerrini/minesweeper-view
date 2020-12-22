using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Minesweeper.Core.Factories
{
    [Serializable]
    public class GenericFactoryConfiguration : FactoryConfiguration
    {
        [XmlAttribute("instanceType")]
        public Type InstanceType { get; set; }

        [XmlAttribute("factoryType")]
        public override Type FactoryType
        {
            get
            {
                Type output = base.FactoryType;
                if (output == null)
                {
                    if (this.InstanceType != null)
                    {
                        Type factoryType = typeof(GenericFactory<>);
                        factoryType = factoryType.MakeGenericType(this.InstanceType);
                        base.FactoryType = factoryType;
                    }
                }

                return base.FactoryType;
            }
            set
            {
                base.FactoryType = value;
            }
        }
    }
}

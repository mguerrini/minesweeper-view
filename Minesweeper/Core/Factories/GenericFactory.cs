using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core.Factories
{
    public class GenericFactory<TEntity> : Factory<TEntity>
    {
        private FactoryConfiguration InstanceConfiguration { get; set; }

        protected override TEntity DoCreateEntity()
        {
            Type t = typeof(TEntity);

            if (t.IsAssignableFrom(typeof(IConfigurable)))
            {
                //la configuracion no es del factory, es del componente.
                object output = Activator.CreateInstance(typeof(TEntity));

                IConfigurable configurable = output as IConfigurable;
                configurable.Configure(this.InstanceConfiguration);

                return (TEntity)output;
            }
            else
            {
                ConstructorInfo[] info = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                Type c = null;
                foreach (var i in info)
                {
                    var prms = i.GetParameters();
                    if (prms.Length == 1)
                    {
                        c = prms[0].ParameterType;
                        break;
                    }
                }

                if (c==null)
                {
                    return (TEntity) Activator.CreateInstance(typeof(TEntity));
                }
                else
                {
                    object instanceConfig = this.InstanceConfiguration.GetConfiguration(c);
                    object output = Activator.CreateInstance(typeof(TEntity), instanceConfig);

                    return (TEntity)output;
                }
            }

        }


        protected override void Configure(FactoryConfiguration configData)
        {
            base.Configure(configData);
            this.InstanceConfiguration = configData;
        }
    }
}

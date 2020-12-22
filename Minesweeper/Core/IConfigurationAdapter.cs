using Minesweeper.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    public class IConfigurationAdapter : IConfiguration
    {
        public IConfigurationAdapter()
        {

        }

        public IConfigurationAdapter(string name)
        {
            this.Name = name;
        }

        public IConfigurationAdapter(string name, object config)
        {
            this.Name = name;
            this.Configuration = config;
        }

        public string Name { get; set; }

        private object Configuration { get; set; }

        /// <summary>
        /// Devuelve el objeto tal cual fue asigna
        /// </summary>
        /// <returns></returns>
        public object GetConfiguration()
        {
            return this.Configuration;
        }

        public object GetConfiguration(Type typeOf)
        {
            if (this.Configuration == null)
                return null;
            if (TypeHelper.CanAssign(this.Configuration, typeOf))
                return this.Configuration;

            return null;
        }

        public TConfiguration GetConfiguration<TConfiguration>()
        {
            return (TConfiguration) this.GetConfiguration(typeof(TConfiguration));
        }
    }
}

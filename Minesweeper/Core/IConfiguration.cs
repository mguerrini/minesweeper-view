using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    public interface IConfiguration
    {
        string Name { get; }

        /// <summary>
        /// Devuelve la configuracion tal cual esta contenida.
        /// </summary>
        /// <returns></returns>
        object GetConfiguration();

        /// <summary>
        /// Obtiene la configuración para la instancia del elemento.
        /// </summary>
        /// <param name="typeOf">Tipo de la configuración</param>
        /// <returns>The configuration.</returns>
        object GetConfiguration(Type typeOf);

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">Configuration type.</typeparam>
        /// <returns>Resulting configuration.</returns>
        TConfiguration GetConfiguration<TConfiguration>();
    }
}

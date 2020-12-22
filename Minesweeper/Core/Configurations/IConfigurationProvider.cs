namespace Minesweeper.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;

    /// <summary>
    /// 
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Devuelve el value asociado al key del AppSettings.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetSettings(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        bool ExistsConfiguration(Type configType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ExistsConfiguration(Type configType, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <returns></returns>
        bool ExistsConfiguration<TConfiguration>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ExistsConfiguration<TConfiguration>(string name);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <returns></returns>
        TConfiguration GetConfiguration<TConfiguration>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        TConfiguration GetConfiguration<TConfiguration>(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        object GetConfiguration(Type configType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetConfiguration(Type configType, string name);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <returns></returns>
        List<TConfiguration> GetAllConfigurations<TConfiguration>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        List<object> GetAllConfigurations(Type configType);
        /*
        /// <summary>
        /// Graba o crea la configuracion.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        bool Save(object config);

        /// <summary>
        /// Graba o crea la configuracion con ese nombre.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        void Save(string name, object config);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType">Tipo de configuracion que debe actualizar. 
        /// La implementacion de la configuracion puede ser diferente al tipo buscado.</param>
        /// <param name="config"></param>
        /// <returns></returns>
        bool Save(Type configType, object config);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configType"></param>
        /// <param name="config"></param>
        void Save(string name, Type configType, object config);
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetConnectionString(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ConnectionStringSettings GetConnectionStringSettings(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ConnectionStringSettingsCollection GetConnectionStringSettings();
    }
}

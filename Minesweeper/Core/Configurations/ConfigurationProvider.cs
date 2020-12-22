namespace Minesweeper.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Configuration;
    using Minesweeper.Core.Configurations.Configurations;
    using Providers;

    public abstract class ConfigurationProvider : IConfigurationProvider
    {
        #region -- Singleton --

        private static IConfigurationProvider _instance;
        private static object locker = new object();
        private static bool IsInitializing = false;

        /// <summary>
        /// 
        /// </summary>
        public static IConfigurationProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (IsInitializing)
                        throw new InvalidOperationException("The Configuration Singleton instance is initializing. Can not get the single instance.");

                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            
                            try
                            {
                                IsInitializing = true;

                                IConfigurationProvider defConfig = ConfigurationProvider.CreateLocalProvider();
                                ConfigurationProviderConfiguration config = null;

                                if (defConfig.ExistsConfiguration<ConfigurationProviderConfiguration>())
                                    config = defConfig.GetConfiguration<ConfigurationProviderConfiguration>();

                                if (config != null)
                                {
                                    IConfigurationProvider prov = config.CreateConfigurationProvider(config);
                                    _instance = prov;
                                }
                                else
                                {
                                    //DefaultLogger.Info("Default configuration provider was created because no other was configured.");
                                    _instance = defConfig;
                                }

                                if (! (_instance is CompositeConfigurationProvider))
                                {
                                    CompositeConfigurationProvider comp = new CompositeConfigurationProvider(_instance);
                                    _instance = comp;
                                }
                            }
                            finally
                            {
                                IsInitializing = false;
                            }
                        }
                    }
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private static IConfigurationProvider _localConfigurationProvider;
        private static LocalConfigurationProviderConfiguration _localConfig = new LocalConfigurationProviderConfiguration();
        /// <summary>
        /// Crea el provider que corresponda según el tipo de aplicación (Web o no Web).
        /// </summary>
        /// <returns></returns>
        public static IConfigurationProvider CreateLocalProvider()
        {
            lock (_localConfig)
            {
                if (_localConfigurationProvider == null)
                    _localConfigurationProvider = _localConfig.CreateConfigurationProvider(null);
            }

            return _localConfigurationProvider;
        }

        #endregion


        #region -- IConfigurationProvider Members --

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract string GetSettings(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <returns></returns>
        public virtual TConfiguration GetConfiguration<TConfiguration>()
        {
            return (TConfiguration)this.GetConfiguration(typeof(TConfiguration));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual TConfiguration GetConfiguration<TConfiguration>(string name)
        {
            return (TConfiguration)this.GetConfiguration(typeof(TConfiguration), name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <returns></returns>
        public virtual List<TConfiguration> GetAllConfigurations<TConfiguration>()
        {
            List<object> list = this.GetAllConfigurations(typeof(TConfiguration));

            List<TConfiguration> output = new List<TConfiguration>();

            if (list != null)
                list.ForEach(i => output.Add((TConfiguration)i));

            return output;
        }


        public abstract bool ExistsConfiguration(Type configType);

        public abstract bool ExistsConfiguration(Type configType, string name);

        public virtual bool ExistsConfiguration<TConfiguration>()
        {
            return this.ExistsConfiguration(typeof(TConfiguration));
        }

        public virtual bool ExistsConfiguration<TConfiguration>(string name)
        {
            return this.ExistsConfiguration(typeof(TConfiguration), name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public abstract object GetConfiguration(Type configType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract object GetConfiguration(Type configType, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public abstract List<object> GetAllConfigurations(Type configType);

        #endregion


        #region -- Connection String --

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConnectionStringSettingsCollection GetConnectionStringSettings()
        {
            ConnectionStringsSection section = this.GetConfiguration<ConnectionStringsSection>("connectionStrings");
            if (section == null)
                return null;

            return  section.ConnectionStrings;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ConnectionStringSettings GetConnectionStringSettings(string name)
        {
            ConnectionStringsSection section = this.GetConfiguration<ConnectionStringsSection>("connectionStrings");
            if (section == null)
                return null;

            ConnectionStringSettingsCollection cnns = section.ConnectionStrings;
            return cnns[name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetConnectionString(string name)
        {
            ConnectionStringSettings set = this.GetConnectionStringSettings(name);
            if (set == null)
                return null;
            else
                return set.ConnectionString;
        }

        #endregion
    }
}

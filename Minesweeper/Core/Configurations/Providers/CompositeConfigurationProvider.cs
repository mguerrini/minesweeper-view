namespace Minesweeper.Core.Configurations.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;
    using System.IO;
    using Minesweeper.Core.Configurations.Configurations;
    //using Minesweeper.Core.Log;


    public class CompositeConfigurationProvider : IConfigurationProvider
    {
        #region -- Constructors --

        public CompositeConfigurationProvider(IConfigurationProvider local)
        {
            this.LocalConfigurationProvider = local;
            this.Providers = new List<IConfigurationProvider>();
        }

        #endregion


        public string BaseDirectory { get; set; }
        
        #region -- Providers --

        private List<IConfigurationProvider> Providers { get; set; }

        public void AddProvider(string fileName)
        {
            AppConfigConfigurationProviderConfiguration appConfig = new AppConfigConfigurationProviderConfiguration(fileName);
            IConfigurationProvider prov = appConfig.CreateConfigurationProvider(null);
            this.AddProvider(prov);
        }

        public void AddProvider(IConfigurationProvider prov)
        {
            this.Providers.Add(prov);
        }
        public void InsertProvider(int index, IConfigurationProvider prov)
        {
            this.Providers.Insert(index, prov);
        }
        public void Load(string fileName)
        {
            this.AddProvider(fileName);
        }

        #endregion




        /// <summary>
        /// 

        public IConfigurationProvider LocalConfigurationProvider
        {
            get;
            protected set;
        }


        #region -- Settings --

        public string GetSettings(string key)
        {
            string output = this.LocalConfigurationProvider.GetSettings(key);

            if (output == null)
            {
                foreach (var p in this.Providers)
                {
                    output = p.GetSettings(key);
                    if (output != null)
                        break;
                }
            }

            return output;
        }

        #endregion


        #region -- Configurations --

        public bool ExistsConfiguration(Type configType)
        {
            foreach (var p in this.Providers)
            {
                if (p.ExistsConfiguration(configType))
                    return true;
            }

            return this.LocalConfigurationProvider.ExistsConfiguration(configType);
        }

        public bool ExistsConfiguration(Type configType, string name)
        {
            foreach (var p in this.Providers)
            {
                if (p.ExistsConfiguration(configType, name))
                    return true;
            }

            return this.LocalConfigurationProvider.ExistsConfiguration(configType, name);
        }

        public bool ExistsConfiguration<TConfiguration>()
        {
            foreach (var p in this.Providers)
            {
                if (p.ExistsConfiguration<TConfiguration>())
                    return true;
            }

            return this.LocalConfigurationProvider.ExistsConfiguration<TConfiguration>();
        }

        public bool ExistsConfiguration<TConfiguration>(string name)
        {
            foreach (var p in this.Providers)
            {
                if (p.ExistsConfiguration<TConfiguration>(name))
                    return true;
            }

            return this.LocalConfigurationProvider.ExistsConfiguration<TConfiguration>(name);
        }


        public TConfiguration GetConfiguration<TConfiguration>()
        {
            //DefaultLogger.Debug("ConfigurationProvider - Get configuration of type {0}", typeof(TConfiguration).FullName);

            object output = null;

            foreach (var p in this.Providers)
            {
                output = p.GetConfiguration<TConfiguration>();
                if (output != null)
                    break;
            }

            if (output == null)
                output = this.LocalConfigurationProvider.GetConfiguration<TConfiguration>();

            return (TConfiguration)output;
        }

        public TConfiguration GetConfiguration<TConfiguration>(string name)
        {
            //DefaultLogger.Debug("ConfigurationProvider - Get configuration with name {0} and type {1}", name, typeof(TConfiguration).FullName);

            object output = null; // this.LocalConfigurationProvider.GetConfiguration<TConfiguration>(name);

                foreach (var p in this.Providers)
                {
                    output = p.GetConfiguration<TConfiguration>(name);
                    if (output != null)
                        break;
                }

            if (output == null)
                output = this.LocalConfigurationProvider.GetConfiguration<TConfiguration>(name);

            return (TConfiguration)output;
        }


        public object GetConfiguration(Type configType)
        {
            //DefaultLogger.Debug("ConfigurationProvider - Get configuration of type {0}", configType.FullName);

            object output = null;

                foreach (var p in this.Providers)
                {
                    output = p.GetConfiguration(configType);
                    if (output != null)
                        break;
                }

            if (output == null)
                output = this.LocalConfigurationProvider.GetConfiguration(configType);

            return output;
        }

        public object GetConfiguration(Type configType, string name)
        {
            //DefaultLogger.Debug("ConfigurationProvider - Get configuration with name {0} and type {1}", name, configType.FullName);

            object output = null; // this.LocalConfigurationProvider.GetConfiguration(configType, name);

                foreach (var p in this.Providers)
                {
                    output = p.GetConfiguration(configType, name);
                    if (output != null)
                        break;
                }

            if (output == null)
                output = this.LocalConfigurationProvider.GetConfiguration(configType, name);

            return output;
        }


        public List<TConfiguration> GetAllConfigurations<TConfiguration>()
        {
            //DefaultLogger.Debug("ConfigurationProvider - Get all configurations of type {0}", typeof(TConfiguration).FullName);
            List<TConfiguration> output = new List<TConfiguration>();
            List<TConfiguration> aux;

            foreach (var p in this.Providers)
            {
                aux = p.GetAllConfigurations<TConfiguration>();
                output.AddRange(aux);
            }

            aux = this.LocalConfigurationProvider.GetAllConfigurations<TConfiguration>();
            output.AddRange(aux);
            return output;
        }

        public List<object> GetAllConfigurations(Type configType)
        {
            //DefaultLogger.Debug("ConfigurationProvider - Get all configurations of type {0}", configType.FullName);
            List<object> output = new List<object>();
            List<object> aux;

            foreach (var p in this.Providers)
            {
                aux = p.GetAllConfigurations(configType);
                output.AddRange(aux);
            }

            aux = this.LocalConfigurationProvider.GetAllConfigurations(configType);
            output.AddRange(aux);
            return output;
        }

        #endregion


        #region -- Connection String --

        public string GetConnectionString(string name)
        {

            string output = null;

                foreach (var p in this.Providers)
                {
                    output = p.GetConnectionString(name);
                    if (output != null)
                        break;
                }

            if (string.IsNullOrEmpty(output))
                output = this.LocalConfigurationProvider.GetConnectionString(name);

            //if (output == null)
            //    DefaultLogger.Warning("The connection string with name {0} not exists or the name is invalid.", name);
            return output;
        }

        public ConnectionStringSettings GetConnectionStringSettings(string name)
        {
            ConnectionStringSettings output = null;


                foreach (var p in this.Providers)
                {
                    output = p.GetConnectionStringSettings(name);
                    if (output != null)
                        break;
                }

            if (output == null)
                output = this.LocalConfigurationProvider.GetConnectionStringSettings(name);

            //if (output == null)
            //    DefaultLogger.Warning("The connection string settings with name {0} not exists or the name is invalid.", name);
            return output;
        }



        public ConnectionStringSettingsCollection GetConnectionStringSettings()
        {
            //tengo que devolver todas las conexiones
            ConnectionStringSettings sett;
            ConnectionStringSettingsCollection output = new ConnectionStringSettingsCollection();

            foreach (var p in this.Providers)
            {
                ConnectionStringSettingsCollection inner = p.GetConnectionStringSettings();
                if (inner != null)
                {
                    for (int i = 0; i < inner.Count; i++)
                    {
                        sett = inner[i];
                        output.Add(sett);
                    }
                }
            }
            ConnectionStringSettingsCollection local = this.LocalConfigurationProvider.GetConnectionStringSettings();
            for (int i = 0; i < local.Count; i++)
            {
                sett = local[i];
                output.Add(sett);
            }
            return output;
        }
        #endregion
    }
}

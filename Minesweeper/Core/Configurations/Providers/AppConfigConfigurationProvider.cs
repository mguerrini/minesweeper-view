namespace Minesweeper.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class AppConfigConfigurationProvider : ConfigurationProvider, IFileConfigurationProvider
    {
        private System.Configuration.Configuration _config;
        private string _externalConfigurationFileName;
        private object ConfigLocker = new object();


        #region -- Constructors --

        /// <summary>
        /// 
        /// </summary>
        public AppConfigConfigurationProvider()
        {
            this.RefreshTime = new TimeSpan(0, 1, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public AppConfigConfigurationProvider(string fileName)
        {
            this.ConfigurationFileName = fileName;
            this.RefreshTime = new TimeSpan(0, 1, 0);
        }

        #endregion


        #region -- Properties --

        public TimeSpan RefreshTime { get; set; }

        private DateTime LastReadTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ConfigurationFileName
        {
            get { return _externalConfigurationFileName; }
            set
            {
                _externalConfigurationFileName = value;
                _config = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ApplicationPath
        {
            get
            {
                //string p = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                //p = Path.GetDirectoryName(p);

                return Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public virtual void Load(string fileName)
        {
            this.ConfigurationFileName = fileName;
        }


        #region -- IConfigurationProvider Members --

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetSettings(string key)
        {
            KeyValueConfigurationElement item = this.GetConfigurationManager().AppSettings.Settings[key];
            if (item != null)
                return item.Value;
            else
                return string.Empty;
        }

        public override bool ExistsConfiguration(Type configType)
        {
            return this.GetConfiguration(configType, this.GetConfigurationManager(), false) != null;
        }
        public override bool ExistsConfiguration(Type configType, string name)
        {
            return this.GetConfiguration(configType, name, this.GetConfigurationManager(), false) != null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public override object GetConfiguration(Type configType)
        {
            return this.GetConfiguration(configType, this.GetConfigurationManager(), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetConfiguration(Type configType, string name)
        {
            return this.GetConfiguration(configType, name, this.GetConfigurationManager(), false);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected virtual object GetConfiguration(Type configType, System.Configuration.Configuration source, bool errorOnNotExists)
        {
            ConfigurationSection section;
            ConfigurationSectionCollection sections = source.Sections;
            string currentName = null;
            object output = null;

            for (int i = 0; i < sections.Count; i++)
            {
                section = sections[i];
                if (this.IsConfigurationOfType(configType, section, out currentName, out output))
                    return output;
            }

            if (errorOnNotExists)
                throw new ConfigurationErrorsException("La sección " + output.GetType().FullName + " no es subclase de la sección solicitada (" + configType.FullName + ").");

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected virtual object GetConfiguration(Type configType, string name, System.Configuration.Configuration source, bool errorOnNotExists)
        {
            string currentName = null;
            object output = null;

            ConfigurationSection config = source.GetSection(name);

            //if (output != null && output.GetType() != configType && !output.GetType().IsSubclassOf(configType) && !configType.IsAssignableFrom(output.GetType()))
            if (!this.IsConfigurationOfType(configType, config, out currentName, out output))
            {
                if (errorOnNotExists)
                    throw new ConfigurationErrorsException("La sección " + output.GetType().FullName + " no es subclase de la sección solicitada (" + configType.FullName + ") de nombre " + name);
                else
                    return null;
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public override List<object> GetAllConfigurations(Type configType)
        {
            List<object> output = new List<object>();

            System.Configuration.Configuration source = this.GetConfigurationManager();
            ConfigurationSection section;
            ConfigurationSectionCollection sections = source.Sections;
            string currentName = null;
            object config = null;

            for (int i = 0; i < sections.Count; i++)
            {
                section = sections[i];
                if (this.IsConfigurationOfType(configType, section, out currentName, out config))
                    output.Add(config);
            }


            return output;
}
       protected bool IsConfigurationOfType(Type configType, ConfigurationSection config, out string name, out object configuration)
        {
            name = null;
            configuration = null;
            if (config == null)
                return false;


            //inicializo
            name = config.SectionInformation.Name;
            Type inputConfigType = config.GetType();

            if (configType.Equals(inputConfigType))
            {
                configuration = config;
                return true;
            }

            if (inputConfigType.IsSubclassOf(configType))
            {
                configuration = config;
                return true;
            }

            if (configType.IsAssignableFrom(inputConfigType))
            {
                configuration = config;
                return true;
            }

            if (config is IConfigurationSectionAdapter)
            {
                IConfigurationSectionAdapter adapter = config as IConfigurationSectionAdapter;
                inputConfigType = adapter.ConfigurationType;
                bool output = false;
                if (!string.IsNullOrEmpty(adapter.Name))
                    name = adapter.Name;

                if (configType.Equals(inputConfigType))
                {
                    configuration = adapter.Configuration;
                    output = true;
                }

                if (inputConfigType.IsSubclassOf(configType))
                {
                    configuration = adapter.Configuration;
                    output = true;
                }

                if (configType.IsAssignableFrom(inputConfigType))
                {
                    configuration = adapter.Configuration;
                    output = true;
                }

                return output; 
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual System.Configuration.Configuration GetConfigurationManager()
        {
            lock (ConfigLocker)
            {
                if (_config == null)
                {
                    _config = this.DoGetConfigurationManager();
                    this.LastReadTime = DateTime.Now;
                }
                else
                {
                    DateTime now = DateTime.Now;
                    if (now.Subtract(this.LastReadTime) > this.RefreshTime)
                    {
                        _config = this.DoGetConfigurationManager();
                        this.LastReadTime = now;
                    }
                }
            }

            return _config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual System.Configuration.Configuration DoGetConfigurationManager()
        {
            if (string.IsNullOrEmpty(this.ConfigurationFileName))
            {
                //busco la seccion en el app.config
                _config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            else
            {
                string filename = Path.Combine(Environment.CurrentDirectory, this.ConfigurationFileName);
                filename = Path.GetFullPath(filename);

                if (!File.Exists(filename))
                    throw new ConfigurationErrorsException(this.ConfigurationFileName);

                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = filename;

                _config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            }

            return _config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected virtual object GetConfiguration(Type configType, System.Configuration.Configuration source)
        {
            ConfigurationSection section;
            ConfigurationSectionCollection sections = source.Sections;

            object output;

            for (int i = 0; i < sections.Count; i++)
            {
                section = sections[i];
                output = section;

                if (output != null && output is IConfigurationSectionAdapter)
                    output = ((IConfigurationSectionAdapter)output).Configuration;

                if (output != null && (output.GetType() == configType || output.GetType().IsSubclassOf(configType) || configType.IsAssignableFrom(output.GetType())))
                {
                    //DefaultLogger.Debug("AppConfigConfigurationProvider - Configuration found type '{0}' with name '{1}'. Configuration requested type '{1}'.", output.GetType().FullName, section.SectionInformation.Name, configType.FullName);

                    return output;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected virtual object GetConfiguration(Type configType, string name, System.Configuration.Configuration source)
        {
            object output = null;

            output = source.GetSection(name);

            if (output != null && output is IConfigurationSectionAdapter)
                output = ((IConfigurationSectionAdapter)output).Configuration;

            if (output != null && output.GetType() != configType && !output.GetType().IsSubclassOf(configType) && !configType.IsAssignableFrom(output.GetType()))
                    throw new ConfigurationErrorsException("La sección " + output.GetType().FullName + " no es subclase de la sección solicitada (" + configType.FullName + ") de nombre " + name);

            if (output != null)
            {
                //DefaultLogger.Debug("AppConfigConfigurationProvider - Configuration found type '{0}' with name '{1}'. Configuration requested type '{1}'.", output.GetType().FullName, name, configType.FullName);
            }

            return output;
        }


        #endregion
	}
}

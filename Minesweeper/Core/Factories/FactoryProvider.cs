
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Minesweeper.Core.Configurations;
//using Minesweeper.Core.Log;
using Minesweeper.Core.Helpers;

namespace Minesweeper.Core.Factories
{
    /// <summary>
    /// Main Factory class. Generates Factories for any given entity using the configuration
    /// specified.
    /// FactoryProvider internally stores all Factories created as Singleton.
    /// </summary>
    public class FactoryProvider
    {
        #region -- Static Singleton Instance --

        private static FactoryProvider instance = new FactoryProvider();

        /// <summary>
        /// Gets the singleton accesor to FactoryProvider.
        /// </summary>
        public static FactoryProvider Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion


        private string _fileName = null;
        private Dictionary<string, FactoryConfiguration> _configurations;
        private Dictionary<FactoryConfiguration, List<string>> _factoryNames;
        private Dictionary<string, object> _factories;

        private object objLock = new object();


        #region -- Constructors --
        
        private FactoryProvider()
        {
            _configurations = new Dictionary<string, FactoryConfiguration>();
            _factories = new Dictionary<string, object>();
            _factoryNames = new Dictionary<FactoryConfiguration, List<string>>();

            //creo la instancia de configuration provider
            this.ConfigurationProvider = null; // Minesweeper.Core.Configurations.ConfigurationProvider.Instance;
        }

        internal FactoryProvider(IConfigurationProvider configurationProvider)
        {
            _configurations = new Dictionary<string, FactoryConfiguration>();
            _factories = new Dictionary<string, object>();
            _factoryNames = new Dictionary<FactoryConfiguration, List<string>>();

            this.ConfigurationProvider = configurationProvider;
        }

        #endregion


        #region -- Public Properties --
        private IConfigurationProvider _configurationProvider;

        /// <summary>
        /// Instance of the Configuration Provider.
        /// </summary>
        protected IConfigurationProvider ConfigurationProvider
        {
            get
            {
                if (_configurationProvider == null)
                    _configurationProvider = Minesweeper.Core.Configurations.ConfigurationProvider.Instance;

                return _configurationProvider;
            }
            set
            {
                _configurationProvider = value;
            }
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
                this.ConfigurationProvider = new AppConfigConfigurationProvider(value);
            }
        }

        /// <summary>
        /// Gets the dictionary with all the configurations stored in this factory. Each configuration
        /// is created and stored the first time it's read. Even if different instances of the same
        /// Factory are required, configurations are reused.
        /// </summary>
        public Dictionary<string, FactoryConfiguration> Configurations
        {
            get { return _configurations; }
        }

        /// <summary>
        /// Gets the dictionary containing the list of factories created. If a factory is requested as
        /// Singleton, this dictionary stores such a factory. If a factory of the same type is then requested
        /// to the FactoryProvider, it will return the instance stored in this dictionary.
        /// </summary>
        public Dictionary<string, object> Factories
        {
            get { return _factories; }
        }

        #endregion

        #region -- Creation --

        /// <summary>
        /// Returns an Instance (new or singleton, depending on configuration) of the corresponding Factory that create entities,
        /// of type TEntity. Return the first that it find.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="factory"></param>
        /// <returns>A boolean value and an instance of a Factory, wich may be new or singleton.</returns>
        public bool TryCreateFactory<TEntity>(out IFactory<TEntity> factory)
        {
            //DefaultLogger.Debug("FactoryProvider - Try Create factory of type {0}.", typeof(TEntity).FullName);

            factory = null;

            FactoryConfiguration config = this.GetFactoryConfigurationForEntity(typeof(TEntity));

            if (config != null) //creo la instancia
                factory = (IFactory<TEntity>)DoCreateFactory(config.Name, null, typeof(TEntity));
//            factory = (IFactory<TEntity>) DoCreateFactory(config.Name, config, typeof(TEntity));

            return factory != null;
        }
		
        /// Returns an Instance (new or singleton, depending on configuration) of the corresponding Factory that create entities,
        /// of type TEntity. Return the first that it find.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="factory"></param>
        /// <returns>A boolean value and an instance of a Factory, wich may be new or singleton.</returns>
        public bool TryCreateFactory<TEntity>(string name, out IFactory<TEntity> factory)
        {
            //DefaultLogger.Debug("FactoryProvider - Try Create factory of type {0} and name {1}.", typeof(TEntity).FullName, name);

            if (string.IsNullOrEmpty(name))
                return this.TryCreateFactory<TEntity>(out factory);

            factory = null;

            FactoryConfiguration config = this.GetFactoryConfigurationForEntity(name, typeof(TEntity));

            if (config != null) //creo la instancia
                //factory = (IFactory<TEntity>)DoCreateFactory(config.Name, config, typeof(TEntity));
                factory = (IFactory<TEntity>)DoCreateFactory(config.Name, null, typeof(TEntity));

            return factory != null;
        }


        public IFactory CreateFactory(Type entityType)
        {
            FactoryConfiguration config = this.GetFactoryConfigurationForEntity(entityType);

            if (config == null)
                throw new ConfigurationErrorsException(string.Format("No hay factory configurado para el tipo de entidad '{0}'.", entityType.Name));

            return (IFactory)DoCreateFactory(config.Name, null, entityType);
        }

        public IFactory CreateFactory(Type entityType, string name)
        {
            return (IFactory)DoCreateFactory(name, null, entityType);
        }


        /// <summary>
        /// Returns an Instance (new or singleton, depending on configuration) of the corresponding Factory,
        /// based on the specified configuration.
        /// </summary>
        /// <typeparam name="TEntity">The type of Entity to create with the returned factory.</typeparam>
        /// <param name="name">The name of the configuration section to read, in order to create the appropiate
        /// factory and entity.</param>
        /// <returns>An instance of a Factory, wich may be new or singleton.</returns>
        public IFactory<TEntity> CreateFactory<TEntity>(string name)
        {
            return (IFactory<TEntity>)DoCreateFactory(name, null, typeof(TEntity));
        }

        /// Returns an Instance (new or singleton, depending on configuration) of the corresponding Factory that create entities,
        /// of type TEntity. Return the first that it find.
        /// </summary>
        /// <returns>An instance of a Factory, wich may be new or singleton.</returns>
        public IFactory<TEntity> CreateFactory<TEntity>()
        {
            FactoryConfiguration config = this.GetFactoryConfigurationForEntity(typeof(TEntity));
            
            if (config == null)
                throw new ConfigurationErrorsException(string.Format("No hay factory configurado para el tipo de entidad '{0}'.", typeof(TEntity).Name));

            //creo la instancia
            return (IFactory<TEntity>)DoCreateFactory(config.Name, null, typeof(TEntity));
        }

        /// <summary>
        /// Obtiene el factory de tipo IFactory que crea instancias del tipo TEntity. Devuelve el primer factory que encuentra.
        /// </summary>
        /// <typeparam name="TEntity">Factory type.</typeparam>
        /// <param name="factoryName">Name of the factory</param>
        /// <returns>A new object reference.</returns>
        public TEntity CreateEntity<TEntity>()
        {
            IFactory<TEntity> factory = this.CreateFactory<TEntity>();
            return factory.Create();
        }

        /// <summary>
        /// Obtiene el factory de tipo IFactory e invoca al Create.
        /// </summary>
        /// <typeparam name="TEntity">Factory type.</typeparam>
        /// <param name="factoryName">Name of the factory</param>
        /// <returns>A new object reference.</returns>
        public TEntity CreateEntity<TEntity>(string factoryName)
        {
            if (string.IsNullOrEmpty(factoryName))
            {
                IFactory<TEntity> factory = this.CreateFactory<TEntity>();
                return factory.Create();
            }
            else
            {
                IFactory<TEntity> factory = this.CreateFactory<TEntity>(factoryName);
                return factory.Create();
            }
        }


        /// Obtiene el factory de nombre factoryName e invoca al create por reflection evitando el casteo, ya que no se conoce el tipo de la entidad a crear
        /// </summary>
        /// <param name="factoryName">Factory name.</param>
        /// <returns>New entity</returns>
        public object CreateEntity(string factoryName)
        {
            object factory = DoCreateFactory(factoryName, null, null);
            MethodInfo method = factory.GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
            return method.Invoke(factory, null);
        }


        /// <summary>
        /// Returns if the factory configuration for the Entity exist in the configuration file.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>A boolean value</returns>
        public bool ExistsFactory<TEntity>()
        {
            FactoryConfiguration config = this.GetFactoryConfigurationForEntity(typeof(TEntity));

            return (config != null);
        }

        public bool ExistsFactory<TEntity>(string name)
        {
            FactoryConfiguration config = this.GetFactoryConfigurationForEntity(name, typeof(TEntity));

            return (config != null);
        }

        #endregion

	
        /// <summary>
        /// Gets the factory configuration.
        /// </summary>
        /// <param name="entityType">Entity Type that factory creates.</param>
        /// <returns>Factory configuration.</returns>
        protected FactoryConfiguration GetFactoryConfigurationForEntity(Type entityType)
        {
            List<FactoryConfiguration> factories = this.ConfigurationProvider.GetAllConfigurations<FactoryConfiguration>();
            FactoryConfiguration output = null;

            foreach (FactoryConfiguration factoryConfiguration in factories)
            {
                Type factoryType = factoryConfiguration.FactoryType;

                if (factoryType != null)
                {
                    if (this.CanCreateEntity(factoryConfiguration, entityType))
                    {
                        output = factoryConfiguration;
                        break;
                    }
                }
                else
                {
                    var msg = string.Format("La configuración del factory de nombre {0}, no tiene factoryType definido." + Environment.NewLine + "{1}", factoryConfiguration.Name, factoryConfiguration.Configuration);
                    throw new Exception(msg);

                }
                //else
                //DefaultLogger.Warning("La configuración del factory de nombre {0}, no tiene factoryType definido." + Environment.NewLine + "{1}", factoryConfiguration.Name, factoryConfiguration.Configuration);
            }

            return output;
        }

        /// <summary>
        /// Gets the factory configuration.
        /// </summary>
        /// <param name="entityType">Entity Type that factory creates.</param>
        /// <returns>Factory configuration.</returns>
        protected FactoryConfiguration GetFactoryConfigurationForEntity(string name, Type entityType)
        {
            List<FactoryConfiguration> factories = this.ConfigurationProvider.GetAllConfigurations<FactoryConfiguration>();
            FactoryConfiguration output = null;

            foreach (FactoryConfiguration factoryConfiguration in factories)
            {
                Type factoryType = factoryConfiguration.FactoryType;
                if (factoryType != null)
                {
                    if (this.CanCreateEntity(factoryConfiguration, entityType))
                    {
                        output = factoryConfiguration;
                        break;
                    }
                }
            }
            return output;
        }


        private bool CanCreateEntity(FactoryConfiguration factoryConfig, Type entityType)
        {
            Type factoryType = factoryConfig.FactoryType;

            if (factoryType == null)
                return false;

            Type[] interfaces = factoryType.GetInterfaces();

            foreach (Type t in interfaces)
            {
                if (t.Name.StartsWith("IFactory") && TypeHelper.IsGenericType(t))
                {
                    //verfico si es asignable
                    Type entType = TypeHelper.GetFirstGenericArgumentType(t);
                    if (TypeHelper.CanAssign(entType, entityType))
                        return true;
                }
            }

            return false;
        }

        #region -- Do Create --

        /// <summary>
        /// Creates factory de nombre name. Si ya fue creado un factory se usa esa instancia, sino si config no es nulo se usa config, y si no se busca la configuracion con ese nombre.
        /// </summary>
        /// <param name="name">Factory name.</param>
        /// <returns>Factory instance.</returns>
        private object DoCreateFactory(string name, FactoryConfiguration config, Type entityType)
        {
            object factory = null;

            lock (objLock)
            {
                FactoryConfiguration factoryConfiguration;

                if (!_configurations.ContainsKey(name))
                {
                    if (config == null)
                        factoryConfiguration = this.GetConfiguration(name);
                    else
                        factoryConfiguration = config;

                    if (factoryConfiguration == null)
                        throw new ConfigurationErrorsException(string.Format("El factory de nombre '{0}' no esta definido en la configuración.", name));
                }
                else
                {
                    factoryConfiguration = _configurations[name];
                }

                Type factoryType = factoryConfiguration.FactoryType;

                if (factoryType == null)
                    throw new ConfigurationErrorsException(string.Format("The factory with name '{0}' has Type null or invalid or could not load some of their referencies", name, factoryConfiguration.FactoryType));

                //validate that the entity type can be created with this configuration.
                if (entityType != null)
                {
                    if (!this.CanCreateEntity(factoryConfiguration,  entityType))
                        throw new ConfigurationErrorsException(string.Format("Can not create the entity of type '{0}'. Factory - Name: '{1}' and Type: '{2}', .", entityType, name, factoryConfiguration.FactoryType));
                }

                string uniqueFactoryName = this.GetUniqueFactoryName(name, entityType, factoryConfiguration);

                if (_factories.ContainsKey(uniqueFactoryName))
                {
                    return _factories[uniqueFactoryName];
                }

                try
                {
                    factory = factoryConfiguration.Create();
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException(string.Format("Configuration error. Check factory types. Configuration in use: {0}", factoryConfiguration.Name), ex);
                }

                factory = this.ConvertFactory(factory, entityType);

                _factories[uniqueFactoryName] = factory;

                if (!_factoryNames.ContainsKey(factoryConfiguration))
                    _factoryNames[factoryConfiguration] = new List<string>();

                _factoryNames[factoryConfiguration].Add(uniqueFactoryName);
            }

            return factory;
        }

        private object ConvertFactory(object factory, Type destinyEntityType)
        {
            Type sourceFactoryType = factory.GetType();
            Type[] interfaces = sourceFactoryType.GetInterfaces();

            Type desintyFactoryType = typeof(IFactory<>).MakeGenericType(destinyEntityType);

            foreach (Type t in interfaces)
            {
                if (t.Name.StartsWith("IFactory") && t.IsGenericType)
                {
                    if (t.Equals(desintyFactoryType))
                        return factory;
                }
            }

            Type newFactoryType = typeof(FactoryAdapter<>).MakeGenericType(destinyEntityType);
            IFactory newFactory = (IFactory)Activator.CreateInstance(newFactoryType, factory);

            return newFactory;
        }

        /// <summary>
        /// Gets the factory configuration.
        /// </summary>
        /// <param name="name">Configuration name.</param>
        /// <returns>Factory configuration.</returns>
        protected FactoryConfiguration GetConfiguration(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            lock (objLock)
            {
                FactoryConfiguration factoryConfiguration;
                if (!_configurations.ContainsKey(name))
                {
                    factoryConfiguration = this.ConfigurationProvider.GetConfiguration<FactoryConfiguration>(name);

                    if (factoryConfiguration == null)
                        throw new ConfigurationErrorsException(string.Format("The factory with name '{0}' is not defined in the configuration stored.", name));
                    
                    _configurations.Add(name, factoryConfiguration);
                }
                else
                {
                    factoryConfiguration = _configurations[name];
                }
                return factoryConfiguration;
            }
        }

        #endregion


        #region -- Single Factory Methods --

        private string GetUniqueFactoryName(string name, Type entityType, FactoryConfiguration factoryConfiguration)
        {
            return name + "_" + entityType.FullName + "_" + factoryConfiguration.FactoryType.FullName;
        }

        #endregion


        #region -- Register Factory --

        public void RegisterFactory(FactoryConfiguration factoryConfiguration)
        {
            lock (objLock)
            {
                string name = factoryConfiguration.Name;

                if (string.IsNullOrEmpty(name))
                    throw new ConfigurationErrorsException("The configuration has not name defined.");

                if (_configurations.ContainsKey(name))
                    //remuevo la configuracion
                    _configurations.Remove(name);

                //TODO recorrer todas las clases e interfaces de la entidad que construye el factory y registrarlo
/*
                Type factoryType = factoryConfiguration.FactoryType;

                string uniqueFactoryName = this.GetUniqueFactoryName(name, factoryConfiguration);

                //remuevo el factory si es que existe
                if (_factories.ContainsKey(uniqueFactoryName))
                    _factories.Remove(uniqueFactoryName);

*/
                //agrego la configuracion
                _configurations.Add(name, factoryConfiguration);
            }
        }

        public void UnregisterFactory(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            lock (objLock)
            {
                if (_configurations.ContainsKey(name))
                {
                    FactoryConfiguration factoryConfiguration = _configurations[name];
                    List<string> names = _factoryNames[factoryConfiguration];

                    foreach (string singleName in names)
                    {
                        //string singleName = this.GetUniqueFactoryName(name, factoryConfiguration);
                        _configurations.Remove(singleName);

                        if (_factories.ContainsKey(singleName))
                            _factories.Remove(singleName);
                    }

                    _factoryNames.Remove(factoryConfiguration);
                }
            }
        }

        public void UnregisterFactory(FactoryConfiguration factoryConfiguration)
        {
            this.UnregisterFactory(factoryConfiguration.Name);
        }

        #endregion

        public void Clean()
        {
            this.Factories.Clear();
            this.Configurations.Clear();
        }

        public void ChangeConfigurationProvider(IConfigurationProvider prov)
        {
            this.Clean();
            this.ConfigurationProvider = prov;
        }
    }
}
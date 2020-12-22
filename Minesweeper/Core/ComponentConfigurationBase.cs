using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Configuration;
    using System.Reflection;
    using Minesweeper.Core.Helpers;
    using Minesweeper.Core.Xml;
    using Configurations;

    [Serializable]
    public abstract class ComponentConfigurationBase : IConfiguration
    {
        #region -- Single Instancies --

        protected static object locker = new object();

        private static Dictionary<string, object> instances = new Dictionary<string, object>();

        protected object GetSingleInstance()
        {
            if (this.ComponentSingleInstance)
            {
                lock (locker)
                {
                    var instanceKey = string.Concat(this.ComponentType.FullName, ":", this.ComponentName);
                    if (!instances.ContainsKey(instanceKey))
                        return null;
                    return instances[instanceKey];
                }
            }
            else
                return null;
        }

        protected void AddSingleInstance(object instance)
        {
            if (this.ComponentSingleInstance)
            {
                lock (locker)
                {
                    var instanceKey = string.Concat(this.ComponentType.FullName, ":", this.ComponentName);
                    if (!instances.ContainsKey(instanceKey))
                        instances.Add(instanceKey, instance);
                }
            }
        }

        #endregion

        /// <summary>
        /// XML configuration.
        /// </summary>
        private Type _configurationType;

        /// <summary>
        /// Instance configuration.
        /// </summary>
        private object _configuration;


        #region -- Constructores --

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        public ComponentConfigurationBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="type">Instance type.</param>
        public ComponentConfigurationBase(Type type)
        {
            this.ComponentType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="config">Configuration data.</param>
        public ComponentConfigurationBase(object config)
        {
            this.SetConfiguration(config);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="type">instance type.</param>
        /// <param name="config">Configuration data.</param>
        public ComponentConfigurationBase(Type type, object config)
        {
            this.ComponentType = type;
            this.SetConfiguration(config);
        }

        #endregion


        protected string ComponentName { get; set; }

        string IConfiguration.Name
        {
            get
            {
                return this.ComponentName;
            }
        }


        /// <summary>
        /// Gets or sets the instance type.
        /// </summary>
        protected virtual Type ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the isSingleton property.
        /// </summary>
        protected bool ComponentSingleInstance { get; set; }

        /// <summary>
        /// Gets or sets the XML Configuration.
        /// </summary>
        protected string ComponentConfiguration
        {
            get;
            set;
        }


        #region -- Create --

        protected object CreateComponent()
        {
            object output = this.GetSingleInstance();

            if (output == null)
            {
                output = this.DoCreateComponent();

                this.DoConfigureComponent(output);

                this.AddSingleInstance(output);
            }

            return output;
        }

        protected virtual TComponent CreateComponent<TComponent>()
        {
            object output = this.CreateComponent();
            return (TComponent)output;
        }

        protected virtual object DoCreateComponent()
        {
            if (this.ComponentType == null)
                throw new ConfigurationErrorsException("There is not Type configured. The Type property can not be null.");

            return Activator.CreateInstance(this.ComponentType, true);
        }

        protected virtual void DoConfigureComponent(object instance)
        {
            if (instance is IConfigurable)
            {
                IConfigurable configurable = instance as IConfigurable;
                configurable.Configure(this);
            }
        }

        #endregion


        #region -- Set / Get Configuration --

        /// <summary>
        /// Asigna la configuración de la instancia a crear.
        /// </summary>
        /// <param name="config">Instancia de la configuracion. Tiene que ser serializable en xml</param>
        public virtual void SetConfiguration(object config)
        {
            this.SetConfiguration(config, null);
        }

        /// <summary>
        /// Asigna la configuración de la instancia a crear. 
        /// </summary>
        /// <param name="config">Instancia de la configuracion. Tiene que ser serializable en xml</param>
        /// <param name="rootName">Nombre a usar para el root de xml al serializar la configuración</param>
        public virtual void SetConfiguration(object config, string rootName)
        {
            XmlSerializerSettings set = new XmlSerializerSettings();
            set.UniqueSerializationForInstance = false;
            set.WriteRootTypeDefinition = false;
            if (string.IsNullOrEmpty(rootName))
            {
                this.ComponentConfiguration = XmlHelper.Serialize(config, null, set);
            }
            else
            {
                this.ComponentConfiguration = XmlHelper.Serialize(config, rootName, set);
                _configuration = config;
            }

            if (config != null)
                _configurationType = config.GetType();
        }

        public virtual void SetConfiguration(Type configType, string xmlConfig)
        {
            this._configurationType = configType;
            this.ComponentConfiguration = xmlConfig;
        }

        public virtual object GetConfiguration()
        {
            return this.ComponentConfiguration;
        }

        /// <summary>
        /// Obtiene la configuración para la instancia del elemento.
        /// </summary>
        /// <param name="typeOf">Tipo de la configuración</param>
        /// <returns>The configuration.</returns>
        public virtual object GetConfiguration(Type typeOf)
        {
            if (string.IsNullOrEmpty(this.ComponentConfiguration))
                return null;

            if (typeOf == null)
                return this.ComponentConfiguration;

            if (_configuration == null)
                _configuration = this.DeserializeConfiguration(typeOf);
            else
            {
                if (!typeOf.IsAssignableFrom(_configuration.GetType()))
                {
                    object value = this.DeserializeConfiguration(typeOf);
                    _configuration = value;
                }
            }

/*
            if (_configuration == null || typeOf.IsAssignableFrom(_configuration.GetType()))
            {
                _configuration = this.DeserializeConfiguration(typeOf);
                string elementName = XmlHelper.GetElementName(this.ComponentConfiguration);
                string config = this.ComponentConfiguration;

                if (string.IsNullOrEmpty(config))
                {
                    _configuration = null;
                }
                else
                {
                    try
                    {
                        _configuration = XmlHelper.Deserialize(typeOf, elementName, config);
                    }
                    catch (Exception ex)
                    {
                        throw this.CreateCanNotCreateDeserializeConfigurationException(ex);
                    }
                }
            }
*/

            return _configuration;
        }

        private object DeserializeConfiguration(Type entityType)
        {
            string elementName = XmlHelper.GetElementName(this.ComponentConfiguration);
            string config = this.ComponentConfiguration;

            if (string.IsNullOrEmpty(config))
            {
                return null;
            }
            else
            {
                try
                {
                    return XmlHelper.Deserialize(entityType, elementName, config);
                }
                catch (Exception ex)
                {
                    throw this.CreateCanNotCreateDeserializeConfigurationException(ex);
                }
            }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">Configuration type.</typeparam>
        /// <returns>Resulting configuration.</returns>
        public virtual TConfiguration GetConfiguration<TConfiguration>()
        {
            object output = this.GetConfiguration(typeof(TConfiguration));
            if (output == null)
                return default(TConfiguration);

            return (TConfiguration)output;
        }

        #endregion


        /// <summary>
        /// CreateCanNotCreateDeserializeConfigurationException exception.
        /// </summary>
        /// <param name="inner">Inner exception.</param>
        /// <returns>A new exception.</returns>
        private Exception CreateCanNotCreateDeserializeConfigurationException(Exception inner)
        {
            ConfigurationException ex;

            string msg = "No es posible deserializar la configuración.";

            if (inner == null)
            {
                ex = new ConfigurationErrorsException(msg);
            }
            else
            {
                ex = new ConfigurationErrorsException(msg, inner);
            }

            ex.Data.Add("TypeName", this.ComponentType.FullName);
            ex.Data.Add("Configuration", this.ComponentConfiguration);

            return ex;
        }
    }
}

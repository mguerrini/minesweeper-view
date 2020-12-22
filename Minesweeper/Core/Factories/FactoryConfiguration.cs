using Minesweeper.Core.Configurations;
using Minesweeper.Core.Xml.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Minesweeper.Core.Factories
{
    [Serializable]
    public class FactoryConfiguration : ComponentConfigurationBase, IConfigurationBase
    {
        #region -- Constructores --

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        public FactoryConfiguration() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="type">Instance type.</param>
        public FactoryConfiguration(Type type) : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="config">Configuration data.</param>
        public FactoryConfiguration(object config) : base(config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="type">instance type.</param>
        /// <param name="config">Configuration data.</param>
        public FactoryConfiguration(Type type, object config) : base(type, config)
        {
        }

        #endregion

        #region -- Properties --

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return this.ComponentName;
            }
            set
            {
                this.ComponentName = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the Factory to create.        
        /// </summary>
        [XmlAttribute("factoryType")]
        public virtual Type FactoryType
        {
            get
            {
                return this.ComponentType;
            }
            set
            {
                this.ComponentType = value;
            }
        }

        /// <summary>
        /// Gets or sets the SingleInstance that indicate the factory create only one instance.
        /// </summary>
        [XmlAttribute("singleInstance")]
        public bool SingleInstance
        {
            get
            {
                return this.ComponentSingleInstance;
            }
            set
            {
                this.ComponentSingleInstance = value;
            }
        }

        /// <summary>
        /// Gets or sets the XML Configuration.
        /// </summary>
        [XmlContent]
        [XmlElement("Configuration")]
        public string Configuration
        {
            get
            {
                return this.ComponentConfiguration;
            }
            set
            {
                this.ComponentConfiguration = value;
            }
        }

        #endregion

        #region -- Create --

        public virtual object Create()
        {
            return this.CreateComponent();
        }

        public virtual TComponent Create<TComponent>()
        {
            return this.CreateComponent<TComponent>();
        }

        #endregion
    }
}

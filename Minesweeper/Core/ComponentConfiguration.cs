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
    using Configurations;

    [Serializable]
    public class ComponentConfiguration : ComponentConfigurationBase, IConfigurationBase
    {
        #region -- Constructores --

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        public ComponentConfiguration() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="type">Instance type.</param>
        public ComponentConfiguration(Type type) : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="config">Configuration data.</param>
        public ComponentConfiguration(object config) : base(config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceBuilder" /> class.
        /// </summary>
        /// <param name="type">instance type.</param>
        /// <param name="config">Configuration data.</param>
        public ComponentConfiguration(Type type, object config) : base(type, config)
        {
        }

        #endregion


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
        /// Gets or sets the instance type.
        /// </summary>
        [XmlAttribute("type")]
        public virtual Type Type
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
        /// Gets or sets the isSingleton property.
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


        #region -- Create --

        public object Create()
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

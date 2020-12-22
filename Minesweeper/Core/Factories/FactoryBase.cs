
namespace Minesweeper.Core.Factories
{
    using System;
    using System.Configuration;
    using System.Xml;
    using Minesweeper.Core.Helpers;

    /// <summary>
    /// FactoryBase class.
    /// </summary>
    public abstract class FactoryBase : IConfigurable
    {
        #region -- Constructors --

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryBase"/> class.
        /// </summary>
        protected FactoryBase()
        {
            this.IsSingleInstance = true;
            //this.Configure(new FactoryConfiguration(null));
        }

        protected FactoryBase(object configuration)
        {
            this.IsSingleInstance = true;
            this.Configuration = configuration;
            this.Configure(new FactoryConfiguration(configuration));
        }

        protected FactoryBase(FactoryConfiguration configuration)
        {
            this.IsSingleInstance = configuration.SingleInstance;
            this.Configure(configuration);
        }

        #endregion


        protected bool IsSingleInstance { get; set; }


        #region -- Configure --

        void IConfigurable.Configure(IConfiguration configData)
        {
            if (configData == null)
            {
                this.IsSingleInstance = true;
                this.Configure(new FactoryConfiguration());
                return;
            }
            if (configData is FactoryConfiguration)
            {
                FactoryConfiguration c = configData as FactoryConfiguration;
                this.IsSingleInstance = c.SingleInstance;
                this.Configure(configData as FactoryConfiguration);
            }
            else
            {
                FactoryConfiguration f = new FactoryConfiguration(this.GetType());
                f.Name = configData.Name;
                object customConfig = configData.GetConfiguration();
                if (customConfig != null)
                    f.Configuration = customConfig.ToString();
                if (configData is ComponentConfiguration)
                    f.SingleInstance = (configData as ComponentConfiguration).SingleInstance;
                else
                    f.SingleInstance = true;

                FactoryConfiguration c = new FactoryConfiguration(configData);
                this.IsSingleInstance = c.SingleInstance;
                this.Configure(c);
            }
        }

        protected virtual void Configure(FactoryConfiguration configData)
        {

        }

        #endregion


        /// <summary>
        /// Gets or sets the configuration section corresponding to the entity to instantiatem, if any.
        /// </summary>
        protected object Configuration { get; set; }

        protected abstract object DoCreate();
    }
}

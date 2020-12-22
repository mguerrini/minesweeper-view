namespace Minesweeper.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;
    using System.Xml;
    using Minesweeper.Core.Helpers;


    [Serializable]
    public class ConfigurationSectionAdapter<TConfiguration> : ConfigurationSection, IConfigurationSectionAdapter
    {
       private TConfiguration _configuration = default(TConfiguration);

        #region -- Constructores --

       public ConfigurationSectionAdapter()
        {
            this.NeedUpdate = false;
            this.NeedDeserializeConfiguration = false;
        }

        #endregion

        protected string Name
        {
            get { return this.SectionInformation.Name; }
        }

        protected Type ConfigurationType
        {
            get { return typeof(TConfiguration); }
        }

        public virtual TConfiguration Configuration
        {
            get
            {
                if (this.NeedDeserializeConfiguration)
                {
                    if (string.IsNullOrEmpty(this.ConfigurationXml))
                        _configuration = default(TConfiguration);
                    else
                    {
                        _configuration = this.DeserializeConfiguration(this.ConfigurationElementName, this.ConfigurationXml);
                    }
                }

                if (_configuration != null)
                {
                    if (_configuration is IConfigurationBase)
                    {
                        IConfigurationBase named = _configuration as IConfigurationBase;
                        if (string.IsNullOrEmpty(named.Name))
                            named.Name = this.SectionInformation.Name;
                    }
                }

                return _configuration;
            }
            set
            {
                _configuration = value;
                this.NeedDeserializeConfiguration = false;
                this.NeedUpdate = true;
            }
        }

        protected bool NeedDeserializeConfiguration { get; set; }

        protected string ConfigurationXml { get; set; }

        protected string ConfigurationElementName { get; set; }


        private bool NeedUpdate
        {
            get;
            set;
        }

        protected override bool IsModified()
        {
            return this.NeedUpdate;
        }

        protected override void DeserializeSection(XmlReader reader)
        {
            reader.MoveToContent();
            this.ConfigurationElementName = reader.Name;
            this.ConfigurationXml = reader.ReadOuterXml();

            if (this.ConfigurationXml != null)
                this.ConfigurationXml = this.ConfigurationXml.Trim();

            this.NeedDeserializeConfiguration = true;
        }

        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            if (this.Configuration != null)
            {
                object nameValue = TypeHelper.GetPropertyValue(this.Configuration, "Name");

                if (nameValue != null)
                {
                    if (string.Compare(nameValue.ToString(), this.SectionInformation.Name) == 0)
                        TypeHelper.SetPropertyValue(this.Configuration, "Name", null);
                }
/*
                if (this.Configuration is IEditableNamed)
                {
                    IEditableNamed named = this.Configuration as IEditableNamed;

                    if (string.Compare(named.Name, this.SectionInformation.Name) == 0)
                        named.Name = null;
                }
*/
            }

            string xml = XmlHelper.Serialize(this.Configuration, name);
            return xml;
        }


        protected virtual TConfiguration DeserializeConfiguration(string name, string configXml)
        {
            TConfiguration configuration = XmlHelper.Deserialize<TConfiguration>(this.ConfigurationElementName, this.ConfigurationXml);

            if (configuration != null)
                TypeHelper.SetPropertyValue(configuration, "Name", this.SectionInformation.Name);

            return configuration;
        }

        #region -- IConfigurationAdapter --

        string IConfigurationSectionAdapter.Name
        {
            get { return this.Name; }
        }

        Type IConfigurationSectionAdapter.ConfigurationType
        {
            get { return this.ConfigurationType; }
        }

        object IConfigurationSectionAdapter.Configuration
        {
            get { return this.Configuration; }
        }

        #endregion
    }
}

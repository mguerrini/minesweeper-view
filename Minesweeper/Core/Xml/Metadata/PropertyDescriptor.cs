namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using Minesweeper.Core.Xml.Attributes;

    public abstract class PropertyDescriptor
    {
        #region -- Constructors --

        protected PropertyDescriptor(PropertyInfo info)
        {
            this.Metadata = new PropertyMetadata(info);
            this.UseAlias = false;
        }

        protected PropertyDescriptor(PropertyMetadata data)
        {
            this.Metadata = data;
            this.UseAlias = false;
        }

        #endregion

        public virtual PropertyMetadata Metadata {get; set;}

        public bool UseAlias { get; set; }

        #region -- From Xml --

        /// <summary>
        /// Devuelve el tipo de la propiedad serializado.
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Type GetTypeFromAttributeName(string attributeName, XmlSerializerContext context);

        public abstract Type GetTypeFromElementName(string elementName, XmlSerializerContext context);

        #endregion


        #region -- ToXml --

        public abstract string GetAttributeNameForType(Type entityType, XmlSerializerContext context);

        public abstract string GetElementNameForType(Type entityType, XmlSerializerContext context, bool isNullReturnDefault);

        /// <summary>
        /// Devuelve un booleano que indica si debe escribir el tipo 
        /// </summary>
        /// <param name="propertyEntityType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool MustDeclareTypeNameInXmlElement(Type entityType, XmlSerializerContext context)
        {
            bool isRoot = context.IsRootElement(this);
 
            if (isRoot)
                return context.MustDeclareRootTypeName(entityType);
            else
                return this.DoMustDeclareTypeNameInXmlElement(entityType, context);
        }

        protected abstract bool DoMustDeclareTypeNameInXmlElement(Type entityType, XmlSerializerContext context);

        #endregion


        #region -- Cast --

        public virtual TPropertyDescriptor GetPropertyDescriptor<TPropertyDescriptor>(Type entityType, XmlSerializerContext context) where TPropertyDescriptor : PropertyDescriptor
        {
            if (typeof(TPropertyDescriptor) == this.GetType())
                return this as TPropertyDescriptor;
            else
                return Activator.CreateInstance(typeof(TPropertyDescriptor), this.Metadata) as TPropertyDescriptor;
        }

        #endregion

        public virtual bool IsXmlAttribute(Type entityType, XmlSerializerContext context)
        {
            return !string.IsNullOrEmpty(this.GetAttributeNameForType(entityType, context));
        }

        public virtual bool IsXmlElement(Type entityType, XmlSerializerContext context)
        {
            return !this.IsXmlAttribute(entityType, context) || !string.IsNullOrEmpty(this.GetElementNameForType(entityType, context, false));
        }

        public override string ToString()
        {
            return "Descriptor Type: "+ this.GetType().Name + ", Property: " + this.Metadata.PropertyName + ", Type: " + this.Metadata.PropertyType;
        }
    }
}

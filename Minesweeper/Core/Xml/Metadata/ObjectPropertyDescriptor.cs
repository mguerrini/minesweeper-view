namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Minesweeper.Core.Xml.Attributes;
    using System.Xml.Serialization;
    using System.Reflection;
    using System.Xml;

    public class ObjectPropertyDescriptor : PropertyDescriptor
    {
        #region -- Constructors --

        public ObjectPropertyDescriptor(PropertyInfo info)
            : base(info)
        {
        }

        public ObjectPropertyDescriptor(PropertyMetadata data)
            : base(data)
        {
        }

        #endregion


        #region -- ToXml --

        protected override bool DoMustDeclareTypeNameInXmlElement(Type entityType, XmlSerializerContext context)
        {
            bool output = false;
            if (!this.Metadata.TypeToElementMap.ContainsKey(entityType))
                output = entityType != this.Metadata.PropertyType;

            return output;
        }

        public override string GetAttributeNameForType(Type entityType, XmlSerializerContext context)
        {
            if (this.Metadata.TypeToAttributeMap.ContainsKey(entityType))
                return this.Metadata.TypeToAttributeMap[entityType];

            if (!string.IsNullOrEmpty(this.Metadata.DefaultAttributeName) && entityType == this.Metadata.PropertyType)
                return this.Metadata.DefaultAttributeName;
            else
                return null;
        }

        public override string GetElementNameForType(Type entityType, XmlSerializerContext context, bool isNullReturnDefault)
        {
            if (this.Metadata.TypeToElementMap.ContainsKey(entityType))
                return this.Metadata.TypeToElementMap[entityType];

            if (!string.IsNullOrEmpty(this.Metadata.DefaultElementName))
                return this.Metadata.DefaultElementName;

            if (isNullReturnDefault)
                return this.Metadata.PropertyName;
            else
                return null;
        }

        #endregion


        #region -- From Xml --

        public override Type GetTypeFromAttributeName(string attributeName, XmlSerializerContext context)
        {
            if (this.Metadata.AttributeToTypeMap.ContainsKey(attributeName))
                return this.Metadata.AttributeToTypeMap[attributeName];

            if (string.Compare(this.Metadata.DefaultAttributeName, attributeName) == 0)
                return this.Metadata.PropertyType;

            return null;
        }

        public override Type GetTypeFromElementName(string elementName, XmlSerializerContext context)
        {
            if (this.Metadata.ElementToTypeMap.ContainsKey(elementName))
                return this.Metadata.ElementToTypeMap[elementName];

            if (string.Compare(this.Metadata.DefaultElementName, elementName) == 0)
                return this.Metadata.PropertyType;

            if (string.Compare(elementName, this.Metadata.PropertyName) == 0)
                return this.Metadata.PropertyType;
            else
                return null;
        }

        #endregion
    }
}

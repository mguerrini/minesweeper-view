namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DictionaryKeyValuePropertyDescriptor : PropertyDescriptor
    {
        #region -- Constructors --

        public DictionaryKeyValuePropertyDescriptor(PropertyMetadata metadata) : base(metadata)
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



        public override string GetElementNameForType(Type entityType, XmlSerializerContext context, bool isNullReturnDefault)
        {
            //si hay un elemento definido para el tipo lo uso.
            if (this.Metadata.TypeToElementMap.ContainsKey(entityType))
                return this.Metadata.TypeToElementMap[entityType];

            //si tiene definido un elemento por defect lo uso
            if (!string.IsNullOrEmpty(this.Metadata.DefaultElementName))
                return this.Metadata.DefaultElementName;

            //obtengo el alias
            if (isNullReturnDefault)
                return this.Metadata.PropertyName;
            else
                return null;
        }

        public override string GetAttributeNameForType(Type entityType, XmlSerializerContext context)
        {
            
            //si hay un elemento definido para el tipo lo uso.
            if (this.Metadata.TypeToAttributeMap.ContainsKey(entityType))
                return this.Metadata.TypeToAttributeMap[entityType];

            //si tiene definido un elemento por defect lo uso, si tiene por default y el tipo es el mismo que el definido
            if (!string.IsNullOrEmpty(this.Metadata.DefaultAttributeName) && entityType == this.Metadata.PropertyType)
                return this.Metadata.DefaultAttributeName;

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

        public override TPropertyDescriptor GetPropertyDescriptor<TPropertyDescriptor>(Type entityType, XmlSerializerContext context) 
        {
            if (typeof(TPropertyDescriptor) == this.GetType())
                return this as TPropertyDescriptor;
            else
            {
                TPropertyDescriptor output = Activator.CreateInstance(typeof(TPropertyDescriptor), this.Metadata) as TPropertyDescriptor;
                //output.UseAlias = true;
                return output;
            }
        }


        public override bool IsXmlAttribute(Type entityType, XmlSerializerContext context)
        {
            return !string.IsNullOrEmpty(this.GetAttributeNameForType(entityType, context));
        }

        public override bool IsXmlElement(Type entityType, XmlSerializerContext context)
        {
            return !this.IsXmlAttribute(entityType, context) || !string.IsNullOrEmpty(this.GetElementNameForType(entityType, context, false));
        }
    }
}

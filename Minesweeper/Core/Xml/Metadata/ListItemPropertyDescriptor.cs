namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;

    public class ListItemPropertyDescriptor : PropertyDescriptor
    {
        #region -- Constructors --

        public ListItemPropertyDescriptor(PropertyMetadata metadata)
            : base(metadata)
        {
        }

        #endregion


        #region -- ToXml --

        protected override bool DoMustDeclareTypeNameInXmlElement(Type entityType, XmlSerializerContext context)
        {
            throw new NotImplementedException();
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

            //obtengo el alias
            if (this.UseAlias)
            {
                string alias = context.GetAlias(entityType);
                if (!string.IsNullOrEmpty(alias))
                    return alias;
            }

            //tengo que buscar los elementos registrados....
            if (!string.IsNullOrEmpty(this.Metadata.DefaultElementName))
                return this.Metadata.DefaultElementName;

            //devuelvo el root del elemento
            TypeDescriptor itemTypeDesc = context.GetTypeDescriptor(entityType);
            return itemTypeDesc.GetRootName();
        }

        public override bool IsXmlAttribute(Type entityType, XmlSerializerContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsXmlElement(Type entityType, XmlSerializerContext context)
        {
            throw new NotImplementedException();
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

            //tengo que buscar los elementos registrados....
            if (!string.IsNullOrEmpty(this.Metadata.DefaultElementName) && string.Compare(this.Metadata.DefaultElementName, elementName) == 0)
                return this.Metadata.PropertyType;

            //obtengo el alias
            Type type = context.GetTypeFromAlias(elementName, this.Metadata.PropertyType);

            return type;
        }

        #endregion


        public override TPropertyDescriptor GetPropertyDescriptor<TPropertyDescriptor>(Type entityType, XmlSerializerContext context) 
        {
            if (typeof(TPropertyDescriptor) == this.GetType())
                return this as TPropertyDescriptor;
            else
            {
                TPropertyDescriptor output = Activator.CreateInstance(typeof(TPropertyDescriptor), this.Metadata) as TPropertyDescriptor;
                output.UseAlias = true;
                return output;
            }
        }
    }
}

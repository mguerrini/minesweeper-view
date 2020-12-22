namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Minesweeper.Core.Xml.Attributes;
    using System.Xml.Serialization;
    using System.Reflection;


    public class ValueTypePropertyDescriptor : ObjectPropertyDescriptor
    {
        #region -- Constructors --

        public ValueTypePropertyDescriptor(PropertyInfo info)
            : base(info)
        {
        }

        public ValueTypePropertyDescriptor(PropertyMetadata data)
            : base(data)
        {
        }

        #endregion


        public bool IsXmlContentText
        {
            get
            {
                return this.Metadata.Attributes.Find(p => p is XmlContentAttribute) != null;
            }
        }

        public string DateTimeFormat
        {
            get
            {
                XmlDateTimeFormatAttribute date = (XmlDateTimeFormatAttribute) this.Metadata.Attributes.FirstOrDefault(p => p is XmlDateTimeFormatAttribute);

                if (date != null)
                    return date.Format;
                else
                    return null;
            }
        }

        public string TimeSpanFormat
        {
            get
            {
                XmlTimeSpanFormatAttribute date = (XmlTimeSpanFormatAttribute)this.Metadata.Attributes.FirstOrDefault(p => p is XmlTimeSpanFormatAttribute);

                if (date != null)
                    return date.Format;
                else
                    return null;
            }
        }

        public string NumberFormat
        {
            get
            {
                XmlNumericFormatAttribute date = (XmlNumericFormatAttribute)this.Metadata.Attributes.FirstOrDefault(p => p is XmlNumericFormatAttribute);

                if (date != null)
                    return date.Format;
                else
                    return null;
            }
        }


        #region -- ToXml --

        protected override bool DoMustDeclareTypeNameInXmlElement(Type entityType, XmlSerializerContext context)
        {
            bool output = false;
            if (!this.Metadata.TypeToElementMap.ContainsKey(entityType) && !this.Metadata.TypeToAttributeMap.ContainsKey(entityType))
                output = entityType != this.Metadata.PropertyType;

            return output;
        }

        public override string GetAttributeNameForType(Type entityType, XmlSerializerContext context)
        {
            if (this.Metadata.TypeToAttributeMap.ContainsKey(entityType))
                return this.Metadata.TypeToAttributeMap[entityType];

            if (!string.IsNullOrEmpty(this.Metadata.DefaultAttributeName))
                return this.Metadata.DefaultAttributeName;
            else
                return null;
        }

        #endregion


        #region -- FromXml --

        public override Type GetTypeFromAttributeName(string attributeName, XmlSerializerContext context)
        {
            if (this.Metadata.AttributeToTypeMap.ContainsKey(attributeName))
                return this.Metadata.AttributeToTypeMap[attributeName];

            if (string.Compare(this.Metadata.DefaultAttributeName, attributeName) == 0)
                return this.Metadata.PropertyType;

            return null;
        }

        #endregion
    }
}

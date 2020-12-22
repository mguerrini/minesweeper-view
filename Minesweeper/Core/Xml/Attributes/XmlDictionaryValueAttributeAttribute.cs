namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class XmlDictionaryValueAttributeAttribute : XmlAttributeAttribute
    {
        public XmlDictionaryValueAttributeAttribute(string attributeName)
            : base(attributeName)
        {
        }
        public XmlDictionaryValueAttributeAttribute(string attributeName, Type elementType)
            : base(attributeName, elementType)
        {
        }
    }
}

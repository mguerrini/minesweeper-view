namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class XmlDictionaryKeyAttributeAttribute : XmlAttributeAttribute
    {
        public XmlDictionaryKeyAttributeAttribute(string attributeName)
            : base(attributeName)
        {
        }
        public XmlDictionaryKeyAttributeAttribute(string attributeName, Type elementType)
            : base(attributeName, elementType)
        {
        }
    }
}

namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class XmlDictionaryValueElementAttribute : XmlElementAttribute
    {
        public XmlDictionaryValueElementAttribute()
            : base()
        {
        }

        public XmlDictionaryValueElementAttribute(string elementName)
            : base(elementName)
        {
        }
        public XmlDictionaryValueElementAttribute(string elementName, Type elementType)
            : base(elementName, elementType)
        {
        }
    }
}

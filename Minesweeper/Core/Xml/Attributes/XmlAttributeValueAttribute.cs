namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class XmlAttributeValueAttribute : XmlAttributeAttribute
    {
        public XmlAttributeValueAttribute(string attName) : base(attName)
        {
        }

        public XmlAttributeValueAttribute(string attName, Type type) : base (attName, type)
        {
        }
    }
}

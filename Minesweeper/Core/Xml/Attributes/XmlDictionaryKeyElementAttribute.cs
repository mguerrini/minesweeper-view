namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class XmlDictionaryKeyElementAttribute : XmlElementAttribute
    {
        public XmlDictionaryKeyElementAttribute()
            : base()
        {
        }

        public XmlDictionaryKeyElementAttribute(string elemName)
            : base(elemName)
        {
        }
        public XmlDictionaryKeyElementAttribute(string elemName, Type elementType)
            : base(elemName, elementType)
        {
        }
    }
}

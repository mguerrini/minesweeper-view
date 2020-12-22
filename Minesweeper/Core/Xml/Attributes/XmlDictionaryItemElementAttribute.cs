namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class XmlDictionaryItemElementAttribute : Attribute
    {
        public XmlDictionaryItemElementAttribute()
        {
        }

        public XmlDictionaryItemElementAttribute(string elementName)
        {
            this.ElementName = elementName;
        }


        public string ElementName { get; set; }
    }
}

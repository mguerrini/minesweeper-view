namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;


    public class XmlArrayItemValueAttribute : XmlArrayItemAttribute
    {
        public XmlArrayItemValueAttribute(string elementName, string attributeName) : base(elementName)
        {
            this.AttributeName = attributeName;
        }

        public XmlArrayItemValueAttribute(string elementName, string attributeName, Type type) : base(elementName, type)
        {
            this.AttributeName = attributeName;
        }


        public string AttributeName { get; set; }
    }
}

namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class XmlElementValueAttribute : XmlElementAttribute
    {
        public string AttributeName { get; set; }
    }
}

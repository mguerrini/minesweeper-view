namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlDateTimeFormatAttribute : Attribute
    {
        public XmlDateTimeFormatAttribute(string format)
        {
            this.Format = format;
        }

        public string Format { get; set; }
    }
}

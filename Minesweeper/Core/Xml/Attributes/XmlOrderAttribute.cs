namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlOrderAttribute : Attribute
    {
        public XmlOrderAttribute(float order)
        {
            this.Order = order;
        }

        public float Order { get; set; }
    }
}

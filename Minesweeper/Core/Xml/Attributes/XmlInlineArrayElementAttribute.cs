namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Minesweeper.Core.Xml.Metadata;
    using Minesweeper.Core.Helpers;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class XmlInlineArrayElementAttribute : XmlElementAttribute
    {
        public XmlInlineArrayElementAttribute(string itemElementName, Type type, string itemSeparator) : base(itemElementName, type)
        {
            this.ItemSeparator = itemSeparator;
            
            if (type == null)
                throw new Exception("El tipo de la lista es nulo.");

            Type elementType = type.GetElementType();

            if (elementType == null)
                throw new Exception("El tipo no es una lista o es un array sin tipo.");

            if (!TypeHelper.IsValueType(type.GetElementType()))
                throw new Exception("El tipo del item de la lista tiene que ser de tipo ValueType para poder serializarse inline.");
        }


        public string ItemSeparator
        {
            get;
            set;
        }
    }
}

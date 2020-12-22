namespace Minesweeper.Core.Xml.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Minesweeper.Core.Xml.Metadata;
    using System.Collections;
    using Minesweeper.Core.Helpers;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class XmlInlineArrayAttributeAttribute : XmlAttributeAttribute
    {
        public XmlInlineArrayAttributeAttribute(string attributeName, string itemSeparator)
            : base(attributeName)
        {
            this.ItemSeparator = itemSeparator;
        }

        public XmlInlineArrayAttributeAttribute(string attributeName, Type arrayType, string itemSeparator)
            : base(attributeName, arrayType)
        {
            this.ItemSeparator = itemSeparator;

            if (arrayType == null)
                throw new Exception("El parametro type no puede ser nulo.");

            if (!arrayType.IsArray && !arrayType.IsAssignableFrom(typeof(IList)))
                throw new Exception("El tipo de la entidad tiene que ser un array o un IList<T>.");
            
            Type t = null;

            if (arrayType.IsGenericType)
            {
                Type[] genercisTypes = arrayType.GetGenericArguments();
                t = genercisTypes[0];
            }
            else
            {
                if (arrayType.IsArray)
                    t = arrayType.GetElementType();
            }

            if (t == null)
                throw new Exception("No es posible inferir el tipo de los items de la lista a partir del tipo " + arrayType.Name);

            if (!TypeHelper.IsValueType(t))
                throw new Exception("El tipo del item de la lista tiene que ser de tipo ValueType para poder serializarse como un atributo xml.");
        }

        public string ItemSeparator
        {
            get;
            set;
        }
    }
}

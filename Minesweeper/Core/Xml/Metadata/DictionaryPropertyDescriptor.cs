namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Collections;

    public class DictionaryPropertyDescriptor : ObjectPropertyDescriptor
    {
        private Type _keyType;
        private Type _valueType;

        #region -- Constructors --

        public DictionaryPropertyDescriptor(PropertyInfo info)
            : base(info)
        {
        }

        public DictionaryPropertyDescriptor(PropertyMetadata data)
            : base(data)
        {
        }

        #endregion


        public DictionaryKeyValuePropertyDescriptor GetDictionaryItemKeyPropertyDescriptor(XmlSerializerContext context)
        {
/*
            ListItemPropertyMetadata metadata;
            string alias = context.GetAlias(this.DeclaringItemType);
            if (string.IsNullOrEmpty(alias))
                metadata = new ListItemPropertyMetadata("Item", this.DeclaringItemType, isInline, this.Metadata);
            else
                metadata = new ListItemPropertyMetadata(alias, this.DeclaringItemType, isInline, this.Metadata);

            ListItemPropertyDescriptor output = new ListItemPropertyDescriptor(metadata);
            output.UseAlias = true;
            return output;
*/
            //el Key no tiene alias.....si no se asigna un nombre de elemento se usa Key como nombre
            DictionaryItemKeyPropertyMetadata metadata;

            //string alias = context.GetAlias(this.KeyType);
            //if (string.IsNullOrEmpty(alias))
                metadata = new DictionaryItemKeyPropertyMetadata("Key", this.KeyType, this.Metadata);
            //else
            //    metadata = new DictionaryItemKeyPropertyMetadata(alias, this.KeyType, this.Metadata);

            return new DictionaryKeyValuePropertyDescriptor(metadata);
        }

        public DictionaryKeyValuePropertyDescriptor GetDictionaryItemValuePropertyDescriptor(XmlSerializerContext context)
        {
            //al value se le puede asignar un nombre de elemento por alias.
            DictionaryItemValuePropertyMetadata metadata;
            //string alias = context.GetAlias(this.ValueType);
            //if (string.IsNullOrEmpty(alias))
                metadata = new DictionaryItemValuePropertyMetadata("Value", this.ValueType, this.Metadata);
            //else
            //    metadata = new DictionaryItemValuePropertyMetadata(alias, this.ValueType, this.Metadata);

            return new DictionaryKeyValuePropertyDescriptor(metadata);
        }


        public Type KeyType
        {
            get
            {
                if (_keyType == null)
                {
                    Type declaringType = this.Metadata.PropertyType;

                    if (declaringType.IsGenericType)
                    {
                        Type[] genercisTypes = declaringType.GetGenericArguments();
                        _keyType = genercisTypes[0];
                    }
                    else
                    {
                        _keyType = typeof(object);
                    }
                }

                return _keyType;
            }
        }

        public Type ValueType
        {
            get
            {
                if (_valueType == null)
                {
                    Type declaringType = this.Metadata.PropertyType;

                    if (declaringType.IsGenericType)
                    {
                        Type[] genercisTypes = declaringType.GetGenericArguments();
                        _valueType = genercisTypes[1];
                    }
                    else
                    {
                        _valueType = typeof(object);
                    }
                }

                return _valueType;
            }
        }


        #region -- ToXml --

        protected override bool DoMustDeclareTypeNameInXmlElement(Type entityType, XmlSerializerContext context)
        {
            //las listas no necesitan declarar el tipo, se crea un tipo por defecto....
            //si es generica y es root, se debe declarar el tipo.
            if (entityType.IsGenericType && context.IsRootElement(this))
                return true;
            else
                return false; //entityType != this.PropertyType;
        }

        public override string GetAttributeNameForType(Type propertyEntityType, XmlSerializerContext context)
        {
            //TODO hacer que las listas se puedan guardar en un atributo, los items separados por ;...&, algo asi
            return null;
        }

        #endregion


        #region -- From Xml --

        public override Type GetTypeFromAttributeName(string attributeName, XmlSerializerContext context)
        {
            return null;
        }

        public override Type GetTypeFromElementName(string elementName, XmlSerializerContext context)
        {
            Type type = base.GetTypeFromElementName(elementName, context);

            if (type != null)
                return type;

            //si el tipo declarado es una interface....devuelvo el tipo List<T>
            if (this.Metadata.PropertyType.IsInterface)
            {
                //IList o IList<T>
                if (this.Metadata.PropertyType.IsGenericType)
                {
                    Type dicType = typeof(Dictionary<,>);
                    dicType = dicType.MakeGenericType(this.KeyType, this.ValueType);
                    return dicType;
                }
                else
                {
                    return typeof(Hashtable);
                }
            }

            return null;
        }

        #endregion


        #region -- Item ToXml --

        public string GetElementNameForDictionaryItem(XmlSerializerContext context)
        {
            //tengo que ver si se escribe el elemento por default o sacarlo del atributo
            if (string.IsNullOrEmpty(this.Metadata.DictionaryItemElementName))
                return "Entry";
            else
                return this.Metadata.DictionaryItemElementName;
        }

        #endregion
    }
}

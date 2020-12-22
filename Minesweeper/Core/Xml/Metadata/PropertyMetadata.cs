namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using Minesweeper.Core.Xml.Attributes;
    using Minesweeper.Core.Helpers;

    
    public class PropertyMetadata
    {
        public static readonly Dictionary<string, Type> EmptyAliasToTypeMap = new Dictionary<string, Type>();
        public static readonly Dictionary<Type, string> EmptyTypeToAliasMap = new Dictionary<Type, string>();
        public static readonly List<Attribute> EmptyAttributes = new List<Attribute>();
        public static readonly List<Type> EmptyIncludeTypes = new List<Type>();

        private Dictionary<Type, string> _typeToAttributeItemMap;
        private Dictionary<Type, string> _typeToAttributeMap;
        private Dictionary<Type, string> _typeToElementItemMap;
        private Dictionary<Type, string> _typeToElementMap;

        private Dictionary<string, Type> _elementToTypeItemMap;
        private Dictionary<string, Type> _elementToTypeMap;
        private Dictionary<string, Type> _attributeToTypeItemMap;
        private Dictionary<string, Type> _attributeToTypeMap;

        private Dictionary<string, Type> _attributeToTypeDictionaryKeyMap;
        private Dictionary<string, Type> _attributeToTypeDictionaryValueMap;
        private Dictionary<string, Type> _elementToTypeDictionaryKeyMap;
        private Dictionary<string, Type> _elementToTypeDictionaryValueMap;

        private Dictionary<Type, string> _typeToAttributeDictionaryKeyMap;
        private Dictionary<Type, string> _typeToAttributeDictionaryValueMap;
        private Dictionary<Type, string> _typeToElementDictionaryKeyMap;
        private Dictionary<Type, string> _typeToElementDictionaryValueMap;


        private List<Attribute> _attributes;
        private List<Type> _includeTypes;


        #region -- Constructors --

        public PropertyMetadata()
        {
            this.IsSerializable = true;
        }

        public PropertyMetadata(PropertyInfo propertyInfo)
        {
            this.IsSerializable = true;
            this.PropertyInfo = propertyInfo;
            this.Initialize(null);
        }

        #endregion



        #region -- Initialization --

        public virtual void Initialize(List<Attribute> attributes)
        {
            //si la propiedad Get es publica, se adopta que la propiedad entera es serializable
            if (this.PropertyInfo.GetSetMethod() != null && this.PropertyInfo.GetSetMethod().IsPublic) 
                this.IsSerializable = true;
            else
                this.IsSerializable = false;

            object att;

            if (attributes == null)
            {
                object[] atts = this.PropertyInfo.GetCustomAttributes(true);
                for (int i = 0; i < atts.Length; i++)
                {
                    att = atts[i];
                    this.IsSerializable = this.InitializeAttribute((Attribute)att) || this.IsSerializable;
                }
            }
            else
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    att = attributes[i];
                    this.IsSerializable = this.InitializeAttribute((Attribute)att) || this.IsSerializable;
                }
            }
        }

        /// <summary>
        /// Devuelve un booleano indicando que el atributo es de serializacion en xml
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        protected virtual bool InitializeAttribute(Attribute att)
        {
            bool output = false;

            if (att is XmlElementValueAttribute)
            {
                XmlElementValueAttribute a = att as XmlElementValueAttribute;
                if (a.Type == null)
                {
                    if (TypeHelper.IsValueType(a.Type))
                        this.DefaultAttributeName = a.AttributeName;
                    this.DefaultElementName = a.ElementName;
                }
                else
                {
                    if (TypeHelper.IsValueType(a.Type))
                    {
                        this.AddAttributeToTypeMap(a.AttributeName, a.Type);
                        this.AddTypeToAttributeMap(a.Type, a.AttributeName);
                    }

                    this.AddElementToTypeMap(a.ElementName, a.Type);
                    this.AddTypeToElementMap(a.Type, a.ElementName);

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }

                output = true;
            }
            else if (att is XmlContentAttribute)
            {
                this.AddAttribute(att);
                output = true;
            }
            else if (att is XmlDateTimeFormatAttribute)
            {
                this.AddAttribute(att);
                output = true;
            }
            else if (att is XmlTimeSpanFormatAttribute)
            {
                this.AddAttribute(att);
                output = true;
            }
            else if (att is XmlNumericFormatAttribute)
            {
                this.AddAttribute(att);
                output = true;
            }
            else if (att is XmlOrderAttribute)
            {
                this.Order = ((XmlOrderAttribute)att).Order;
            }
            else if (att is XmlIncludeAttribute)
            {
                this.IncludeTypes.Add(((XmlIncludeAttribute)att).Type);
            }
            else if (att is XmlArrayAttribute)
            {
                this.DefaultElementName = ((XmlArrayAttribute)att).ElementName;
                output = true;
            }
            else if (att is XmlInlineArrayAttributeAttribute)
            {
                XmlInlineArrayAttributeAttribute a = att as XmlInlineArrayAttributeAttribute;

                if (a.Type == null)
                {
                    this.DefaultAttributeName = a.AttributeName;
                    this.DefaultElementName = null;

                    if (!string.IsNullOrEmpty(a.ItemSeparator))
                        this.AddAttribute(a);
                }
                else
                {
                    Type itemType = a.Type.GetElementType();
                    if (itemType == null)
                        throw new Exception("No es posible inferir el tipo del item del array de la propiedad " + this.PropertyName + " del tipo " + this.OwnerType.Name);

                    if (this.PropertyType.IsArray)
                    {
                        Type dcItemType = this.PropertyType.GetElementType();
                        if (dcItemType == null || !dcItemType.Equals(itemType))
                            throw new Exception("El tipo del item del array declarado en el atributo XmlInlineArrayAttributeAttribute es distinto al tipo del item del array de la propiedad " + this.PropertyName + " del tipo " + this.OwnerType.Name);
                    }

                    //agrego el tipo de el arrar..
                    this.AddAttributeToTypeMap(a.AttributeName, a.Type);
                    this.AddTypeToAttributeMap(a.Type, a.AttributeName);

                    //agrego el tipo del item
                    this.AddTypeToAttributeItemMap(itemType, a.AttributeName);
                    this.AddAttributeToTypeItemMap(a.AttributeName, itemType);

                    //si el tipo del array es object[], los item son por ej int, detecta que no es inline, pero al serializaer los items va 
                    //a detectar que cada item se tiene que serializar como attributo, pero no tiene elemento definido.....
                    //this.AddTypeToElementItemMap(a.Type.GetElementType(), a.AttributeName);
                    //this.AddElementToTypeItemMap(a.AttributeName, a.Type.GetElementType());

                    //el separador lo busco en la lista de attributos
                    this.AddAttribute(att);

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }

                output = true;
            }
            else if (att is XmlInlineArrayElementAttribute)
            {
                XmlInlineArrayElementAttribute a = att as XmlInlineArrayElementAttribute;

                if (a.Type == null)
                {
                    this.DefaultAttributeName = null;
                    this.DefaultElementName = a.ElementName;

                    if (!string.IsNullOrEmpty(a.ItemSeparator))
                        this.AddAttribute(a);
                }
                else
                {
                    //agrego el tipo del item
                    this.AddTypeToElementItemMap(a.Type.GetElementType(), a.ElementName);
                    this.AddElementToTypeItemMap(a.ElementName, a.Type.GetElementType());

                    //agrego el tipo de el arrar..
                    this.AddElementToTypeMap(a.ElementName, a.Type);
                    this.AddTypeToElementMap(a.Type, a.ElementName);

                    //el separador lo busco en la lista de attributos
                    this.AddAttribute(att);

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }
                output = true;
            }
            else if (att is XmlArrayItemValueAttribute)
            {
                XmlArrayItemValueAttribute a = att as XmlArrayItemValueAttribute;
                if (a.Type == null)
                {
                    this.DefaulItemAttributeName = a.AttributeName;
                    this.DefaultItemElementName = a.ElementName;
                }
                else
                {
                    this.AddTypeToElementItemMap(a.Type, a.ElementName);
                    this.AddElementToTypeItemMap(a.ElementName, a.Type);

                    if (TypeHelper.IsValueType(a.Type))
                    {
                        this.AddTypeToAttributeItemMap(a.Type, a.AttributeName);
                        this.AddAttributeToTypeItemMap(a.AttributeName, a.Type);
                    }

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }
                output = true;
            }
            else if (att is XmlArrayItemAttribute)
            {
                XmlArrayItemAttribute a = att as XmlArrayItemAttribute;
                if (a.Type == null)
                    this.DefaultItemElementName = a.ElementName;
                else
                {
                    this.AddTypeToElementItemMap(a.Type, a.ElementName);
                    this.AddElementToTypeItemMap(a.ElementName, a.Type);

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }
                output = true;
            }
            else if (att is XmlDictionaryKeyElementAttribute)
            {
                XmlDictionaryKeyElementAttribute a = att as XmlDictionaryKeyElementAttribute;
                if (a.Type == null)
                {
                    this.DefaultDictionaryKeyElementName = a.ElementName;
                }
                else
                {
                    this.AddElementToTypeDictionaryKeyMap(a.ElementName, a.Type);
                    this.AddTypeToElementDictionaryKeyMap(a.Type, a.ElementName);

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }
                output = true;
            }
            else if (att is XmlDictionaryValueElementAttribute)
            {
                XmlDictionaryValueElementAttribute a = att as XmlDictionaryValueElementAttribute;
                if (a.Type == null)
                {
                    this.DefaultDictionaryValueElementName = a.ElementName;
                }
                else
                {
                    this.AddElementToTypeDictionaryValueMap(a.ElementName, a.Type);
                    this.AddTypeToElementDictionaryValueMap(a.Type, a.ElementName);

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }
                output = true;
            }
            else if (att is XmlDictionaryKeyAttributeAttribute)
            {
                XmlDictionaryKeyAttributeAttribute a = att as XmlDictionaryKeyAttributeAttribute;
                if (a.Type == null)
                {
                    Type declaringType = this.PropertyType;
                    Type keyType = typeof(object);

                    if (declaringType.IsGenericType)
                    {
                        Type[] genercisTypes = declaringType.GetGenericArguments();
                        keyType = genercisTypes[0];
                    }

                    if (TypeHelper.IsValueType(keyType))
                        this.DefaultDictionaryKeyAttributeName = a.AttributeName;
                }
                else
                {
                    if (TypeHelper.IsValueType(a.Type))
                    {
                        this.AddAttributeToTypeDictionaryKeyMap(a.AttributeName, a.Type);
                        this.AddTypeToAttributeDictionaryKeyMap(a.Type, a.AttributeName);
                    }

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }
                output = true;
            }
            else if (att is XmlDictionaryValueAttributeAttribute)
            {
                XmlDictionaryValueAttributeAttribute a = att as XmlDictionaryValueAttributeAttribute;
                if (a.Type == null)
                {
                    Type declaringType = this.PropertyType;
                    Type valueType = typeof(object);

                    if (declaringType.IsGenericType)
                    {
                        Type[] genercisTypes = declaringType.GetGenericArguments();
                        valueType = genercisTypes[0];
                    }

                    if (TypeHelper.IsValueType(valueType))
                        this.DefaultDictionaryValueAttributeName = a.AttributeName;
                }
                else
                {
                    if (TypeHelper.IsValueType(a.Type))
                    {
                        this.AddAttributeToTypeDictionaryValueMap(a.AttributeName, a.Type);
                        this.AddTypeToAttributeDictionaryValueMap(a.Type, a.AttributeName);
                    }

                    if (!this.IncludeTypes.Contains(a.Type))
                        this.AddIncludeTypes(a.Type);
                }
                output = true;
            }
            else if (att is XmlDictionaryItemElementAttribute)
            {
                this.DictionaryItemElementName = ((XmlDictionaryItemElementAttribute)att).ElementName;
                output = true;
            }
            else if (att is XmlElementAttribute)
            {
                XmlElementAttribute atElem = (XmlElementAttribute)att;

                if (!string.IsNullOrEmpty(atElem.ElementName))
                {
                    if (atElem.Type != null)
                    {
                        this.AddElementToTypeMap(atElem.ElementName, atElem.Type);
                        this.AddTypeToElementMap(atElem.Type, atElem.ElementName);

                        if (!this.IncludeTypes.Contains(atElem.Type))
                            this.AddIncludeTypes(atElem.Type);
                    }
                    else
                        this.DefaultElementName = atElem.ElementName;
                }
                else
                    this.DefaultElementName = this.PropertyName;

                output = true;
            }
            else if (att is XmlAttributeAttribute)
            {
                XmlAttributeAttribute atElem = (XmlAttributeAttribute)att;

                if (!string.IsNullOrEmpty(atElem.AttributeName))
                {
                    if (atElem.Type != null)
                    {
                        if (TypeHelper.IsValueType(atElem.Type))
                        {
                            this.AddAttributeToTypeMap(atElem.AttributeName, atElem.Type);
                            this.AddTypeToAttributeMap(atElem.Type, atElem.AttributeName);
                        }

                        if (!this.IncludeTypes.Contains(atElem.Type))
                            this.AddIncludeTypes(atElem.Type);
                    }
                    else
                    {
                        if (TypeHelper.IsValueType(this.PropertyType))
                            this.DefaultAttributeName = atElem.AttributeName;
                    }
                }
                else
                {
                    this.DefaultAttributeName = this.PropertyName;
                }

                output = true;
            }
            else
                this.AddAttribute(att as Attribute);

            return output;
        }

        #endregion


        public bool IsSerializable { get; set; }

        public virtual float Order { get; set; }


        public void AddAttribute(Attribute att)
        {
            if (_attributes == null)
                _attributes = new List<Attribute>();

            _attributes.Add(att);
        }

        public virtual List<Attribute> Attributes
        {
            get
            {
                if (_attributes == null)
                    return EmptyAttributes;
                else
                    return _attributes;
            }
            protected set
            {
                _attributes = value;
            }
        }

        public void AddIncludeTypes(Type type)
        {
            if (_includeTypes == null)
                _includeTypes = new List<Type>();

            _includeTypes.Add(type);
        }

        public virtual List<Type> IncludeTypes
        {
            get
            {
                if (_includeTypes == null)
                    return EmptyIncludeTypes;
                else
                    return _includeTypes;
            }
            protected set
            {
                _includeTypes = value;
            }
        }


        protected PropertyInfo PropertyInfo { get; set; }


        public virtual Type OwnerType
        {
            get
            {
                return this.PropertyInfo.DeclaringType;
            }
        }

        public virtual Type PropertyType
        {
            get
            {
                return this.PropertyInfo.PropertyType;
            }
        }

        public virtual string PropertyName
        {
            get
            {
                return this.PropertyInfo.Name;
            }
        }


        #region -- Elements --

        public virtual string DefaultElementName { get; protected set; }

        public virtual void AddTypeToElementMap(Type type, string element)
        {
            if (_typeToElementMap == null)
                _typeToElementMap = new Dictionary<Type, string>();

            if (_typeToElementMap.ContainsKey(type))
                _typeToElementMap[type] = element;
            else
                _typeToElementMap.Add(type, element);
        }

        public virtual void AddElementToTypeMap(string element, Type type)
        {
            if (_elementToTypeMap == null)
                _elementToTypeMap = new Dictionary<string, Type>();

            if (_elementToTypeMap.ContainsKey(element))
                _elementToTypeMap[element] = type;
            else
                _elementToTypeMap.Add(element, type);
        }

        public virtual Dictionary<Type, string> TypeToElementMap
        {
            get
            {
                if (_typeToElementMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToElementMap;
            }
            protected set
            {
                _typeToElementMap = value;
            }
        }

        public virtual Dictionary<string, Type> ElementToTypeMap
        {
            get
            {
                if (_elementToTypeMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _elementToTypeMap;
            }
            protected set
            {
                _elementToTypeMap = value;
            }
        }

        #endregion


        #region -- Attributes --

        public virtual string DefaultAttributeName { get; protected set; }

        public virtual void AddTypeToAttributeMap(Type type, string attribute)
        {
            if (_typeToAttributeMap == null)
                _typeToAttributeMap = new Dictionary<Type, string>();

            if (_typeToAttributeMap.ContainsKey(type))
                _typeToAttributeMap[type] = attribute;
            else
                _typeToAttributeMap.Add(type, attribute);
        }

        public virtual void AddAttributeToTypeMap(string attribute, Type type)
        {
            if (_attributeToTypeMap == null)
                _attributeToTypeMap = new Dictionary<string, Type>();

            if (_attributeToTypeMap.ContainsKey(attribute))
                _attributeToTypeMap[attribute] = type;
            else
                _attributeToTypeMap.Add(attribute, type);
        }

        public virtual Dictionary<Type, string> TypeToAttributeMap
        {
            get
            {
                if (_typeToAttributeMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToAttributeMap;
            }
            protected set
            {
                _typeToAttributeMap = value;
            }
        }
        public virtual Dictionary<string, Type> AttributeToTypeMap
        {
            get
            {
                if (_attributeToTypeMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _attributeToTypeMap;
            }
            protected set
            {
                _attributeToTypeMap = value;
            }
        }
        #endregion


        #region -- List Items --

        public virtual string DefaultItemElementName { get; protected set; }


        public virtual void AddTypeToElementItemMap(Type type, string element)
        {
            if (_typeToElementItemMap == null)
                _typeToElementItemMap = new Dictionary<Type, string>();

            if (_typeToElementItemMap.ContainsKey(type))
                _typeToElementItemMap[type] = element;
            else
                _typeToElementItemMap.Add(type, element);
        }

        public virtual void AddElementToTypeItemMap(string element, Type type)
        {
            if (_elementToTypeItemMap == null)
                _elementToTypeItemMap = new Dictionary<string, Type>();

            if (_elementToTypeItemMap.ContainsKey(element))
                _elementToTypeItemMap[element] = type;
            else
                _elementToTypeItemMap.Add(element, type);
        }


        public virtual Dictionary<Type, string> TypeToElementItemMap
        {
            get
            {
                if (_typeToElementItemMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToElementItemMap;
            }
            protected set
            {
                _typeToElementItemMap = value;
            }
        }


        public virtual Dictionary<string, Type> ElementToTypeItemMap
        {
            get
            {
                if (_elementToTypeItemMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _elementToTypeItemMap;
            }
            protected set
            {
                _elementToTypeItemMap = value;
            }
        }



        public virtual string DefaulItemAttributeName { get; protected set; }

        public virtual void AddTypeToAttributeItemMap(Type type, string attribute)
        {
            if (_typeToAttributeItemMap == null)
                _typeToAttributeItemMap = new Dictionary<Type, string>();

            if (_typeToAttributeItemMap.ContainsKey(type))
                _typeToAttributeItemMap[type] = attribute;
            else
                _typeToAttributeItemMap.Add(type, attribute);
        }

        public virtual void AddAttributeToTypeItemMap(string attribute, Type type)
        {
            if (_attributeToTypeItemMap == null)
                _attributeToTypeItemMap = new Dictionary<string, Type>();

            if (_attributeToTypeItemMap.ContainsKey(attribute))
                _attributeToTypeItemMap[attribute] = type;
            else
                _attributeToTypeItemMap.Add(attribute, type);
        }

        public virtual Dictionary<Type, string> TypeToAttributeItemMap
        {
            get
            {
                if (_typeToAttributeItemMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToAttributeItemMap;
            }
            protected set
            {
                _typeToAttributeItemMap = value;
            }
        }

        public virtual Dictionary<string, Type> AttributeToTypeItemMap
        {
            get
            {
                if (_attributeToTypeItemMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _attributeToTypeItemMap;
            }
            protected set
            {
                _attributeToTypeItemMap = value;
            }
        }

        #endregion


        #region -- Dictionary ---

        public string DictionaryItemElementName { get; set; }


        #region -- Key --

        public virtual string DefaultDictionaryKeyElementName { get; protected set; }


        public virtual void AddTypeToElementDictionaryKeyMap(Type type, string element)
        {
            if (_typeToElementDictionaryKeyMap == null)
                _typeToElementDictionaryKeyMap = new Dictionary<Type, string>();

            if (_typeToElementDictionaryKeyMap.ContainsKey(type))
                _typeToElementDictionaryKeyMap[type] = element;
            else
                _typeToElementDictionaryKeyMap.Add(type, element);
        }

        public virtual void AddElementToTypeDictionaryKeyMap(string element, Type type)
        {
            if (_elementToTypeDictionaryKeyMap == null)
                _elementToTypeDictionaryKeyMap = new Dictionary<string, Type>();

            if (_elementToTypeDictionaryKeyMap.ContainsKey(element))
                _elementToTypeDictionaryKeyMap[element] = type;
            else
                _elementToTypeDictionaryKeyMap.Add(element, type);
        }


        public virtual Dictionary<Type, string> TypeToElementDictionaryKeyMap
        {
            get
            {
                if (_typeToElementDictionaryKeyMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToElementDictionaryKeyMap;
            }
            protected set
            {
                _typeToElementDictionaryKeyMap = value;
            }
        }

        public virtual Dictionary<string, Type> ElementToTypeDictionaryKeyMap
        {
            get
            {
                if (_elementToTypeDictionaryKeyMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _elementToTypeDictionaryKeyMap;
            }
            protected set
            {
                _elementToTypeDictionaryKeyMap = value;
            }
        }



        public virtual string DefaultDictionaryKeyAttributeName { get; protected set; }


        public virtual void AddTypeToAttributeDictionaryKeyMap(Type type, string element)
        {
            if (_typeToAttributeDictionaryKeyMap == null)
                _typeToAttributeDictionaryKeyMap = new Dictionary<Type, string>();

            if (_typeToAttributeDictionaryKeyMap.ContainsKey(type))
                _typeToAttributeDictionaryKeyMap[type] = element;
            else
                _typeToAttributeDictionaryKeyMap.Add(type, element);
        }

        public virtual void AddAttributeToTypeDictionaryKeyMap(string element, Type type)
        {
            if (_attributeToTypeDictionaryKeyMap == null)
                _attributeToTypeDictionaryKeyMap = new Dictionary<string, Type>();

            if (_attributeToTypeDictionaryKeyMap.ContainsKey(element))
                _attributeToTypeDictionaryKeyMap[element] = type;
            else
                _attributeToTypeDictionaryKeyMap.Add(element, type);
        }


        public virtual Dictionary<Type, string> TypeToAttributeDictionaryKeyMap
        {
            get
            {
                if (_typeToAttributeDictionaryKeyMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToAttributeDictionaryKeyMap;
            }
            protected set
            {
                _typeToAttributeDictionaryKeyMap = value;
            }
        }

        public virtual Dictionary<string, Type> AttributeToTypeDictionaryKeyMap
        {
            get
            {
                if (_attributeToTypeDictionaryKeyMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _attributeToTypeDictionaryKeyMap;
            }
            protected set
            {
                _attributeToTypeDictionaryKeyMap = value;
            }
        }

        #endregion


        #region -- Value --

        public virtual string DefaultDictionaryValueElementName { get; protected set; }


        public virtual void AddTypeToElementDictionaryValueMap(Type type, string element)
        {
            if (_typeToElementDictionaryValueMap == null)
                _typeToElementDictionaryValueMap = new Dictionary<Type, string>();

            if (_typeToElementDictionaryValueMap.ContainsKey(type))
                _typeToElementDictionaryValueMap[type] = element;
            else
                _typeToElementDictionaryValueMap.Add(type, element);
        }

        public virtual void AddElementToTypeDictionaryValueMap(string element, Type type)
        {
            if (_elementToTypeDictionaryValueMap == null)
                _elementToTypeDictionaryValueMap = new Dictionary<string, Type>();

            if (_elementToTypeDictionaryValueMap.ContainsKey(element))
                _elementToTypeDictionaryValueMap[element] = type;
            else
                _elementToTypeDictionaryValueMap.Add(element, type);
        }


        public virtual Dictionary<Type, string> TypeToElementDictionaryValueMap
        {
            get
            {
                if (_typeToElementDictionaryValueMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToElementDictionaryValueMap;
            }
            protected set
            {
                _typeToElementDictionaryValueMap = value;
            }
        }

        public virtual Dictionary<string, Type> ElementToTypeDictionaryValueMap
        {
            get
            {
                if (_elementToTypeDictionaryValueMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _elementToTypeDictionaryValueMap;
            }
            protected set
            {
                _elementToTypeDictionaryValueMap = value;
            }
        }



        public virtual string DefaultDictionaryValueAttributeName { get; protected set; }


        public virtual void AddTypeToAttributeDictionaryValueMap(Type type, string element)
        {
            if (_typeToAttributeDictionaryValueMap == null)
                _typeToAttributeDictionaryValueMap = new Dictionary<Type, string>();

            if (_typeToAttributeDictionaryValueMap.ContainsKey(type))
                _typeToAttributeDictionaryValueMap[type] = element;
            else
                _typeToAttributeDictionaryValueMap.Add(type, element);
        }

        public virtual void AddAttributeToTypeDictionaryValueMap(string element, Type type)
        {
            if (_attributeToTypeDictionaryValueMap == null)
                _attributeToTypeDictionaryValueMap = new Dictionary<string, Type>();

            if (_attributeToTypeDictionaryValueMap.ContainsKey(element))
                _attributeToTypeDictionaryValueMap[element] = type;
            else
                _attributeToTypeDictionaryValueMap.Add(element, type);
        }


        public virtual Dictionary<Type, string> TypeToAttributDictionaryValueMap
        {
            get
            {
                if (_typeToAttributeDictionaryValueMap == null)
                    return EmptyTypeToAliasMap;
                else
                    return _typeToAttributeDictionaryValueMap;
            }
            protected set
            {
                _typeToAttributeDictionaryValueMap = value;
            }
        }

        public virtual Dictionary<string, Type> AttributeToTypeDictionaryValueMap
        {
            get
            {
                if (_attributeToTypeDictionaryValueMap == null)
                    return EmptyAliasToTypeMap;
                else
                    return _attributeToTypeDictionaryValueMap;
            }
            protected set
            {
                _attributeToTypeDictionaryValueMap = value;
            }
        }


        #endregion

        #endregion


        #region -- Set/Get Value --

        public virtual object GetValue(object entity)
        {
            return this.PropertyInfo.GetValue(entity, null);
        }

        public virtual void SetValue(object entity, object value)
        {
            this.PropertyInfo.SetValue(entity, value, null);
        }

        #endregion

    }
}

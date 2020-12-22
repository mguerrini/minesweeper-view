namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Minesweeper.Core.Xml.Attributes;
    using System.Reflection;
    using Minesweeper.Core.Helpers;
    using System.Configuration;

    public class TypeDescriptor
    {
        private static readonly BindingFlags __flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance); 
        private static Type _configurationSectionType = typeof(ConfigurationSection);

        #region -- Constructors --

        public TypeDescriptor(Type type)
        {
            this.Type = type;
            this.Attributes = new List<Attribute>();
            this.PropertyMap = new Dictionary<string, PropertyDescriptor>();
            this.Properties = new List<PropertyDescriptor>();
            this.IncludeTypes = new List<Type>();
            this.ElementName = null;
            this.IsSerializable = TypeHelper.IsSerializable(type);
            this.GenericTypes = null;

            this.Initialize();
        }

        #endregion

        public bool IsArray { get { return this.Type.IsArray; } }

        public bool IsList { get; protected set; }

        public bool IsDictionary { get; protected set; }

        public bool IsValueType { get; protected set; }

        protected virtual void Initialize()
        {
            this.IsList = TypeHelper.IsListType(this.Type);
            this.IsDictionary = TypeHelper.IsDictionaryType(this.Type);
            this.IsValueType = TypeHelper.IsValueType(this.Type);

            if (this.Type.IsArray || TypeHelper.IsListType(this.Type) || TypeHelper.IsDictionaryType(this.Type) || TypeHelper.IsValueType(this.Type))
            {
                //no recorro todas las propiedades y atributos, ya que son de tipo basico, no tienen atributos
                this.InitializeBasicType();
            }
            else
            {
                this.InitializeObjectType();
            }

            if (this.IsSerializable && this.Type.IsGenericType)
                this.GenericTypes = this.Type.GetGenericArguments();
        }

        protected virtual void InitializeBasicType()
        {
            this.IsSerializable = true;
        }

        protected virtual void InitializeObjectType()
        {
            if (this.IsSerializable)
            {
                object att;
                object[] atts = this.Type.GetCustomAttributes(false);

                for (int i = 0; i < atts.Length; i++)
                {
                    att = atts[i];
                    if (att is XmlRootAttribute)
                        this.ElementName = ((XmlRootAttribute)atts[0]).ElementName;
                    else if (att is SerializableAttribute)
                        this.IsSerializable = true;
                    else if (att is XmlElementValueAttribute)
                    {
                        XmlElementValueAttribute e = (XmlElementValueAttribute)att;
                        this.ElementName = e.ElementName;

                        if (TypeHelper.IsValueType(this.Type))
                            this.AttributeName = e.AttributeName;
                    }
                    else
                        this.Attributes.Add(att as Attribute);
                }

                object[] atts1 = this.Type.GetCustomAttributes(typeof(XmlIncludeAttribute), true);
                foreach (XmlIncludeAttribute at in atts1)
                    this.IncludeTypes.Add(at.Type);

                //cargo la propiedades
                PropertyInfo info;
                PropertyInfo[] infos = this.Type.GetProperties(__flags);
                PropertyDescriptor prop;

                for (int i = 0; i < infos.Length; i++)
                {
                    info = infos[i];
                    Type t = info.PropertyType;

                    if ((TypeHelper.IsSerializable(t)|| t.IsInterface) && info.CanRead && info.CanWrite)
                    {
                        try
                        {
                            object[] ignoreAtt = info.GetCustomAttributes(typeof(XmlIgnoreAttribute), false);

                            if (ignoreAtt.Length == 0)
                            {
                                if (t.IsArray || TypeHelper.IsListType(t))
                                    prop = new ListPropertyDescriptor(info);
                                else if (TypeHelper.IsDictionaryType(t))
                                    prop = new DictionaryPropertyDescriptor(info);
                                else if (TypeHelper.IsValueType(t))
                                    prop = new ValueTypePropertyDescriptor(info);
                                else
                                    prop = new ObjectPropertyDescriptor(info);

                                //si la propiedad es serializable, la agrego...sino no
                                if (prop.Metadata.IsSerializable)
                                {
                                    this.Properties.Add(prop);
                                    this.PropertyMap.Add(prop.Metadata.PropertyName, prop);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error al intentar obtener la descripción de la propiedad " + info.Name + " del tipo " + this.Type.FullName, ex);
                        }
                    }
                }

                this.Properties = this.Properties.OrderBy(p => p.Metadata.Order).ToList();
            }
        }


        public virtual bool IsSerializable { get; protected set; }


        public Type Type { get; protected set; }

        public Type[] GenericTypes { get; protected set; }


        public string ElementName { get; protected set; }

        public string AttributeName { get; protected set; }


        public List<Type> IncludeTypes { get; protected set; }

        public List<Attribute> Attributes { get; protected set; }




        public string GetRootName()
        {
            if (string.IsNullOrEmpty(this.ElementName))
            {
                if (this.Type.IsGenericType)
                    return this.Type.Name.Replace("`", "");
                else if (this.Type.IsArray)
                    return this.Type.Name.Replace("[]", "Array");
                else
                    return this.Type.Name;
            }
            else
                return this.ElementName;
        }



        private Dictionary<string, PropertyDescriptor> PropertyMap { get; set; }

        public List<PropertyDescriptor> Properties { get; protected set; }

        

        public PropertyDescriptor GetPropertyByAttributeName(string attributeName)
        {
            foreach (PropertyDescriptor desc in this.Properties)
            {
                if (!string.IsNullOrEmpty(desc.Metadata.DefaultAttributeName) && string.Compare(attributeName, desc.Metadata.DefaultAttributeName) == 0)
                    return desc;

                if (desc.Metadata.AttributeToTypeMap.ContainsKey(attributeName))
                    return desc;
            }

            return null;
        }

        public PropertyDescriptor GetPropertyByElementName(string elementName)
        {
            foreach (PropertyDescriptor desc in this.Properties)
            {
                if (!string.IsNullOrEmpty(desc.Metadata.DefaultElementName) && string.Compare(elementName, desc.Metadata.DefaultElementName) == 0)
                    return desc;

                if (desc.Metadata.ElementToTypeMap.ContainsKey(elementName))
                    return desc;
            }

            if (this.PropertyMap.ContainsKey(elementName))
                return this.PropertyMap[elementName];
            else
                return null;
        }

       

        public PropertyDescriptor GetPropertyIfIsAttribute(string propertyName, Type entityType)
        {
            if (this.PropertyMap.ContainsKey(propertyName))
            {
                PropertyDescriptor desc = this.PropertyMap[propertyName];

                if (!string.IsNullOrEmpty(desc.Metadata.DefaultAttributeName) && string.IsNullOrEmpty(desc.Metadata.DefaultElementName))
                    return desc;

                if (desc.Metadata.TypeToElementMap.ContainsKey(entityType))
                    return null;

                if (desc.Metadata.TypeToAttributeMap.ContainsKey(entityType))
                    return desc;
            }

            return null;
        }

        public PropertyDescriptor GetPropertyIfIsElement(string propertyName, Type entityType)
        {
            if (this.PropertyMap.ContainsKey(propertyName))
            {
                PropertyDescriptor desc = this.PropertyMap[propertyName];

                if (!string.IsNullOrEmpty(desc.Metadata.DefaultAttributeName) && string.IsNullOrEmpty(desc.Metadata.DefaultElementName))
                    return null;

                if (desc.Metadata.TypeToElementMap.ContainsKey(entityType))
                    return desc;

                if (desc.Metadata.TypeToAttributeMap.ContainsKey(entityType))
                    return null;

                return desc;
            }

            return null;
        }

       
        public bool IsAttributeProperty(PropertyDescriptor desc, Type entityType)
        {
            return this.GetPropertyIfIsAttribute(desc.Metadata.PropertyName, entityType) != null;
        }

        public bool IsElementProperty(PropertyDescriptor desc, Type entityType)
        {
            return this.GetPropertyIfIsElement(desc.Metadata.PropertyName, entityType) != null;
        }

        public bool IsAttributeNullValueProperty(string propertyName)
        {
            if (this.PropertyMap.ContainsKey(propertyName))
            {
                PropertyDescriptor desc = this.PropertyMap[propertyName];
                return !string.IsNullOrEmpty(desc.Metadata.DefaultAttributeName);
            }

            return false;
        }

        public bool CanWriteNullValue(string propertyName)
        {
            //puede escribir valores nulos solo si hay una unica opcion
            if (this.PropertyMap.ContainsKey(propertyName))
            {
                PropertyDescriptor desc = this.PropertyMap[propertyName];

                if (desc.Metadata.TypeToElementMap.Count == 0 && desc.Metadata.TypeToAttributeMap.Count == 0)
                {
                    bool atDef = !string.IsNullOrEmpty(desc.Metadata.DefaultAttributeName);
                    bool elDef = !string.IsNullOrEmpty(desc.Metadata.DefaultElementName);

                    //no tiene valores por default o solo tiene uno
                    return (atDef && !elDef) || (!atDef && elDef) || (!atDef && !elDef);
                }
            }

            return false;
        }


        public override string ToString()
        {
            return "Type: " + this.Type.Name + " Root: " + this.GetRootName();
        }


        
    }
}

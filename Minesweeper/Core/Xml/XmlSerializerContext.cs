namespace Minesweeper.Core.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;
    using System.Reflection;
    using Minesweeper.Core.Xml.Exceptions;
    using Minesweeper.Core.Xml.Converters;
    using System.Xml.Serialization;
    using System.Collections;
    using Minesweeper.Core.Helpers;
    using System.Configuration;

    public class XmlSerializerContext : IXmlContextData
    {
        public static Dictionary<string, Type> KnowAliasMap = new Dictionary<string, Type>();
        public static Dictionary<Type, string> KnowAliasReverseMap = new Dictionary<Type, string>();

        public static Dictionary<Type, IXmlConverter> ConverterMap = new Dictionary<Type, IXmlConverter>();
        public static Dictionary<Type, TypeDescriptor> TypeDescriptorMap = new Dictionary<Type, TypeDescriptor>();
        
        
        private static readonly Type __configurationSectionType = typeof(ConfigurationSection);
        private static readonly Type __interfaceXmlSerializableType = typeof(IXmlSerializable);
        private static readonly Type __arrayType = typeof(Array);
        private static readonly Type __enumType = typeof(Enum);
        private static readonly Type __methodInfoType = typeof(MethodInfo);
        private static readonly Assembly __mscorlib = typeof(int).Assembly;
        private static readonly Type __nullType = typeof(NullType);
        private static readonly Type __objectType = typeof(object);
        private static readonly Type __typeType = typeof(Type);

        private static readonly Type __listType = typeof(IList);
        private static readonly Type __dictionaryType = typeof(IDictionary);
        //private static readonly Type _holderType = typeof(ObjectHolder<>);


        #region -- Constructors --

        public XmlSerializerContext()
        {
            this.Stack = new XmlSerializerContextStack();
        }

        #endregion

        public XmlSerializerSettings Settings { get; set; }

        public XmlSerializerContextStack Stack {get; set;}


        #region -- Alias --

        public string GetGlobalAlias(Type entityType)
        {
            string output;

            if (XmlSerializerContext.KnowAliasReverseMap.TryGetValue(entityType, out output))
                return output;
            else if (entityType.IsGenericType)
            {
                entityType = entityType.GetGenericTypeDefinition();

                if (XmlSerializerContext.KnowAliasReverseMap.TryGetValue(entityType, out output))
                    return output;
            }

            return null;
        }

        public string GetAlias(Type entityType)
        {
            string alias = this.Stack.GetAlias(entityType);
            if (string.IsNullOrEmpty(alias))
                return this.GetGlobalAlias(entityType);
            else
                return alias;
        }

        public Type GetTypeFromAlias(string alias)
        {
            return this.GetTypeFromAlias(alias, null);
        }

        public Type GetTypeFromAlias(string alias, Type assignableTo)
        {
            Type output = this.Stack.GetTypeFromAlias(alias, assignableTo);
            if (output == null)
            {
                if (KnowAliasMap.ContainsKey(alias))
                    output = KnowAliasMap[alias];
            }

            return output;
        }

        public Type GetTypeFromAttribute(XmlReader reader)
        {
            string typeStr = reader.GetAttribute(this.Settings.TypeSettings.AttributeTypeName);
            if (!string.IsNullOrEmpty(typeStr))
                typeStr = reader.GetAttribute(this.Settings.TypeSettings.AttributeTypeName);

            if (!string.IsNullOrEmpty(typeStr))
            {
                try
                {
                    Type t = Type.GetType(typeStr, false);
                    if (t == null)
                        t = TypeConverter.GetSystemType(typeStr);

                    if (t == null)
                        throw new Exception("No es posible crear la instancia de Type de nombre '" + typeStr + "'.");

                    return t;
                }
                catch (Exception ex)
                {
                    throw new Exception("No es posible crear la instancia de Type de nombre '" + typeStr + "'.", ex);
                }
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region -- Converters --

        public IXmlConverter ObjectConverter
        {
            get
            {
                return this.GlobalConverterMap[__objectType];
            }
        }


        protected Dictionary<Type, IXmlConverter> GlobalConverterMap
        {
            get
            {
                return ConverterMap;
            }
        }

        protected Dictionary<Type, IXmlConverter> LocalConverterMap
        {
            get
            {
                return this.Stack.ConverterMap;
            }
        }


        public IXmlConverter GetConverter(Type type)
        {
            if (type == null)
                type = __nullType;
            
            //busco en el repositorio local, tiene mas prioridad.
            IXmlConverter objectConverter = null;
            this.LocalConverterMap.TryGetValue(type, out objectConverter);
            
            //busco en el global
            if (objectConverter == null)
                this.GlobalConverterMap.TryGetValue(type, out objectConverter);

            //busco en el global el que mejor se adapte
            if (objectConverter == null)
            {
                if (__interfaceXmlSerializableType.IsAssignableFrom(type))
                {
                    return (this.GlobalConverterMap[__interfaceXmlSerializableType] as IXmlConverter);
                }
                if (__configurationSectionType.IsAssignableFrom(type))
                {
                    return (this.GlobalConverterMap[__configurationSectionType] as IXmlConverter);
                }
                if (type.IsArray)
                {
                    return (this.GlobalConverterMap[__arrayType] as IXmlConverter);
                }
                if (type.IsEnum)
                {
                    return (this.GlobalConverterMap[__enumType] as IXmlConverter);
                }
                if (__listType.IsAssignableFrom(type) || type.Name.StartsWith("IList"))
                {
                    return (this.GlobalConverterMap[__listType] as IXmlConverter);
                }
                if (__dictionaryType.IsAssignableFrom(type) || type.Name.StartsWith("IDictionary"))
                {
                    return (this.GlobalConverterMap[__dictionaryType] as IXmlConverter);
                }
                if (type.IsSubclassOf(__typeType))
                {
                    return (this.GlobalConverterMap[__typeType] as IXmlConverter);
                }
                if (type.IsSubclassOf(__methodInfoType))
                {
                    return (this.GlobalConverterMap[__methodInfoType] as IXmlConverter);
                }
                if (type.Name.StartsWith("Predicate`1"))
                {
                    return NoopConverter.Instance;
                }
                //me fijo si es un enum
                if (TypeHelper.IsNullableType(type))
                {
                    type = TypeHelper.GetNullableType(type);
                    return this.GetConverter(type);
                }

                objectConverter = this.ObjectConverter;
            }
            if (objectConverter == null)
            {
                throw new XmlDataSerializerException("No valid converter found for type " + type);
            }
            return objectConverter;
        }

        public virtual void RegisterConverter(Type type, IXmlConverter converter)
        {
            if (type == null)
                type = __nullType;

            this.Stack.ConverterMap[type] = converter;

            if (!this.GlobalConverterMap.ContainsKey(type))
                this.GlobalConverterMap[type] = converter;
        }

        public void AddConverter(IXmlConverter converter)
        {
            converter.Register(this);
        }

        private bool IsListType(Type type)
        {
            if (type.IsGenericType && type.IsInterface)
            {
                Type[] genericsTypes = type.GetGenericArguments();
                if (genericsTypes.Length == 1)
                {
                    Type listType = typeof(IList<>).MakeGenericType(genericsTypes[0]);
                    return listType.IsAssignableFrom(type);
                }
                else
                    return false;
            }
            else
                return typeof(IList).IsAssignableFrom(type);
        }

        private  bool IsDictionaryType(Type type)
        {
            if (type.IsGenericType && type.IsInterface)
            {
                Type[] genericsTypes = type.GetGenericArguments();
                if (genericsTypes.Length == 2)
                {
                    Type dictionaryType = typeof(IDictionary<,>).MakeGenericType(genericsTypes[0], genericsTypes[1]);
                    return dictionaryType.IsAssignableFrom(type);
                }
                else
                    return false;
            }
            else
                return typeof(IDictionary).IsAssignableFrom(type);
        }

        #endregion


        #region -- Type Descriptors --

        protected Dictionary<Type, TypeDescriptor> GlobalTypeDescriptorMap
        {
            get
            {
                return TypeDescriptorMap;
            }
        }

        protected Dictionary<Type, TypeDescriptor> LocalTypeDescriptorMap
        {
            get
            {
                return this.Stack.TypeDescriptorMap;
            }
        }


        public TypeDescriptor GetTypeDescriptor(Type type)
        {
            if (TypeHelper.IsSerializable(type))
            {
                TypeDescriptor desc;
                if (this.LocalTypeDescriptorMap.TryGetValue(type, out desc))
                    return desc;

                if (this.GlobalTypeDescriptorMap.TryGetValue(type, out desc))
                {
                    //actualizo el dsecriptor local....para registrar que se uso el tipo
                    this.LocalTypeDescriptorMap[type] = desc;

                    //registro el alias
                    this.Stack.RegisterAlias(desc);

                    //registro los knowtyps
                    foreach (Type t in desc.IncludeTypes)
                        this.GetTypeDescriptor(t);

                    return desc;
                }
                else
                {
                    desc = new TypeDescriptor(type);

                    //lo registro de manera local, ya que globalmente puede haber miles de diferentes tipos
                    if (desc.IsList || desc.IsDictionary || desc.IsArray)
                    {
                        this.LocalTypeDescriptorMap[desc.Type] = desc;
                        this.Stack.RegisterAlias(desc);
                    }
                    else
                    {
                        this.GlobalTypeDescriptorMap[desc.Type] = desc;
                        this.LocalTypeDescriptorMap[desc.Type] = desc;

                        this.Stack.RegisterAlias(desc);

                        //registro los knowtyps
                        foreach (Type t in desc.IncludeTypes)
                            this.GetTypeDescriptor(t);
                    }

                    return desc;
                }
            }
            else
            {
                if (this.Settings.ThrowErrorWithNonSerializableTypes)
                    throw new XmlDataSerializerException("The type " + type.FullName + " is not market as serializable.");

                return null;
            }
        }

        public void RegisterType(TypeDescriptor desc)
        {
            if (desc.IsSerializable)
            {
                if (!this.GlobalTypeDescriptorMap.ContainsKey(desc.Type))
                    this.GlobalTypeDescriptorMap[desc.Type] = desc;

                if (!this.LocalTypeDescriptorMap.ContainsKey(desc.Type))
                {
                    this.LocalTypeDescriptorMap[desc.Type] = desc;
                    this.Stack.RegisterAlias(desc);

                    //registro los knowtyps
                    foreach (Type t in desc.IncludeTypes)
                        this.GetTypeDescriptor(t);
                }
            }
        }

        public void RegisterAlias(string alias, Type type)
        {
            this.Stack.RegisterAlias(alias, type);
        }

        #endregion


        public bool MustDeclareRootTypeName(Type entityType)
        {
            string alias = this.GetAlias(entityType);
            if (alias == null)
                return this.Settings.WriteRootTypeDefinition;
            else
                return false;
        }

        public bool IsRootElement(PropertyDescriptor property)
        {
            return this.Stack.InstanciesSequence.Count == 1;
/*
            if (property.Metadata.OwnerType != null && property.Metadata.OwnerType.GetGenericTypeDefinition() != null && property.Metadata.OwnerType.GetGenericTypeDefinition().Equals(_holderType))
                return true;
            else
                return false;
 */ 
        }
    }
}

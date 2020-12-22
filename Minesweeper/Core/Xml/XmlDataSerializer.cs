namespace Minesweeper.Core.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Runtime.Serialization;
    using Minesweeper.Core.Xml.Metadata;
    using System.Reflection;
    using Minesweeper.Core.Xml.Converters;
    using Minesweeper.Core.Helpers;

    public class XmlDataSerializer : IXmlContextData
    {
        #region -- Static --

        private static bool IsInitialized = false;
        private static object InitializationLocker = new object();

        private static void Initialize(XmlDataSerializer ser)
        {
            if (!IsInitialized)
            {
                lock (InitializationLocker)
                {
                    if (!IsInitialized)
                    {
                        XmlDataSerializer.AddConverter(new ConfigurationSectionConverter(), ser);
                        XmlDataSerializer.AddConverter(new IListConverter(), ser);
                        XmlDataSerializer.AddConverter(new IDictionaryConverter(), ser);
                        XmlDataSerializer.AddConverter(new ObjectConverter(), ser);

                        XmlDataSerializer.AddConverter(new NullConverter(), ser);
                        XmlDataSerializer.AddConverter(new StringConverter(), ser);
                        XmlDataSerializer.AddConverter(new ArrayConverter(), ser);
                        XmlDataSerializer.AddConverter(new TypeConverter(), ser);

                        XmlDataSerializer.AddConverter(new RealNumberConverter(), ser);
                        XmlDataSerializer.AddConverter(new DateTimeConverter(), ser);
                        XmlDataSerializer.AddConverter(new TimeSpanConverter(), ser);
                        XmlDataSerializer.AddConverter(new GuidConverter(), ser);
                        XmlDataSerializer.AddConverter(new EnumConverter(), ser);
                        XmlDataSerializer.AddConverter(new ValueTypeConverter(), ser);

                        IsInitialized = true;
                    }
                }
            }
        }

        private static void AddConverter(IXmlConverter converter, IXmlContextData prov)
        {
            converter.Register(prov);
        }

        #endregion

        #region -- Constructors --

        public XmlDataSerializer() : this(new XmlSerializerSettings())
        {

        }

        public XmlDataSerializer(XmlSerializerSettings settings)
        {
            this.Settings = settings;
            XmlDataSerializer.Initialize(this);
            
            this.TypeDescriptorMap = new Dictionary<Type,TypeDescriptor>();
            this.AliasMap = new Dictionary<string, List<Type>>();
        }

        #endregion

        public XmlSerializerSettings Settings { get; set; }

        public XmlSerializerContext Context
        {
            get;
            protected set;
        }

        #region -- Types --

        private Dictionary<Type, TypeDescriptor> TypeDescriptorMap { get; set; }

        public void RegisterType(Type type)
        {
            try
            {
                if (TypeHelper.IsSerializable(type) && !this.TypeDescriptorMap.ContainsKey(type))
                {
                    if (type.IsGenericType)
                        type = type.GetGenericTypeDefinition();

                    TypeDescriptor desc = new TypeDescriptor(type);
                    this.TypeDescriptorMap[type] = desc;

                    //registro los knowtyps
                    foreach (Type t in desc.IncludeTypes)
                        this.RegisterType(type);

                    //tengo que registrar el alias
                    if (!string.IsNullOrEmpty(desc.ElementName))
                        this.DoRegisterAlias(desc.ElementName, type);
                    else
                        this.DoRegisterAlias(desc.GetRootName(), type);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar registrar el tipo " + type.FullName, ex);
            }
        }

        #endregion


        #region -- Alias --

        private Dictionary<string, List<Type>> AliasMap { get; set; }

        public void RegisterAlias(string alias, Type type)
        {
            this.RegisterType(type);
            this.DoRegisterAlias(alias, type);
        }
        protected void DoRegisterAlias(string alias, Type type)
        {
            if (!this.AliasMap.ContainsKey(alias))
                this.AliasMap[alias] = new List<Type>();

            List<Type> types = this.AliasMap[alias];
            if (!types.Exists(d => d.Equals(type)))
                types.Add(type);
        }

        #endregion


        #region -- Serialization --

        public string Serialize(object entity)
        {
            return this.Serialize(entity, null);
        }

        public string Serialize(object entity, string rootElementName)
        {
            if (entity == null)
                return null;

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            if (this.Settings.FormatXmlOutput)
                writer.Formatting = Formatting.Indented;
            string output;
            try
            {
                this.Serialize(writer, entity, rootElementName);
                writer.Flush();
                output = stringWriter.ToString();
            }
            finally
            {
                stringWriter.Close();
                writer.Close();
            }
            return output;
        }


        public virtual void Serialize(XmlTextWriter writer, object entity)
        {
            this.Serialize(writer, entity, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="entity"></param>
        /// <param name="rootElementName"></param>
        /// <param name="declareRootType">Indica si debe resgitrar el tipo del objeto como atributo del elemento Root. Si es true, lo registra si no hay un alias definido, sino, no lo registra.</param>
        public virtual void Serialize(XmlTextWriter writer, object entity, string rootElementName)
        {
            if (entity == null)
                return;

            Type type = entity.GetType();
            if (!type.IsSerializable)
                throw new SerializationException("The entity " + type.Name + " is not marked as serializable.");

            XmlSerializerContext context = this.CreateContext();
            object holder = this.CreateObjectHolder(type, entity);
            TypeDescriptor holderDesc = new TypeDescriptor(holder.GetType());

            PropertyDescriptor desc = holderDesc.Properties[0];
            RootPropertyMetadata metadata;
            if (string.IsNullOrEmpty(rootElementName))
            {
                rootElementName = this.GetElementName(type, context);
                metadata = new RootPropertyMetadata(rootElementName, desc.Metadata.DefaultAttributeName, desc.Metadata);
            }
            else
            {
                context.RegisterAlias(rootElementName, entity.GetType());
                metadata = new RootPropertyMetadata(rootElementName, desc.Metadata.DefaultAttributeName, desc.Metadata);
            }
            
            desc.Metadata = metadata;
            desc.UseAlias = true;

            //podria agregar la instancia a serializa y tener un seguimiento de la secuencia.....pero para mas adelante
            this.Context = context;
            IXmlConverter converter = context.GetConverter(type);
            converter.ToXml(holder, desc, entity, writer, context);
        }

        #endregion


        #region -- Deserialization --

        public object DeserializeFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            if (!File.Exists(fileName))
                throw new ArgumentException("The file " + fileName + " not exist.");

            string xmlObject = File.ReadAllText(fileName);

            return this.Deserialize(xmlObject);
        }

        public object DeserializeFile(Type rootType, string rootElementName, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            if (!File.Exists(fileName))
                throw new ArgumentException("The file " + fileName + " not exist.");

            string xmlObject = File.ReadAllText(fileName);

            return this.Deserialize(rootType, rootElementName, xmlObject);
        }


        public object Deserialize(string xmlObject)
        {
            if (string.IsNullOrEmpty(xmlObject))
                return null;

            object output = null;
            StringReader stringReader = new StringReader(xmlObject);
            XmlTextReader reader = new XmlTextReader(stringReader);

            try
            {
                output = this.Deserialize(reader);
                return output;
            }
            finally
            {
                stringReader.Close();
                reader.Close();
            }
        }

        public object Deserialize(Type rootType, string xmlObject)
        {
            return this.Deserialize(rootType, null, xmlObject);
        }

        public object Deserialize(Type rootType, string rootElementName, string xmlObject)
        {
            if (string.IsNullOrEmpty(xmlObject))
                return null;

            object output = null;
            StringReader stringReader = new StringReader(xmlObject);
            XmlTextReader reader = new XmlTextReader(stringReader);

            try
            {
                output = this.Deserialize(rootType, rootElementName, reader);
                return output;
            }
            finally
            {
                stringReader.Close();
                reader.Close();
            }
        }

        public virtual object Deserialize(XmlReader reader)
        {
            return this.Deserialize(null, null, reader);
        }

        public virtual object Deserialize(Type rootType, XmlReader reader)
        {
            return this.Deserialize(rootType, null, reader);
        }

        public virtual object Deserialize(Type rootType, string rootElementName, XmlReader reader)
        {
            using (reader)
            {
                XmlNodeType typeNode = reader.MoveToContent();
                if (typeNode == XmlNodeType.Element || typeNode == XmlNodeType.EndElement)
                {
                    XmlSerializerContext context = this.CreateContext();

                    Type type = context.GetTypeFromAttribute(reader);
                    if (type == null)
                        type = context.GetTypeFromAlias(reader.LocalName);

                    if (type == null)
                    {
                        if (rootType != null)
                            type = rootType;
                        else
                            throw new Exception("No es posible obtener tipo del objeto a deserializar cuyo nombre de elemento raíz es " + reader.LocalName);
                    }

                    object holder = this.CreateObjectHolder(type, null);
                    TypeDescriptor holderDesc = new TypeDescriptor(holder.GetType());
                    if (holderDesc.Properties.Count == 0)
                        throw new SerializationException("The type is not marked as serializable.");

                    PropertyDescriptor desc = holderDesc.Properties[0];

                    RootPropertyMetadata metadata;

                    if (string.IsNullOrEmpty(rootElementName))
                        rootElementName = this.GetElementName(type, context);

                    metadata = new RootPropertyMetadata(rootElementName, desc.Metadata.DefaultAttributeName, desc.Metadata);

                    desc.Metadata = metadata;
                    desc.UseAlias = true;

                    this.Context = context;
                    IXmlConverter converter = context.GetConverter(type);

                    object output = converter.FromXml(holder, desc, type, reader, context);

                    return output;
                }
                else
                    return null;
            }
        }

        #endregion


        #region -- Varios --

        private string GetElementName(Type propertyEntityType, XmlSerializerContext context)
        {
            //obtengo el alias
            string alias = context.GetAlias(propertyEntityType);
            if (string.IsNullOrEmpty(alias))
            {
                TypeDescriptor itemTypeDesc = context.GetTypeDescriptor(propertyEntityType);
                return itemTypeDesc.GetRootName();
            }
            else
                return alias;
        }

        private object CreateObjectHolder(Type entityType, object entity)
        {
            Type holderType = typeof(ObjectHolder<>);
            holderType = holderType.MakeGenericType(entityType);
            return Activator.CreateInstance(holderType, entity);
        }

        private XmlSerializerContext CreateContext()
        {
            XmlSerializerContext context = new XmlSerializerContext();
            context.Settings = this.Settings;
            context.Stack.AliasMap = new Dictionary<string, List<Type>>();
            context.Stack.TypeDescriptorMap = new Dictionary<Type, TypeDescriptor>();

            foreach (KeyValuePair<string, List<Type>> alias in this.AliasMap)
            {
                List<Type> types = new List<Type>();
                types.AddRange(alias.Value);
                context.Stack.AliasMap.Add(alias.Key, types);
            }

            foreach (TypeDescriptor desc in this.TypeDescriptorMap.Values)
            {
                context.RegisterType(desc);
            }

            return context;
        }

        #endregion


        #region -- IConverterProvider --

        private static readonly Type __nullType = typeof(NullType);

        void IXmlContextData.RegisterConverter(Type type, IXmlConverter converter)
        {
            if (type == null)
                type = __nullType;

            //registro los converters globales....
            XmlSerializerContext.ConverterMap[type] = converter;
        }

        void IXmlContextData.RegisterAlias(string alias, Type type)
        {
            XmlSerializerContext.KnowAliasReverseMap[type] = alias;
            XmlSerializerContext.KnowAliasMap[alias] = type;
        }

        #endregion
    }
}

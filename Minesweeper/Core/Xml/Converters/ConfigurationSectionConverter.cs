using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Minesweeper.Core.Xml.Metadata;

namespace Minesweeper.Core.Xml.Converters
{
    public class ConfigurationSectionConverter : ConverterBase
    {
        private static readonly Type __configurationSectionType = typeof(ConfigurationSection);
        private static MethodInfo DeserializeMethod;
        private static MethodInfo SerializeMethod; 

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__configurationSectionType, this);
            DeserializeMethod = typeof(ConfigurationSection).GetMethod("DeserializeSection", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            SerializeMethod = typeof(ConfigurationSection).GetMethod("SerializeSection", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="metadata"></param>
        /// <param name="entityType"></param>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            long id = this.GetInstanceId(reader);
            object entity = Activator.CreateInstance(entityType);

            //escribo el nombre del elemento...
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(stringWriter);

            string attTypeName = context.Settings.TypeSettings.AttributeTypeName;

            //tengo que remover Obj-Type y Obj-Id atributos
            try
            {
                reader.MoveToContent();

                writer.WriteStartElement(reader.Name);

                //recorro todos los attributos y los escribo en el destino
                if (reader.MoveToFirstAttribute())
                {
                    if (string.Compare(reader.LocalName, attTypeName) != 0 &&
                        string.Compare(reader.LocalName, XmlSerializerSettings.ObjectIdAttributeName) != 0 &&
                        string.Compare(reader.LocalName, XmlSerializerSettings.ObjectReferenceAttributeName) != 0)
                        writer.WriteAttributeString(reader.LocalName, reader.Value);

                    while (reader.MoveToNextAttribute())
                    {
                        if (string.Compare(reader.LocalName, attTypeName) != 0 && 
                            string.Compare(reader.LocalName, XmlSerializerSettings.ObjectIdAttributeName) != 0 &&
                            string.Compare(reader.LocalName, XmlSerializerSettings.ObjectReferenceAttributeName) != 0)
                            writer.WriteAttributeString(reader.LocalName, reader.Value);
                    }
                }

                reader.MoveToContent();
                string str = reader.ReadInnerXml();
                if (!string.IsNullOrEmpty(str))
                    writer.WriteRaw(str);

                //cierro el tag
                writer.WriteEndElement();


            }
            finally
            {
                writer.Close();
                stringWriter.Close();
            }

            //creo un nuevo reader y se lo paso a la section
            string data = stringWriter.ToString();
            XmlTextReader newReader = new XmlTextReader(new StringReader(data));
            try
            {
                //MethodInfo deserializeMethod = entityType.GetMethod("DeserializeSection", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                DeserializeMethod.Invoke(entity, new object[] { newReader });
            }
            finally
            {
                newReader.Close();
            }

            //agrego la instancia al stack
            context.Stack.AddInstance(id, entity);

            return entity;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="metadata"></param>
        /// <param name="entity"></param>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        protected override void DoToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            ConfigurationSection section = (ConfigurationSection)entity;
            
            if (section == null)
                return;

            Type entityType = section.GetType();
            string elementName = metadata.GetElementNameForType(entityType, context, true);

            if (!context.Settings.UniqueSerializationForInstance || !context.Stack.ContainsInstance(entity))
            {
                //agrego la lista a las entidades registradas
                long id = context.Stack.AddInstance(entity);
                string valueStr = (string)SerializeMethod.Invoke(section, new object[] { null, elementName, ConfigurationSaveMode.Full });

                //escribo el nombre del elemento...
                writer.WriteStartElement(elementName);

                //recorro el contenido
                StringReader stringReader = new StringReader(valueStr);
                XmlTextReader reader = new XmlTextReader(stringReader);

                try
                {
                    reader.MoveToContent();
                    //recorro todos los attributos y los escribo en el destino
                    if (reader.MoveToFirstAttribute())
                    {
                        writer.WriteAttributeString(reader.LocalName, reader.Value);
                        while (reader.MoveToNextAttribute())
                            writer.WriteAttributeString(reader.LocalName, reader.Value);
                    }

                    //me fijo si hay que escribir el id
                    //escribo el id del objeto si corresponde
                    if (context.Settings.UniqueSerializationForInstance)
                        writer.WriteAttributeString(XmlSerializerSettings.ObjectIdAttributeName, id.ToString());

                    //agrego el tipo de la entidad como ultimo atributo
                    ObjectPropertyDescriptor descriptor = metadata.GetPropertyDescriptor<ObjectPropertyDescriptor>(entityType, context);
                    base.WriteTypeDefinition(descriptor, entityType, context, writer);

                    reader.MoveToContent();
                    string str = reader.ReadInnerXml();
                    if (!string.IsNullOrEmpty(str))
                        writer.WriteRaw(str);
                }
                finally
                {
                    stringReader.Close();
                    reader.Close();
                }

                //cierro el tag
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteStartElement(elementName);

                //me fijo si ya existe en el context
                long id = context.Stack.GetInstanceReferenceId(entity);
                writer.WriteAttributeString(XmlSerializerSettings.ObjectReferenceAttributeName, id.ToString());

                writer.WriteEndElement();
            }
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context)
        {
        }
    }
}

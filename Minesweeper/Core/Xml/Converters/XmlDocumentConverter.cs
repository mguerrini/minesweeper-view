namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;

    public class XmlDocumentConverter : ConverterBase
    {
        private static readonly Type __xmlType = typeof(XmlDocument);

        protected override object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            long id = this.GetInstanceId(reader);

            XmlDocument doc = new XmlDocument();
            string xml = reader.ReadOuterXml();
            doc.LoadXml(xml);

            //agrego la instancia al stack
            context.Stack.AddInstance(id, doc);

            return doc;
        }

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__xmlType, this);
            context.RegisterAlias("XmlDocument", __xmlType);
        }

        protected override void DoToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            XmlDocument doc = (XmlDocument)entity;

            if (!context.Settings.UniqueSerializationForInstance || !context.Stack.ContainsInstance(entity))
            {
                //agrego la lista a las entidades registradas
                long id = context.Stack.AddInstance(entity);
                
                string str = doc.DocumentElement.OuterXml;
                
                //al str le agrego id de la entidad
                XmlDocument doc2 = new XmlDocument();
                doc2.LoadXml(str);

                if (context.Settings.UniqueSerializationForInstance)
                {
                    XmlAttribute att = doc2.CreateAttribute(XmlSerializerSettings.ObjectIdAttributeName);
                    att.Value = id.ToString();
                    doc2.DocumentElement.Attributes.Append(att);
                }

                str = doc2.DocumentElement.OuterXml;
                writer.WriteRaw(str);
            }
            else
            {
                writer.WriteStartElement(doc.DocumentElement.Name);

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

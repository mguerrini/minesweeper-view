namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Minesweeper.Core.Xml.Metadata;
    using System.Xml;

    public class IXmlSerializableConverter : ConverterBase
    {
        private static readonly Type __type = typeof(IXmlSerializable);

        protected override object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            long id = this.GetInstanceId(reader);
            object entity = Activator.CreateInstance(entityType, true);
            IXmlSerializable ser = (IXmlSerializable)entity;
            ser.ReadXml(reader);

            //agrego la instancia al stack
            context.Stack.AddInstance(id, entity);

            return entity;
        }

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__type, this);
        }

        protected override void DoToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            IXmlSerializable ser = (IXmlSerializable)entity;

            string nodeName = metadata.GetElementNameForType(entity.GetType(), context, true);

            writer.WriteStartElement(nodeName);

            if (!context.Settings.UniqueSerializationForInstance || !context.Stack.ContainsInstance(entity))
            {
                //agrego la lista a las entidades registradas
                long id = context.Stack.AddInstance(entity);

                ser.WriteXml(writer);

                //escribo el id del objeto si corresponde
                if (context.Settings.UniqueSerializationForInstance)
                    writer.WriteAttributeString(XmlSerializerSettings.ObjectIdAttributeName, id.ToString());
            }
            else
            {
                //me fijo si ya existe en el context
                long id = context.Stack.GetInstanceReferenceId(entity);
                writer.WriteAttributeString(XmlSerializerSettings.ObjectReferenceAttributeName, id.ToString());
            }

            writer.WriteEndElement(); 
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context)
        {
        }
    }
}

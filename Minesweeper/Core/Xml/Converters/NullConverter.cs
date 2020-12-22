namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;

    internal class NullConverter : ConverterBase
    {
        private static readonly Type __type = typeof(NullType);

        protected override object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            reader.Read();
            return null;
        }

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__type, this);
            context.RegisterAlias("null", __type);
        }

        protected override void DoToXml(object parent, Metadata.PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            writer.WriteElementString("null", string.Empty);
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context)
        {
            writer.WriteElementString("null", string.Empty);
        }
    }
}


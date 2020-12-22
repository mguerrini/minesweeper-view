namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml;
    using Minesweeper.Core.Xml.Metadata;

    internal class NoopConverter : ConverterBase
    {
        private static NoopConverter instance = new NoopConverter();

        public static IXmlConverter Instance
        {
            get
            {
                return instance;
            }
        }


        private NoopConverter()
        {
        }


        protected override object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            throw new NotImplementedException();
        }

        public override void Register(IXmlContextData context)
        {
        }

        protected override void DoToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context)
        {
        }
    }
}


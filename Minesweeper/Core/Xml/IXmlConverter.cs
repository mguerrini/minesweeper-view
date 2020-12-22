namespace Minesweeper.Core.Xml
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;

    public interface IXmlConverter
    {
        object FromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context);

        void Register(IXmlContextData context);

        void ToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context);
    }
}


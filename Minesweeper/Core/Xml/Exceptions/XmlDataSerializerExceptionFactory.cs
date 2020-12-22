namespace Minesweeper.Core.Xml.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;


    public static class XmlDataSerializerExceptionFactory
    {
        internal static Exception CreateCanNotGetParseValueTypeException(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context, Exception inner)
        {
            string msg = "No es posible parsear el valor desde un string. Valor: {0}, Tipo del Valor: {1}, Tipo Entidad: {2}, Propiedad: {3}";
            object entity = context.Stack.InstanciesSequence.Peek();
            string entityType = "";
            if (entity != null)
                entityType = entity.GetType().FullName;

            return new XmlDataSerializerException(string.Format(msg, attValue, type.FullName, entityType, metadata.Metadata.PropertyName), inner);
        }

        internal static Exception CreateDeserializationException(string elementName, object parent, PropertyDescriptor metadata, Type entityType, XmlSerializerContext context, Exception inner)
        {
            string msg = "Se ha producido un error al deserializar la instancia de tipo {0}. Tipo del Owner: {1}, Propiedad: {2}";
            msg = string.Format(msg, entityType.FullName, parent.GetType().FullName, metadata.Metadata.PropertyName);
            return new XmlDataSerializerException(msg, inner);
        }

        internal static Exception CreateSerializationException(object parent, PropertyDescriptor metadata, object entity, XmlSerializerContext context, Exception inner)
        {
            string entityType = "NULL";
            if (entity != null)
                entityType = entity.GetType().FullName;

            string msg = "Se ha producido un error al serializar la instancia de tipo {0}. Tipo del Owner: {1}, Propiedad: {2}";
            msg = string.Format(msg, entityType, parent.GetType().FullName, metadata.Metadata.PropertyName);
            return new XmlDataSerializerException(msg, inner);
        }

        internal static Exception CreateCircularReferenceException(object parent, PropertyDescriptor metadata, object entity, XmlSerializerContext context)
        {
            string entityType = "NULL";
            if (entity != null)
                entityType = entity.GetType().FullName;

            string msg = "Referencia circular. La instancia de tipo {0} ya fue procesada anteriormente. Tipo del Owner: {1}, Propiedad: {2}";
            msg = string.Format(msg, entityType, parent.GetType().FullName, metadata.Metadata.PropertyName);
            return new XmlDataSerializerException(msg);
        }
    }
}

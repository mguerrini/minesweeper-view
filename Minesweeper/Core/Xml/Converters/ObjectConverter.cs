namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Minesweeper.Core.Xml.Metadata;
    using System.Xml;
    using System.Reflection;
    using System.Diagnostics;
    //using Minesweeper.Core.Log;

    public class ObjectConverter : ConverterBase
    {
        private static readonly Type __type = typeof(object);


        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__type, this);
        }


        #region -- From Xml --

        protected override object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            long id = this.GetInstanceId(reader);

            //obtengo el descriptor de la entidad
            TypeDescriptor entityTypeDescriptor = context.GetTypeDescriptor(entityType);

            if (entityTypeDescriptor != null)
            {
                //creo la instancia
                object entity = Activator.CreateInstance(entityType, true);

                //agrego la instancia al contexto
                context.Stack.AddInstance(id, entity);

                //deserializo los atributo
                this.DeserializeAttributes(entity, entityTypeDescriptor, reader, context);

                //deserializo los tag..
                //si es un elemento vacio, lo devuelvo
                if (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement)
                {
                    //avanzo
                    if (reader.IsEmptyElement)
                        reader.Read(); //avanzo para ubicarme en el siguiente nodo.
                }
                else
                {
                    this.DeserializeElements(entity, entityTypeDescriptor, reader, context);
                }

                return entity;
            }
            else
                reader.Skip();

            return null;
        }

        private void DeserializeElements(object parent, TypeDescriptor entityTypeDescriptor, XmlReader reader, XmlSerializerContext context)
        {
            string nodeName = reader.LocalName;

            //avanzo hasta la posicion del elemento
            bool end = !reader.Read();
            XmlNodeType typeNode = reader.MoveToContent();

            object association;
            PropertyDescriptor prop;

            //parseo las propiedades
            while (!end)
            {
                //me fijo si la propiedad es vacia
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    prop = entityTypeDescriptor.GetPropertyByElementName(reader.LocalName);

                    if (prop != null)
                    {
                        Type xmlType = base.GetEntityTypeForElement(prop, reader, context);

                        if (xmlType != null)
                        {
                            IXmlConverter converter = context.GetConverter(xmlType);
                            association = converter.FromXml(parent, prop, xmlType, reader, context);

                            prop.Metadata.SetValue(parent, association);
                        }
                        else
                            reader.Skip();
                    }
                    else
                    {
                        //DefaultLogger.Warning("The xml element {0} doesn't match with a property of the class {1}", reader.LocalName, parent.GetType().ToString());
                        reader.Skip();
                    }
                }

                typeNode = reader.MoveToContent();
                end = reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals(nodeName);
            }

            reader.Read();
        }

        private void DeserializeAttributes(object parent, TypeDescriptor entityTypeDescriptor, XmlReader reader, XmlSerializerContext context)
        {
            //avanzo hasta el primer atributo
            bool isAtt = reader.MoveToFirstAttribute();
            PropertyDescriptor prop;
            //si es un atributo, el tipo de la entidad coincide con el definido en la propiedad...

            string attName;
            object value;

            if (isAtt)
            {
                do
                {
                    attName = reader.Name;
                    prop = entityTypeDescriptor.GetPropertyByAttributeName(attName);

                    if (prop != null)
                    {
                        //obtengo el tipo a partir del atributo
                        Type propType = prop.GetTypeFromAttributeName(attName, context);
                    
                        IXmlConverter converter = context.GetConverter(propType);
                        value = converter.FromXml(parent, prop, propType, reader, context);
                        prop.Metadata.SetValue(parent, value);
                    }
                    else
                    {
                        //if (! (string.Compare(context.Settings.TypeSettings.AttributeTypeName, attName) == 0 
                        //    || string.Compare(XmlSerializerSettings.ObjectReferenceAttributeName, attName) == 0
                        //    || string.Compare(XmlSerializerSettings.ObjectIdAttributeName, attName) == 0))
                            //DefaultLogger.Warning("The xml attribute {0} doesn't match with an property of the class {1}", attName, parent.GetType().ToString());
                    }
                }

                while (reader.MoveToNextAttribute());
            }

            //me ubico de nuevo en el elemento
            reader.MoveToContent();
        }


        #endregion


        #region -- ToXml --

        protected override void DoToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            Type entityType = entity.GetType();
            TypeDescriptor entityTypeDescriptor = context.GetTypeDescriptor(entityType);

            //no es serializable...
            if (entityTypeDescriptor == null)
                return;
            
            ObjectPropertyDescriptor descriptor = metadata.GetPropertyDescriptor<ObjectPropertyDescriptor>(entityType, context);

            string elementName = metadata.GetElementNameForType(entityType, context, true);

            //escribo el nombre de la propiedad
            writer.WriteStartElement(elementName);

            if (!context.Settings.UniqueSerializationForInstance || !context.Stack.ContainsInstance(entity))
            {
                //agrego la lista a las entidades registradas
                long id = context.Stack.AddInstance(entity);

                //escribo las propiedades que son atributos
                this.WriteProperties(metadata, entity, entityTypeDescriptor, writer, context, true);

                //agrego el tipo de la entidad como ultimo atributo
                base.WriteTypeDefinition(descriptor, entityType, context, writer);

                //escribo el id del objeto
                if (context.Settings.UniqueSerializationForInstance)
                    writer.WriteAttributeString(XmlSerializerSettings.ObjectIdAttributeName, id.ToString());

                //escribo las propiedades que son elementos
                this.WriteProperties(metadata, entity, entityTypeDescriptor, writer, context, false);

            }
            else
            {
                //me fijo si ya existe en el context
                long id = context.Stack.GetInstanceReferenceId(entity);
                writer.WriteAttributeString(XmlSerializerSettings.ObjectReferenceAttributeName, id.ToString());
            }

            //escribo el nombre de la propiedad
            writer.WriteEndElement();
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context)
        {
        }

        private void WriteProperties(PropertyDescriptor metadata, object entity, TypeDescriptor entityDescriptor, XmlTextWriter writer, XmlSerializerContext context, bool writeAttribute)
        {
            IXmlConverter propConverter;
            PropertyDescriptor desc;
            object value;
            bool mustWrite;

            //agrego la propiedades
            for (int i = 0; i < entityDescriptor.Properties.Count; i++)
            {
                desc = entityDescriptor.Properties[i];
                value = desc.Metadata.GetValue(entity);

                if (value != null)
                {
                    Type entityType = value.GetType();

                    if (writeAttribute)
                        mustWrite = entityDescriptor.IsAttributeProperty(desc, entityType);
                    else
                        mustWrite = entityDescriptor.IsElementProperty(desc, entityType);

                    if (mustWrite)
                    {
                        propConverter = context.GetConverter(entityType);
                        propConverter.ToXml(entity, desc, value, writer, context);
                    }
                }
                else
                {
                    if (context.Settings.WriteNullValues)
                    {
                        if (entityDescriptor.CanWriteNullValue(desc.Metadata.PropertyName))
                        {
                            if (writeAttribute)
                                mustWrite = entityDescriptor.IsAttributeNullValueProperty(desc.Metadata.PropertyName);
                            else
                                mustWrite = !entityDescriptor.IsAttributeNullValueProperty(desc.Metadata.PropertyName);

                            if (mustWrite)
                            {
                                propConverter = context.GetConverter(desc.Metadata.PropertyType);
                                propConverter.ToXml(entity, desc, value, writer, context);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}

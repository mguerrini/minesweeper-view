namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using Minesweeper.Core.Xml.Metadata;
    using System.Xml;

    public class IDictionaryConverter : ConverterBase
    {
        private static readonly Type __type = typeof(IDictionary);

        protected override object DoFromXml(object parent, PropertyDescriptor propDesc, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            long id = this.GetInstanceId(reader);
            string nodeName = reader.LocalName;
            DictionaryPropertyDescriptor metadata = propDesc.GetPropertyDescriptor<DictionaryPropertyDescriptor>(entityType, context);

            IDictionary items = (IDictionary)Activator.CreateInstance(entityType, true);

            //recorro todos los items
            if (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement)
            {
                //avanzo
                if (reader.IsEmptyElement)
                    reader.Read(); //avanzo para ubicarme en el siguiente nodo.
            }
            else
            {
                this.DeserializeElements(items, metadata, reader, context);
            }

            //agrego el diccionario al stack
            context.Stack.AddInstance(id, items);

            //creo un array
            return items;
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context)
        {
        }

        #region -- Deserialize --

        private void DeserializeElements(IDictionary parent, DictionaryPropertyDescriptor propDesc, XmlReader reader, XmlSerializerContext context)
        {
            //avanzo hasta la posicion del elemento
            string nodeName = reader.LocalName;

            //avanzo hasta el primer nodo
            bool end = !reader.Read();
            XmlNodeType typeNode = reader.MoveToContent();

            DictionaryKeyValuePropertyDescriptor keyDesc = propDesc.GetDictionaryItemKeyPropertyDescriptor(context);
            DictionaryKeyValuePropertyDescriptor valueDesc = propDesc.GetDictionaryItemValuePropertyDescriptor(context);

            //parseo las propiedades
            while (!end)
            {

                //me fijo si la propiedad es vacia
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    //tengo que leer el item del diccionario
                    this.DeserializeDictionaryItem(parent, keyDesc, valueDesc, reader, context);
                }

                
                reader.Read();
                //avanzo...
                typeNode = reader.MoveToContent();
                end = reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals(nodeName);
            }

            reader.Read();
        }

        private void DeserializeDictionaryItem(IDictionary parent, DictionaryKeyValuePropertyDescriptor keyDesc, DictionaryKeyValuePropertyDescriptor valueDesc, XmlReader reader, XmlSerializerContext context)
        {
            //avanzo hasta la posicion del elemento
            string nodeName = reader.LocalName;

            //avanzo hasta el primer nodo
            //bool end = !reader.Read();
            XmlNodeType typeNode = reader.MoveToContent();

            object key;
            object value;

            //parseo las propiedades
            //me fijo si la propiedad es vacia
            if (reader.NodeType != XmlNodeType.EndElement)
            {
                //me fijo si el key y el value son atributos
                this.DeserializeKeyValueAtttributes(parent, keyDesc, valueDesc, reader, context, out key, out value);

                //si son elementos..
                if (key == null || value == null)
                    this.DeserializeKeyValueElements(parent, keyDesc, valueDesc, reader, context, ref key, ref value);

                //agrego el item al diccionario
                if (key != null)
                    parent.Add(key, value);
            }
        }

        private void DeserializeKeyValueAtttributes(IDictionary parent, DictionaryKeyValuePropertyDescriptor keyDesc, DictionaryKeyValuePropertyDescriptor valueDesc, XmlReader reader, XmlSerializerContext context, out object key, out object value)
        {
            string attName;
            Type keyType;
            Type valueType;
            key = null;
            value = null;

            if (reader.MoveToFirstAttribute())
            {
                attName = reader.Name;
                keyType = keyDesc.GetTypeFromAttributeName(attName, context);
                
                if (keyType == null)
                {
                    valueType = valueDesc.GetTypeFromAttributeName(attName, context);
                    if (valueType != null)
                    {
                        IXmlConverter converter = context.GetConverter(valueType);
                        value = converter.FromXml(parent, valueDesc, valueType, reader, context);
                    }
                }
                else
                {
                    IXmlConverter converter = context.GetConverter(keyType);
                    key = converter.FromXml(parent, keyDesc, keyType, reader, context);
                }

                if (reader.MoveToNextAttribute())
                {
                    attName = reader.Name;
                    keyType = keyDesc.GetTypeFromAttributeName(attName, context);

                    if (keyType == null)
                    {
                        valueType = valueDesc.GetTypeFromAttributeName(attName, context);
                        if (valueType != null)
                        {
                            IXmlConverter converter = context.GetConverter(valueType);
                            value = converter.FromXml(parent, valueDesc, valueType, reader, context);
                        }
                    }
                    else
                    {
                        IXmlConverter converter = context.GetConverter(keyType);
                        key = converter.FromXml(parent, keyDesc, keyType, reader, context);
                    }
                }
            }

            //me ubico en el elemento..
            reader.MoveToContent();
        }

        private void DeserializeKeyValueElements(IDictionary parent, DictionaryKeyValuePropertyDescriptor keyDesc, DictionaryKeyValuePropertyDescriptor valueDesc, XmlReader reader, XmlSerializerContext context, ref object key, ref object value)
        {
            string nodeName = reader.LocalName;

            //avanzo hasta la posicion del elemento
            bool end = !reader.Read();
            XmlNodeType typeNode = reader.MoveToContent();
            string elementName;
            Type keyType;
            Type valueType;

            //parseo las propiedades
            while (!end)
            {
                //me fijo si la propiedad es vacia
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    elementName = reader.LocalName;
                    keyType = keyDesc.GetTypeFromElementName(elementName, context);

                    if (keyType == null)
                    {
                        valueType = valueDesc.GetTypeFromElementName(elementName, context);
                        if (valueType != null)
                        {
                            IXmlConverter converter = context.GetConverter(valueType);
                            value = converter.FromXml(parent, valueDesc, valueType, reader, context);
                        }
                    }
                    else
                    {
                        IXmlConverter converter = context.GetConverter(keyType);
                        key = converter.FromXml(parent, keyDesc, keyType, reader, context);
                    }

                    //ya esta parado sobre el siguiente elemento
                    typeNode = reader.MoveToContent();
                    if (reader.NodeType != XmlNodeType.EndElement)
                    {
                        elementName = reader.LocalName;
                        keyType = keyDesc.GetTypeFromElementName(elementName, context);

                        if (keyType == null)
                        {
                            valueType = valueDesc.GetTypeFromElementName(elementName, context);
                            if (valueType != null)
                            {
                                IXmlConverter converter = context.GetConverter(valueType);
                                value = converter.FromXml(parent, valueDesc, valueType, reader, context);
                            }
                        }
                        else
                        {
                            IXmlConverter converter = context.GetConverter(keyType);
                            key = converter.FromXml(parent, keyDesc, keyType, reader, context);
                        }
                    }
                }

                typeNode = reader.MoveToContent();
                end = reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals(nodeName);
            }
        }

        #endregion


        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__type, this);
            context.RegisterAlias("Dictionary", __type);
        }

        protected override void DoToXml(object parent, PropertyDescriptor propDesc, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            Type entityType = entity.GetType();
            DictionaryPropertyDescriptor metadata = propDesc.GetPropertyDescriptor<DictionaryPropertyDescriptor>(entityType, context);

            string nodeName = metadata.GetElementNameForType(entityType, context, true);

            writer.WriteStartElement(nodeName);

            if (!context.Settings.UniqueSerializationForInstance || !context.Stack.ContainsInstance(entity))
            {
                //agrego la lista a las entidades registradas
                long id = context.Stack.AddInstance(entity);

                base.WriteTypeDefinition(metadata, entityType, context, writer);

                //escribo el id del objeto si corresponde
                if (context.Settings.UniqueSerializationForInstance)
                    writer.WriteAttributeString(XmlSerializerSettings.ObjectIdAttributeName, id.ToString());

                IDictionary dic = (IDictionary)entity;

                DictionaryKeyValuePropertyDescriptor keyDesc = metadata.GetDictionaryItemKeyPropertyDescriptor(context);
                DictionaryKeyValuePropertyDescriptor valueDesc = metadata.GetDictionaryItemValuePropertyDescriptor(context);

                foreach (DictionaryEntry item in dic)
                    this.SerializeDictionaryItem(dic, item, metadata, keyDesc, valueDesc, writer, context);
            }
            else
            {
                //me fijo si ya existe en el context
                long id = context.Stack.GetInstanceReferenceId(entity);
                writer.WriteAttributeString(XmlSerializerSettings.ObjectReferenceAttributeName, id.ToString());
            }

            writer.WriteEndElement();
         }

        #region -- Serialize --

        private void SerializeDictionaryItem(IDictionary dic, DictionaryEntry entry, DictionaryPropertyDescriptor propDesc, DictionaryKeyValuePropertyDescriptor keyDesc, DictionaryKeyValuePropertyDescriptor valueDesc, XmlTextWriter writer, XmlSerializerContext context)
        {
            //escribo el item
            string itemName = propDesc.GetElementNameForDictionaryItem(context);
            object key = entry.Key;
            object value = entry.Value;
            IXmlConverter converter;

            if (key != null)
            {
                writer.WriteStartElement(itemName);

                Type keyType = key.GetType();

                if (keyDesc.IsXmlAttribute(keyType, context))
                {
                    converter = context.GetConverter(keyType);
                    converter.ToXml(dic, keyDesc, key, writer, context);
                }

                if (value != null)
                {
                    Type valueType = value.GetType();
                    if (valueDesc.IsXmlAttribute(valueType, context))
                    {
                        converter = context.GetConverter(valueType);
                        converter.ToXml(dic, valueDesc, value, writer, context);
                    }
                }

                //escribo los elementos
                if (keyDesc.IsXmlElement(keyType, context))
                {
                    converter = context.GetConverter(keyType);
                    converter.ToXml(dic, keyDesc, key, writer, context);
                }

                if (value != null)
                {
                    Type valueType = value.GetType();
                    if (valueDesc.IsXmlElement(valueType, context))
                    {
                        converter = context.GetConverter(valueType);
                        converter.ToXml(dic, valueDesc, value, writer, context);
                    }
                }

                writer.WriteEndElement();
            }
        }

        #endregion

    }
}

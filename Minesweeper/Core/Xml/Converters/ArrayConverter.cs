namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Minesweeper.Core.Xml.Exceptions;
    using Minesweeper.Core.Helpers;

    public class ArrayConverter : ConverterBase
    {
        private static readonly Type __arrayType = typeof(Array);

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__arrayType, this);
            context.RegisterAlias("Array", __arrayType);

        }


        #region -- From Xml --

        public override object FromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            string elementName = reader.LocalName;

            if (this.MustExcludeElement(elementName, context) || !this.MustIncludeElement(elementName, context))
            {
                reader.Skip();
                return null;
            }

            try
            {
                context.Stack.InstanciesSequence.Push(parent);
                return this.DoFromXml(parent, metadata, entityType, reader, context);
            }
            catch (Exception ex)
            {
                throw XmlDataSerializerExceptionFactory.CreateDeserializationException(elementName, parent, metadata, entityType, context, ex);
            }
            finally
            {
                context.Stack.InstanciesSequence.Pop();
            }
        }

        protected override object DoFromXml(object parent, PropertyDescriptor propDesc, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            //string nodeName = reader.LocalName;
            ListPropertyDescriptor metadata = propDesc.GetPropertyDescriptor<ListPropertyDescriptor>(entityType, context);

            IList items;
            //creo una lista para luego transformarla en un array....si es una lista 
            if (entityType.IsArray)
            {
                items = new ArrayList();
            }
            else if (entityType.IsInterface)
            {
                if (entityType.IsGenericType)
                {
                    Type entityItemType = entityType.GetGenericArguments()[0];
                    Type listType = typeof(List<>);
                    listType = listType.MakeGenericType(entityItemType);
                    items = (IList)Activator.CreateInstance(listType, true);
                }
                else
                {
                    items = new ArrayList();
                }
            }
            else
                items = (IList)Activator.CreateInstance(entityType, true);

            Type itemType;
            long id = 0;

            if (metadata.IsInlineAttributeArray(entityType, context))
            {
                itemType = this.DeserializeItemsFromInlineAttribute(entityType, items, metadata, reader, context);
            }
            else
            {
                //si no es inline el array, entonces tiene un id
                id = this.GetInstanceId(reader);

                //recorro todos los items
                if (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement)
                {
                    //avanzo
                    if (reader.IsEmptyElement)
                        reader.Read(); //avanzo para ubicarme en el siguiente nodo.

                    itemType = metadata.DeclaringItemType;
                }
                else
                {
                    if (metadata.IsInlineElementArray(entityType, context))
                        itemType = this.DeserializeItemsFromInlineElements(items, metadata, reader, context);
                    else
                        
                        itemType = this.DeserializeItemsFromElements(entityType, items, metadata, reader, context);
                }
            }

            IList output;

            //creo un array
            if (entityType.IsArray)
                output = this.CreateOutputFrom(metadata, items, entityType.GetElementType());
            else
                output = this.CreateOutputFrom(metadata, items, itemType);

            //agrego la instancia al stack
            context.Stack.AddInstance(id, output);

            return output;
        }

        protected virtual IList CreateOutputFrom(ListPropertyDescriptor metadata, IList items, Type itemType)
        {
            Array arrayItems = Array.CreateInstance(itemType, items.Count);

            for (int i = 0; i < items.Count; i++)
                arrayItems.SetValue(items[i], i);

            return arrayItems;
        }

        private Type DeserializeItemsFromElements(Type entityType, IList parent, ListPropertyDescriptor metadata, XmlReader reader, XmlSerializerContext context)
        {
            ListItemPropertyDescriptor propDesc = metadata.GetItemPropertyDescriptor(context, false);

            //avanzo hasta la posicion del elemento
            string nodeName = reader.LocalName;
            
            //avanzo hasta el primer nodo
            bool end = !reader.Read();
            XmlNodeType typeNode = reader.MoveToContent();

            object association;
            Type itemType = null;

            //parseo las propiedades
            while (!end)
            {
                //me fijo si la propiedad es vacia
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    //obtengo el tipo por el atributo
                    itemType = base.GetEntityTypeForElement(propDesc, reader, context);

                    if (itemType != null)
                    {
                        IXmlConverter converter = context.GetConverter(itemType);
                        association = converter.FromXml(parent, propDesc, itemType, reader, context);

                        if (association != null || context.Settings.AddNullValueInLists)
                            //agrego el item a la lista
                            parent.Add(association);
                    }
                    else
                        reader.Skip();
                }

                //avanzo...
                typeNode = reader.MoveToContent();
                end = reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals(nodeName);
            }

            reader.Read();

            if (entityType.IsArray && itemType != null)
                return itemType;
            else
                return metadata.DeclaringItemType;
        }

        private Type DeserializeItemsFromInlineElements(IList parent, ListPropertyDescriptor metadata, XmlReader reader, XmlSerializerContext context)
        {
            //avanzo hasta la posicion del elemento
            string nodeName = reader.LocalName;
            Type itemType = metadata.DeclaringItemType;

            if (reader.NodeType != XmlNodeType.EndElement)
            {
                //obtengo el tipo por el atributo
                ListItemPropertyDescriptor propDesc = metadata.GetItemPropertyDescriptor(context, true);
                itemType = base.GetEntityTypeForElement(propDesc, reader, context);

                if (itemType != null)
                {
                    //avanzo hasta el primer nodo
                    if (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                    {
                        XmlNodeType typeNode = reader.MoveToContent();

                        string val = reader.Value;

                        this.DeserializeItems(parent.GetType(), itemType, parent, metadata, propDesc, val, context, false);

                        reader.Read();
                    }
                }
                else
                    reader.Skip();
            }

            reader.Read();

            return itemType;
        }

        private Type DeserializeItemsFromInlineAttribute(Type entityType, IList parent, ListPropertyDescriptor metadata, XmlReader reader, XmlSerializerContext context)
        {
            //avanzo hasta la posicion del elemento
            string attributeName = reader.Name;
            string value = reader.Value;
            
            //obtengo el tipo por el atributo
            ListItemPropertyDescriptor propDesc = metadata.GetItemPropertyDescriptor(context, true);
            Type itemType = propDesc.GetTypeFromAttributeName(attributeName, context);

            if (!string.IsNullOrEmpty(value))
                this.DeserializeItems(entityType, itemType, parent, metadata, propDesc, value, context, true);

            return itemType;
        }

        private void DeserializeItems(Type entityType, Type itemType, IList parent, ListPropertyDescriptor metadata, ListItemPropertyDescriptor propDesc, string itemsStr, XmlSerializerContext context, bool isAttribute)
        {
            itemsStr = itemsStr.Trim();

            if (!TypeHelper.IsValueType(itemType))
                throw new Exception("El item de tipo " + itemType.Name + " no es de tipo ValueType, no puede deserializarse de un atributo.");

            ValueTypeConverter itemConverter = (ValueTypeConverter)context.GetConverter(itemType);
            
            //obtengo el tipo por el atributo
            ValueTypePropertyDescriptor itemDescriptor = propDesc.GetPropertyDescriptor<ValueTypePropertyDescriptor>(itemType, context);

            string itemSeparator = metadata.GetInlineItemSeparator(entityType, context, isAttribute);

            string[] items = itemsStr.Split(new string[] { itemSeparator }, StringSplitOptions.None);
            int last = items.Length;
            
            if (string.Compare(itemsStr[itemsStr.Length - 1].ToString(), itemSeparator) == 0)
                last--;

            object val;

            for (int i=0; i<last; i++)
            {
                string item = items[i];
                val = itemConverter.GetValueFromString(itemDescriptor, item, itemType, context);
                parent.Add(val);
            }
        }

        #endregion


        #region -- ToXml --

        public override void ToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            try
            {
                if (entity != null)
                {
                    //verifico si existe una referencia circular
                    if (!context.Settings.UniqueSerializationForInstance && context.Stack.ExistInSequence(entity))
                    {
                        context.Stack.InstanciesSequence.Push(entity);
                        throw XmlDataSerializerExceptionFactory.CreateCircularReferenceException(parent, metadata, entity, context);
                    }
                    else
                        context.Stack.InstanciesSequence.Push(entity);

                    if (!this.MustExcludeSerializationProperty(parent, metadata, entity, context))
                        this.DoToXml(parent, metadata, entity, writer, context);
                }
                else
                {
                    this.DoToNullValueXml(parent, metadata, writer, context);
                }
            }
            catch (Exception ex)
            {
                throw XmlDataSerializerExceptionFactory.CreateSerializationException(parent, metadata, entity, context, ex);
            }
            finally
            {
                if (entity !=null)
                    context.Stack.InstanciesSequence.Pop();
            }
        }

        protected override void DoToXml(object parent, PropertyDescriptor propDesc, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            Type entityType = entity.GetType();
            ListPropertyDescriptor metadata = propDesc.GetPropertyDescriptor<ListPropertyDescriptor>(entityType, context);

            if (metadata.IsInlineAttributeArray(entityType, context))
                this.WriteInlineArrayAsAttribute(parent, metadata, entity, entityType, writer, context);
            else
            {
                //deberia validar si la lista ya se agrego....y colocar una referencia..
                if (metadata.IsInlineElementArray(entityType, context))
                    this.WriteInlineArrayAsElement(parent, metadata, entity, entityType, writer, context);
                else
                    this.WriteArrayAsElement(parent, metadata, entity, entityType, writer, context);
            }
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor propDesc, XmlTextWriter writer, XmlSerializerContext context)
        {
            //no escribe nada
        }

        private void WriteInlineArrayAsAttribute(object parent, ListPropertyDescriptor metadata, object entity, Type entityType, XmlTextWriter writer, XmlSerializerContext context)
        {
            IEnumerable list = (IEnumerable)entity;

            if (!context.Settings.WriteEmptyLists)
            {
                if (!list.GetEnumerator().MoveNext())
                {
                    return;
                }
            }

            string value = this.GetInlineStringArray(metadata, list, entityType, context, true);
            string attributeName = metadata.GetAttributeNameForType(entityType, context);

            //esta definido el atributo, lo escribo
            writer.WriteAttributeString(attributeName, value);
        }

        private void WriteArrayAsElement(object parent, ListPropertyDescriptor metadata, object entity, Type entityType, XmlTextWriter writer, XmlSerializerContext context)
        {
            if (!context.Settings.WriteEmptyLists)
            {
                IEnumerable list = (IEnumerable)entity;
                if (!list.GetEnumerator().MoveNext())
                {
                    return;
                }
            }

            string nodeName = metadata.GetElementNameForType(entityType, context, true);

            //escribo el inicio del nodo
            writer.WriteStartElement(nodeName);

            if (!context.Settings.UniqueSerializationForInstance || !context.Stack.ContainsInstance(entity))
            {
                //agrego la lista a las entidades registradas
                long id = context.Stack.AddInstance(entity);

                //escribo el tipo, si corresponde
                base.WriteTypeDefinition(metadata, entityType, context, writer);

                //escribo el id del objeto si corresponde
                if (context.Settings.UniqueSerializationForInstance)
                    writer.WriteAttributeString(XmlSerializerSettings.ObjectIdAttributeName, id.ToString());

                IEnumerable list = (IEnumerable)entity;
                Type itemType;
                IXmlConverter itemConverter;

                //recorro todos los items
                foreach (object item in list)
                {
                    if (item != null)
                    {
                        itemType = item.GetType();
                        itemConverter = context.GetConverter(itemType);
                        ListItemPropertyDescriptor descriptor = metadata.GetItemPropertyDescriptor(context, false);
                        itemConverter.ToXml(entity, descriptor, item, writer, context);
                    }
                    else
                    {
                        itemConverter = context.GetConverter(null);
                        itemConverter.ToXml(entity, metadata, null, writer, context);
                    }
                }
            }
            else
            {
                //me fijo si ya existe en el context
                long id = context.Stack.GetInstanceReferenceId(entity);
                writer.WriteAttributeString(XmlSerializerSettings.ObjectReferenceAttributeName, id.ToString());
            }

            writer.WriteEndElement();
        }

        private void WriteInlineArrayAsElement(object parent, ListPropertyDescriptor metadata, object entity, Type entityType, XmlTextWriter writer, XmlSerializerContext context)
        {
            IEnumerable list = (IEnumerable)entity;

            if (!context.Settings.WriteEmptyLists)
            {
                if (!list.GetEnumerator().MoveNext())
                {
                    return;
                }
            }

            string nodeName = metadata.GetElementNameForType(entityType, context, true);

            //escribo el inicio del nodo
            writer.WriteStartElement(nodeName);

            if (!context.Stack.ContainsInstance(entity))
            {
                //agrego la lista a las entidades registradas
                long id = context.Stack.AddInstance(entity);

                //escribo el tipo, si corresponde
                base.WriteTypeDefinition(metadata, entityType, context, writer);

                //escribo el id del objeto si corresponde
                if (context.Settings.UniqueSerializationForInstance)
                    writer.WriteAttributeString(XmlSerializerSettings.ObjectIdAttributeName, id.ToString());

                list = (IEnumerable)entity;
                string value = this.GetInlineStringArray(metadata, list, entityType, context, false);
                
                //escribo el contenido del array
                writer.WriteString(value);
            }
            else
            {
                //me fijo si ya existe en el context
                long id = context.Stack.GetInstanceReferenceId(entity);
                writer.WriteAttributeString(XmlSerializerSettings.ObjectReferenceAttributeName, id.ToString());
            }

            writer.WriteEndElement();
        }

        private string GetInlineStringArray(ListPropertyDescriptor metadata, IEnumerable list, Type entityType, XmlSerializerContext context, bool isAttribute)
        {
            Type itemType = null;
            Type currentType = null;

            ValueTypeConverter itemConverter = null;
            StringBuilder attBuilder = new StringBuilder();
            ValueTypePropertyDescriptor valPropDesc = null;

            string itemSeparator = metadata.GetInlineItemSeparator(entityType, context, isAttribute);
            bool first = true;

            foreach (object item in list)
            {
                if (item != null)
                {
                    if (itemType == null)
                    {
                        itemType = item.GetType();

                        if (!TypeHelper.IsValueType(itemType))
                            throw new Exception("El item de tipo " + itemType.Name + " no es de tipo ValueType, no puede serializarse en un atributo.");

                        itemConverter = (ValueTypeConverter)context.GetConverter(itemType);
                        ListItemPropertyDescriptor itemDesc = metadata.GetItemPropertyDescriptor(context, true);
                        valPropDesc = itemDesc.GetPropertyDescriptor<ValueTypePropertyDescriptor>(itemType, context);
                    }

                    currentType = item.GetType();

                    if (!currentType.Equals(itemType))
                        throw new Exception("El item de tipo " + currentType.Name + " es direferente al primer item del array de tipo " + itemType.Name + ". No es posible serializar un array de diferentes tipos en un atributo.");

                    string val = itemConverter.GetValueAsString(valPropDesc, item, itemType, context);

                    if (first)
                        first = false;
                    else
                        attBuilder.Append(itemSeparator);

                    attBuilder.Append(val);
                }
            }

            return attBuilder.ToString();
        }

        #endregion


    }
}


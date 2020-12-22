namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;
    using System.IO;
    using Minesweeper.Core.Helpers;

    internal class StringConverter : ValueTypeConverter
    {
        private static readonly Type __stringType = typeof(string);

        protected override object DoFromXml(object parent, PropertyDescriptor propDesc, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            ValueTypePropertyDescriptor metadata = propDesc.GetPropertyDescriptor < ValueTypePropertyDescriptor>(entityType, context);
            if (metadata.IsXmlContentText)
            {
                //tengo que leer todo el contenido...
                return string.Intern(reader.ReadOuterXml());
            }
            else
            {
                return base.DoFromXml(parent, metadata, entityType, reader, context);
            }
        }

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__stringType, this);
            context.RegisterAlias("String", __stringType);
        }

        protected override void DoToXml(object parent, PropertyDescriptor propDescritor, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            Type entityType = entity.GetType();
            ValueTypePropertyDescriptor metadata = propDescritor.GetPropertyDescriptor<ValueTypePropertyDescriptor>(entityType, context);
            if (metadata.IsXmlContentText)
            {
                if (!TypeHelper.IsDefaultValue(entity, entityType) || context.Settings.WriteDefaultValues)
                {
                    string valueStr = this.GetValueAsString(metadata, entity, entityType, context);
                    string elementName = metadata.GetElementNameForType(entityType, context, true);

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

                        reader.MoveToContent();
                        string str = reader.ReadInnerXml();
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
            }
            else
            {
                base.DoToXml(parent, metadata, entity, writer, context);
            }
         }



        public override string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            //le agrego los cdata si correspnode..
            
            string data = attValue.ToString();
            bool appendCData = false;
            
            int index = data.IndexOf('<');
            
            if (index > 0)
            {
                appendCData = true;
            }
            else
            {
                index = data.IndexOf('>');
                appendCData = index > 0;
            }

            if (appendCData)
                return "<![CDATA[" + data + "]]>";
            else
                return data;
        }

        protected override object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            return attValue;
        }

        protected override object GetDefaultValue(ValueTypePropertyDescriptor metadata, Type type, XmlSerializerContext context)
        {
            return string.Empty;
        }
    }
}


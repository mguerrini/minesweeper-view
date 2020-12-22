namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Minesweeper.Core.Xml.Metadata;
    using System.Xml;
    using System.Globalization;
    using Minesweeper.Core.Xml.Exceptions;
    using Minesweeper.Core.Helpers;

    public class ValueTypeConverter : ConverterBase
    {
        private static readonly Type _byte = typeof(byte);
        private static readonly Type _sbyte = typeof(sbyte);
        private static readonly Type _short = typeof(short);
        private static readonly Type _int = typeof(int);
        private static readonly Type _long = typeof(long);
        private static readonly Type _ushort = typeof(ushort);
        private static readonly Type _uint = typeof(uint);
        private static readonly Type _ulong = typeof(ulong);
        private static readonly Type _bool = typeof(bool);
        private static readonly Type _char = typeof(char);

        private static readonly Type _nbyte = typeof(byte?);
        private static readonly Type _nsbyte = typeof(sbyte?);
        private static readonly Type _nshort = typeof(short?);
        private static readonly Type _nint = typeof(int?);
        private static readonly Type _nlong = typeof(long?);
        private static readonly Type _nushort = typeof(ushort?);
        private static readonly Type _nuint = typeof(uint?);
        private static readonly Type _nulong = typeof(ulong?);
        private static readonly Type _nbool = typeof(bool?);
        private static readonly Type _nchar = typeof(char?);


        protected override object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            string attributeName = null;
            string elementName = null;
            string val = null;
            object output = null;
            ValueTypePropertyDescriptor valPropDesc = metadata.GetPropertyDescriptor<ValueTypePropertyDescriptor>(entityType, context);

            //si el reader esta parado sobre un atributo.....entonces, el tipo lo obtengo a partir del atributo
            if (reader.NodeType == XmlNodeType.Attribute)
                attributeName = reader.Name;
            else
            {
                elementName = reader.LocalName;
                attributeName = valPropDesc.GetAttributeNameForType(entityType, context);
            }

            if (string.IsNullOrEmpty(elementName))
            {
                //esta definido en el atributo, lo escribo
                string propAttName = valPropDesc.GetAttributeNameForType(entityType, context);

                if (string.Compare(attributeName, propAttName) == 0)
                {
                    val = reader.Value;
                    output = this.GetValueFromString(valPropDesc.GetPropertyDescriptor<ValueTypePropertyDescriptor>(entityType, context), val, entityType, context);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(attributeName))
                    val = reader.ReadElementContentAsString();
                else
                {
                    if (reader.MoveToAttribute(attributeName))
                    {
                        //avanzo el elemento
                        val = reader.Value;
                        reader.Read();
                    }
                    else
                    {
                        //esta en el contenido del elemento
                        val = reader.ReadElementContentAsString();
                    }
                }

                output = this.GetValueFromString(valPropDesc, val, entityType, context);
            }

            return output;
        }

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(typeof(bool), this);
            context.RegisterConverter(typeof(byte), this);
            context.RegisterConverter(typeof(ushort), this);
            context.RegisterConverter(typeof(short), this);
            context.RegisterConverter(typeof(uint), this);
            context.RegisterConverter(typeof(int), this);
            context.RegisterConverter(typeof(ulong), this);
            context.RegisterConverter(typeof(long), this);
            context.RegisterConverter(typeof(char), this);

            context.RegisterConverter(typeof(bool?), this);
            context.RegisterConverter(typeof(byte?), this);
            context.RegisterConverter(typeof(ushort?), this);
            context.RegisterConverter(typeof(short?), this);
            context.RegisterConverter(typeof(uint?), this);
            context.RegisterConverter(typeof(int?), this);
            context.RegisterConverter(typeof(ulong?), this);
            context.RegisterConverter(typeof(long?), this);
            context.RegisterConverter(typeof(char?), this);

            context.RegisterAlias("Bool", typeof(bool));
            context.RegisterAlias("Byte", typeof(byte));
            context.RegisterAlias("UShort", typeof(ushort));
            context.RegisterAlias("Short", typeof(short));
            context.RegisterAlias("UInt", typeof(uint));
            context.RegisterAlias("Int", typeof(int));
            context.RegisterAlias("ULong", typeof(ulong));
            context.RegisterAlias("Long", typeof(long));
            context.RegisterAlias("Char", typeof(char));

            context.RegisterAlias("NullableBool", typeof(bool?));
            context.RegisterAlias("NullableByte", typeof(byte?));
            context.RegisterAlias("NullableUShort", typeof(ushort?));
            context.RegisterAlias("NullableShort", typeof(short?));
            context.RegisterAlias("NullableUInt", typeof(uint?));
            context.RegisterAlias("NullableInt", typeof(int?));
            context.RegisterAlias("NullableULong", typeof(ulong?));
            context.RegisterAlias("NullableLong", typeof(long?));
            context.RegisterAlias("NullableChar", typeof(char?));
        }

        protected override void DoToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            Type entityType = entity.GetType();

            if (!TypeHelper.IsDefaultValue(entity, entityType) || context.Settings.WriteDefaultValues)
            {
                ValueTypePropertyDescriptor desc = metadata.GetPropertyDescriptor<ValueTypePropertyDescriptor>(entityType, context);
                string valueStr = this.GetValueAsString(desc, entity, entityType, context);

                if (!desc.IsXmlElement(entityType, context))
                {
                    string attributeName = metadata.GetAttributeNameForType(entityType, context);
                    //esta definido el atributo, lo escribo
                    writer.WriteAttributeString(attributeName, valueStr);
                }
                else
                {
                    string elementName = desc.GetElementNameForType(entityType, context, true);

                    //escribo el nombre de la propiedad
                    writer.WriteStartElement(elementName);

                    base.WriteTypeDefinition(desc, entityType, context, writer);

                    if (desc.IsXmlAttribute(entityType, context))
                    {
                        string attributeName = metadata.GetAttributeNameForType(entityType, context);
                        //escribo el valor
                        writer.WriteAttributeString(attributeName, valueStr);
                    }
                    else
                        writer.WriteRaw(valueStr);
                    //writer.WriteString(valueStr); si tiene caracteres de xml los convierte a &amp

                    //cierro el tag
                    writer.WriteEndElement();
                }
            }
        }

        protected override void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context)
        {
            Type entityType = metadata.Metadata.PropertyType;

                ValueTypePropertyDescriptor desc = metadata.GetPropertyDescriptor<ValueTypePropertyDescriptor>(entityType, context);

                if (!desc.IsXmlElement(entityType, context))
                {
                    string attributeName = metadata.GetAttributeNameForType(entityType, context);
                    //esta definido el atributo, lo escribo
                    writer.WriteAttributeString(attributeName, "");
                }
                else
                {
                    string elementName = desc.GetElementNameForType(entityType, context, true);

                    //escribo el nombre de la propiedad
                    writer.WriteElementString(elementName, "");
                }
        }

        public virtual string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            string value;

            if (type == _byte)
                value = XmlConvert.ToString((Byte)attValue);
            else if (type == _sbyte)
                value = XmlConvert.ToString((SByte)attValue);
            else if (type == _short)
                value = XmlConvert.ToString((short)attValue);
            else if (type == _int)
                value = XmlConvert.ToString((Int32)attValue);
            else if (type == _long)
                value = XmlConvert.ToString((Int64)attValue);
            else if (type == _ushort)
                value = XmlConvert.ToString((UInt16)attValue);
            else if (type == _uint)
                value = XmlConvert.ToString((UInt32)attValue);
            else if (type == _ulong)
                value = XmlConvert.ToString((UInt64)attValue);
            else if (type == _bool)
                value = XmlConvert.ToString((bool)attValue);
            else if (type == _char)
                value = XmlConvert.ToString((char)attValue);
            else
                value = "";

            return value;
        }

        public virtual object GetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(attValue))
                    return this.GetDefaultValue(metadata, type, context);
                else
                    return this.DoGetValueFromString(metadata, attValue, type, context);
            }
            catch (Exception ex)
            {
                throw XmlDataSerializerExceptionFactory.CreateCanNotGetParseValueTypeException(metadata, attValue, type, context, ex);
            }
        }

        protected virtual object GetDefaultValue(ValueTypePropertyDescriptor metadata, Type type, XmlSerializerContext context)
        {
            return Activator.CreateInstance(type, true);
        }

        protected virtual object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            object value;

            if (type == _byte)
                value = XmlConvert.ToByte(attValue);
            else if (type == _sbyte)
                value = XmlConvert.ToSByte(attValue);
            else if (type == _short)
                value = XmlConvert.ToInt16(attValue);
            else if (type == _int)
                value = XmlConvert.ToInt32(attValue);
            else if (type == _long)
                value = XmlConvert.ToInt64(attValue);
            else if (type == _ushort)
                value = XmlConvert.ToUInt16(attValue);
            else if (type == _uint)
                value = XmlConvert.ToUInt32(attValue);
            else if (type == _ulong)
                value = XmlConvert.ToUInt64(attValue);
            else if (type == _bool)
                value = XmlConvert.ToBoolean(attValue);
            else if (type == _char)
                value = XmlConvert.ToChar(attValue);

            else if (type == _nbyte)
                value = XmlConvert.ToByte(attValue);
            else if (type == _nsbyte)
                value = XmlConvert.ToSByte(attValue);
            else if (type == _nshort)
                value = XmlConvert.ToInt16(attValue);
            else if (type == _nint)
                value = XmlConvert.ToInt32(attValue);
            else if (type == _nlong)
                value = XmlConvert.ToInt64(attValue);
            else if (type == _nushort)
                value = XmlConvert.ToUInt16(attValue);
            else if (type == _nuint)
                value = XmlConvert.ToUInt32(attValue);
            else if (type == _nulong)
                value = XmlConvert.ToUInt64(attValue);
            else if (type == _nbool)
                value = XmlConvert.ToBoolean(attValue);
            else if (type == _nchar)
                value = XmlConvert.ToChar(attValue);
            else
                value = null;

            return value;
        }
    }
}

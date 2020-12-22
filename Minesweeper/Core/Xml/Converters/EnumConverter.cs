namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;
    using Minesweeper.Core.Helpers;

    internal class EnumConverter : ValueTypeConverter
    {
        private static readonly Type __enumType = typeof(Enum);

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__enumType, this);
            context.RegisterAlias("Enum", __enumType);
        }

        public override string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            if (attValue == null)
            {
                return null;
            }
            else
            {
                if (context.Settings.EnumAsString)
                    return attValue.ToString();
                else
                    return ((int)attValue).ToString();
            }
        }

        protected override object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            if (TypeHelper.IsNullableType(type))
            {
                Type enumType = TypeHelper.GetNullableType(type);
                return Enum.Parse(enumType, attValue);
            }
            else
                return Enum.Parse(type, attValue);
        }
    }
}


namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;

    internal class GuidConverter : ValueTypeConverter
    {
        private static readonly Type __guidType = typeof(Guid);
        private static readonly Type __nguidType = typeof(Guid?);

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__guidType, this);
            context.RegisterAlias("Guid", __guidType);

            context.RegisterConverter(__nguidType, this);
            context.RegisterAlias("NullableGuid", __nguidType);

        }

        public override string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            Guid guid = (Guid)attValue;
            return guid.ToString();
        }

        protected override object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            return Guid.Parse(attValue);
        }
    }
}


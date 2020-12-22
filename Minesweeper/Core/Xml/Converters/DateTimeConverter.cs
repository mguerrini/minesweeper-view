namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;
    using System.Globalization;

    internal class DateTimeConverter : ValueTypeConverter
    {
        private static readonly Type __dateTimeType = typeof(DateTime);
        private static readonly Type __dateTimeOffsetType = typeof(DateTimeOffset);
        private static readonly Type __nullableDateTimeType = typeof(DateTime?);
        private static readonly Type __nullableDateTimeOffsetType = typeof(DateTimeOffset?);

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__dateTimeType, this);
            context.RegisterAlias("DateTime", __dateTimeType);

            context.RegisterConverter(__dateTimeOffsetType, this);
            context.RegisterAlias("DateTimeOffset", __dateTimeOffsetType);

            context.RegisterConverter(__nullableDateTimeType, this);
            context.RegisterAlias("NullableDateTime", __nullableDateTimeType);

            context.RegisterConverter(__nullableDateTimeOffsetType, this);
            context.RegisterAlias("NullableDateTimeOffset", __nullableDateTimeOffsetType);
        }


        public override string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            string dateFormat = metadata.DateTimeFormat;

            if (dateFormat == null)
                dateFormat = context.Settings.DefaultDateTimeFormat;

            if (attValue is DateTime || attValue is DateTime?)
            {
                DateTime value = (DateTime)attValue;
                if (!string.IsNullOrEmpty(dateFormat))
                    return value.ToString(dateFormat);
                else
                    return value.ToString(DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                DateTimeOffset value = (DateTimeOffset)attValue;
                if (!string.IsNullOrEmpty(dateFormat))
                    return value.ToString(dateFormat);
                else
                    return value.ToString(DateTimeFormatInfo.InvariantInfo);
            }
        }

        protected override object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            string dateFormat = metadata.DateTimeFormat;

            if (dateFormat == null)
                dateFormat = context.Settings.DefaultDateTimeFormat;

            if (type.Equals(__dateTimeType) || type.Equals(__nullableDateTimeType))
                return XmlConvert.ToDateTime(attValue, dateFormat);
            else
                return XmlConvert.ToDateTimeOffset(attValue, dateFormat);
        }
    }
}


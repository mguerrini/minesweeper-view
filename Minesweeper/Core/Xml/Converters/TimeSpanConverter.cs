namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;

    internal class TimeSpanConverter : ValueTypeConverter
    {
        private static readonly Type __timeSpanType = typeof(TimeSpan);
        private static readonly Type __ntimeSpanType = typeof(TimeSpan?);

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__timeSpanType, this);
            context.RegisterAlias("TimeSpan", __timeSpanType);

            context.RegisterConverter(__ntimeSpanType, this);
            context.RegisterAlias("NullableTimeSpan", __ntimeSpanType);
        }


        public override string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            string timeSpanFormat = metadata.TimeSpanFormat;

            if (timeSpanFormat == null)
                timeSpanFormat = context.Settings.DefaultTimeSpanFormat;

            TimeSpan value = (TimeSpan)attValue;
            if (timeSpanFormat == null)
                return value.ToString();
            else
                return value.ToString(timeSpanFormat);
        }

        protected override object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            string timeSpanFormat = metadata.TimeSpanFormat;

            if (timeSpanFormat == null)
                timeSpanFormat = context.Settings.DefaultTimeSpanFormat;

            if (timeSpanFormat == null)
                return XmlConvert.ToTimeSpan(attValue);
            else
                return TimeSpan.ParseExact(attValue, timeSpanFormat, null);
        }
    }
}


namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;
    using System.Globalization;

    public class RealNumberConverter : ValueTypeConverter
    {
        private static readonly Type _float = typeof(float);
        private static readonly Type _double = typeof(double);
        private static readonly Type _decimal = typeof(decimal);

        private static readonly Type _nfloat = typeof(float?);
        private static readonly Type _ndouble = typeof(double?);
        private static readonly Type _ndecimal = typeof(decimal?);
        
        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(_float, this);
            context.RegisterConverter(_double, this);
            context.RegisterConverter(_decimal, this);

            context.RegisterConverter(_nfloat, this);
            context.RegisterConverter(_ndouble, this);
            context.RegisterConverter(_ndecimal, this);

            context.RegisterAlias("Float", _float);
            context.RegisterAlias("Double", _double);
            context.RegisterAlias("Decimal", _decimal);

            context.RegisterAlias("NullableFloat", _nfloat);
            context.RegisterAlias("NullableDouble", _ndouble);
            context.RegisterAlias("NullableDecimal", _ndecimal);
        }


        public override string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            string value;
            string numberFormat = metadata.NumberFormat;

            if (numberFormat == null)
                numberFormat = context.Settings.DefaultNumberFormat;

            if (type == _float || type == _nfloat)
            {
                if (!string.IsNullOrEmpty(numberFormat))
                    value = ((float)attValue).ToString(numberFormat, CultureInfo.InvariantCulture);
                else
                    value = ((float)attValue).ToString(NumberFormatInfo.InvariantInfo);
            }
            else if (type == _double || type == _ndouble)
            {
                if (!string.IsNullOrEmpty(numberFormat))
                    value = ((double)attValue).ToString(numberFormat, CultureInfo.InvariantCulture);
                else
                    value = ((double)attValue).ToString(NumberFormatInfo.InvariantInfo);
            }
            else if (type == _decimal || type == _ndecimal)
            {
                if (!string.IsNullOrEmpty(numberFormat))
                    value = ((decimal)attValue).ToString(numberFormat, CultureInfo.InvariantCulture);
                else
                    value = ((decimal)attValue).ToString(NumberFormatInfo.InvariantInfo);
            }
            else
                value = "";

            return value;
        }

        protected override object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            object value;
            string numberFormat = metadata.NumberFormat;

            if (numberFormat == null)
                numberFormat = context.Settings.DefaultNumberFormat;

            if (type == _float || type == _nfloat)
            {
                if (!string.IsNullOrEmpty(numberFormat))
                    value = float.Parse(attValue, CultureInfo.InvariantCulture);
                else
                    value = float.Parse(attValue, CultureInfo.InvariantCulture);
            }
            else if (type == _double || type == _ndouble)
            {
                if (!string.IsNullOrEmpty(numberFormat))
                    value = double.Parse(attValue, CultureInfo.InvariantCulture);
                else
                    value = double.Parse(attValue, CultureInfo.InvariantCulture);
            }
            else if (type == _decimal || type == _ndecimal)
            {
                if (!string.IsNullOrEmpty(numberFormat))
                    value = decimal.Parse(attValue, CultureInfo.InvariantCulture);
                else
                    value = decimal.Parse(attValue, CultureInfo.InvariantCulture);
            }
            else
                value = null;

            return value;
        }
    }
}

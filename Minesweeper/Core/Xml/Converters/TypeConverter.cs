namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Minesweeper.Core.Xml.Metadata;
    using Minesweeper.Core.Helpers;

    internal class TypeConverter : ValueTypeConverter
    {
        private static readonly Type __type = typeof(Type);

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__type, this);
            context.RegisterAlias("Type", __type);
        }

        public override string GetValueAsString(ValueTypePropertyDescriptor metadata, object attValue, Type type, XmlSerializerContext context)
        {
            return this.TypeToString((Type)attValue, context.Settings.TypeSettings);
        }

        protected override object DoGetValueFromString(ValueTypePropertyDescriptor metadata, string attValue, Type type, XmlSerializerContext context)
        {
            Type t = Type.GetType(attValue);
            if (t == null)
                t = TypeConverter.GetSystemType(attValue);
            return t;
        }

        protected override object GetDefaultValue(ValueTypePropertyDescriptor metadata, Type type, XmlSerializerContext context)
        {
            return null;
        }

        public static Type GetSystemType(string typeName)
        {
            typeName = typeName.ToUpper().Trim();
            switch(typeName)
            {
                case "STRING":
                    return typeof(string);
                case "INT":
                    return typeof(int);
                case "BOOL":
                    return typeof(bool);
                case "LONG":
                    return typeof(long);
                case "FLOAT":
                    return typeof(float);
                case "DOUBLE":
                    return typeof(double);
                case "DATETIME":
                    return typeof(DateTime);
                case "TIMESPAN":
                    return typeof(TimeSpan);
                case "CHAR":
                    return typeof(char);
                case "DECIMAL":
                    return typeof(decimal);
                case "GUID":
                    return typeof(Guid);
                case "BOOLEAN":
                    return typeof(Boolean);
                case "UINT16":
                    return typeof(UInt16);
                case "UINT32":
                    return typeof(UInt32);
                case "UINT64":
                    return typeof(UInt64);
                case "INT16":
                    return typeof(Int16);
                case "INT32":
                    return typeof(Int32);
                case "INT64":
                    return typeof(Int64);
                case "USHORT":
                    return typeof(ushort);
                case "SHORT":
                    return typeof(short);
                case "SBYTE":
                    return typeof(sbyte);
                case "BYTE":
                    return typeof(byte);
                case "SINGLE":
                    return typeof(Single);
                case "DATETIMEOFFSET":
                    return typeof(DateTimeOffset);


                case "BOOL[]":
                    return typeof(bool[]);
                case "BOOLEAN[]":
                    return typeof(Boolean[]);
                case "UINT16[]":
                    return typeof(UInt16[]);
                case "UINT32[]":
                    return typeof(UInt32[]);
                case "UINT64[]":
                    return typeof(UInt64[]);
                case "INT16[]":
                    return typeof(Int16[]);
                case "INT32[]":
                    return typeof(Int32[]);
                case "INT64[]":
                    return typeof(Int64[]);
                case "INT[]":
                    return typeof(int[]);
                case "USHORT[]":
                    return typeof(ushort[]);
                case "SHORT[]":
                    return typeof(short[]);
                case "SBYTE[]":
                    return typeof(sbyte[]);
                case "BYTE[]":
                    return typeof(byte[]);
                case "LONG[]":
                    return typeof(long[]);
                case "DECIMAL[]":
                    return typeof(decimal[]);
                case "SINGLE[]":
                    return typeof(Single[]);
                case "FLOAT[]":
                    return typeof(float[]);
                case "DOUBLE[]":
                    return typeof(double[]);
                case "DATETIME[]":
                    return typeof(DateTime[]);
                case "DATETIMEOFFSET[]":
                    return typeof(DateTimeOffset[]);
                case "TIMESPAN[]":
                    return typeof(TimeSpan[]);
                case "CHAR[]":
                    return typeof(char[]);
                case "GUID[]":
                    return typeof(Guid[]);
                case "STRING[]":
                    return typeof(string[]);

                default:
                    return null;
            }
        }
    }
}


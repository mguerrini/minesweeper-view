namespace Minesweeper.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Configuration;
    using System.Xml.Serialization;

    public enum VisibilityPropertyType
    {
        Public,
        NonPublic,
        PublicAndNonPublic
    }


    public static class Types
    {
        public static readonly Type NullableType = typeof(Nullable<>);

        public static readonly Type ObjectType = typeof(object);
        public static readonly Type BoolType = typeof(bool);
        public static readonly Type CharType = typeof(char);

        public static readonly Type ByteType = typeof(byte);
        public static readonly Type SByteType = typeof(sbyte);

        public static readonly Type ShortType = typeof(short);
        public static readonly Type IntType = typeof(int);
        public static readonly Type LongType = typeof(long);

        public static readonly Type UShortType = typeof(ushort);
        public static readonly Type UIntType = typeof(uint);
        public static readonly Type ULongType = typeof(ulong);

        public static readonly Type FloatType = typeof(float);
        public static readonly Type DoubleType = typeof(double);
        public static readonly Type DecimalType = typeof(decimal);

        public static readonly Type StringType = typeof(string);
        public static readonly Type DateTimeType = typeof(DateTime);
        public static readonly Type GuidType = typeof(Guid);
        public static readonly Type TimeSpanType = typeof(TimeSpan);
        public static readonly Type TypeType = typeof(Type);
        public static readonly Type TypeTypeType = typeof(Type).GetType();
        public static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);
    }

    public class TypeHelper
    {
        protected static readonly Type _configurationSectionType = typeof(ConfigurationSection);
        private static readonly Type __interfaceXmlSerializableType = typeof(IXmlSerializable);

        private static readonly Assembly __mscorlib = typeof(int).Assembly;

        private static readonly byte _byte = default(byte);
        private static readonly sbyte _sbyte = default(sbyte);
        private static readonly short _short = default(short);
        private static readonly int _int = default(int);
        private static readonly long _long = default(long);
        private static readonly ushort _ushort = default(ushort);
        private static readonly uint _uint = default(uint);
        private static readonly ulong _ulong = default(ulong);
        private static readonly float _float = default(float);
        private static readonly double _double = default(double);
        private static readonly bool _bool = default(bool);
        private static readonly char _char = default(char);
        private static readonly decimal _decimal = default(decimal);
        private static readonly DateTime _dateTime = default(DateTime);
        private static readonly TimeSpan _timeSpan = default(TimeSpan);
        private static readonly Guid _guid = default(Guid);
		
        public static object GetDefaultValue(Type type)
        {
            if (TypeHelper.IsNullableType(type))
                type = Nullable.GetUnderlyingType(type);

            object output;
            TypeCode code = Type.GetTypeCode(type);

            switch (code)
            {
                case TypeCode.Boolean:
                    output = _bool;
                    break;

                case TypeCode.Char:
                    output = _char;
                    break;

                case TypeCode.DBNull:
                    output = DBNull.Value;
                    break;


                case TypeCode.DateTime:
                    output = _dateTime;
                    break;

                case TypeCode.Object:
                    if (type.Equals(Types.TimeSpanType))
                        output = _timeSpan;
                    else if (type.Equals(Types.GuidType))
                        output = _guid;
                    else
                        output = null;
                    break;

                case TypeCode.String:
                    output = null;
                    break;

                case TypeCode.Empty:
                    output = null;
                    break;

                case TypeCode.Byte:
                    output = _byte;
                    break;

                case TypeCode.SByte:
                    output = _sbyte;
                    break;

                case TypeCode.Single:
                    output = _float;
                    break;

                case TypeCode.Double:
                    output = _double;
                    break;

                case TypeCode.Decimal:
                    output = _decimal;
                    break;

                case TypeCode.Int16:
                    output = _short;
                    break;

                case TypeCode.Int32:
                    output = _int;
                    break;

                case TypeCode.Int64:
                    output = _long;
                    break;

                case TypeCode.UInt16:
                    output = _ushort;
                    break;

                case TypeCode.UInt32:
                    output = _uint;
                    break;

                case TypeCode.UInt64:
                    output = _ulong;
                    break;

                default:
                    output = null;
                    break;
            }
            return output;
        }

        public static bool IsSerializable(Type type)
        {
            return type.IsSerializable || _configurationSectionType.IsAssignableFrom(type) || __interfaceXmlSerializableType.IsAssignableFrom(type);
        }

        public static bool IsDefaultValue(object value, Type type)
        {
            if (TypeHelper.IsNullableType(type))
            {
                if (value == null)
                    return true;
                type = Nullable.GetUnderlyingType(type);
            }

            bool output;
            TypeCode code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Boolean:
                    output = (bool)value == _bool;
                    break;

                case TypeCode.Char:
                    output = (char)value == _char;
                    break;

                case TypeCode.DBNull:
                    output = (DBNull)value == DBNull.Value;
                    break;
                case TypeCode.DateTime:
                    output = (DateTime)value == _dateTime;
                    break;

                case TypeCode.Object:
                    if (type.Equals(Types.TimeSpanType))
                        output = (TimeSpan)value == _timeSpan;
                    else if (type.Equals(Types.GuidType))
                        output = (Guid) value == _guid;
                    else
                        output = value == null;
                    break;

                case TypeCode.String:
                    output = value == null;
                    break;

                case TypeCode.Empty:
                    output = value == null;
                    break;

                case TypeCode.Byte:
                    output = (byte)value == _byte;
                    break;

                case TypeCode.SByte:
                    output = (sbyte)value == _sbyte;
                    break;
                case TypeCode.Single:
                    output = (float)value == _float;
                    break;

                case TypeCode.Double:
                    output = (double)value == _double;
                    break;
                case TypeCode.Decimal:
                    output = (decimal)value == _decimal;
                    break;

                case TypeCode.Int16:
                    output = (short)value == _short;
                    break;

                case TypeCode.Int32:
                    output = (int)value == _int;
                    break;

                case TypeCode.Int64:
                    output = (long)value == _long;
                    break;
                case TypeCode.UInt16:
                    output = (ushort)value == _ushort;
                    break;
                case TypeCode.UInt32:
                    output = (uint)value == _uint;
                    break;

                case TypeCode.UInt64:
                    output = (ulong)value == _ulong;
                    break;
                default:
                    output = value == null;
                    break;
            }
            return output;
        }


        public static bool IsValueType(Type type)
        {
            return type.IsValueType || type.Equals(Types.TypeType) || type.Equals(Types.TypeTypeType) ||
                type.IsEnum || type.Equals(Types.StringType) || type.Equals(Types.DateTimeType) || type.Equals(Types.DateTimeOffsetType) ||
                type.Equals(Types.TimeSpanType) || type.Equals(Types.GuidType);
        }

        public static bool IsNullableType(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static Type GetNullableType(Type type)
        {
            return type.GetGenericArguments()[0];
        }


        public static bool IsCollectionType(Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type);
        }

        public static bool IsListType(Type type)
        {
            if (type.IsGenericType && type.IsInterface)
            {
                Type[] genericsTypes = type.GetGenericArguments();
                if (genericsTypes.Length == 1)
                {
                    Type listType = typeof(IList<>).MakeGenericType(genericsTypes[0]);
                    return listType.IsAssignableFrom(type);
                }
                else
                    return false;
            }
            else
                return typeof(IList).IsAssignableFrom(type);
        }

        public static bool IsDictionaryType(Type type)
        {
            if (type.IsGenericType && type.IsInterface)
            {
                Type[] genericsTypes = type.GetGenericArguments();
                if (genericsTypes.Length == 2)
                {
                    Type dictionaryType = typeof(IDictionary<,>).MakeGenericType(genericsTypes[0], genericsTypes[1]);
                    return dictionaryType.IsAssignableFrom(type);
                }
                else
                    return false;
            }
            else
                return typeof(IDictionary).IsAssignableFrom(type);
        }

        public static bool IsGenericType(Type type)
        {
            return type.IsGenericType;
        }

        public static Type GenericListArgumentType(Type type)
        {
            if (type.IsGenericType && IsListType(type))
            {
                Type[] genericsTypes = type.GetGenericArguments();
                return genericsTypes[0];
            }

            return null;
        }

        public static Type GetFirstGenericArgumentType(Type type)
        {
            if (type.IsGenericType)
            {
                Type[] genericsTypes = type.GetGenericArguments();
                return genericsTypes[0];
            }

            return null;
        }

        #region -- Class Name --

        public static string GetFullClassName(Type entityType)
        {
            string className;
            string assemblyName;
            TypeHelper.GetFullClassNameAndAssemblyName(entityType, out className, out assemblyName);
            if (entityType.IsGenericType)
            {
                Type[] args = entityType.GetGenericArguments();
                StringBuilder b = new StringBuilder();
                b.Append(className);
                b.Append("[");
                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                        b.Append(",");
                    string innerClass = TypeHelper.GetFullClassName(args[i]); 
                    b.Append("[");
                    b.Append(innerClass);
                    b.Append("]");
                }
                b.Append("]");
                if (entityType.Assembly != __mscorlib)
                {
                    b.Append(", ");
                    b.Append(assemblyName);
                }
                return b.ToString();
            }
            else
            {
                if (entityType.Assembly == __mscorlib)
                    return className;
                else
                    return className + ", " + assemblyName;
            }
        }

        private static void GetFullClassNameAndAssemblyName(Type entityType, out string className, out string assemblyName)
        {
            className = entityType.FullName;
            if (!entityType.IsGenericType)
            {
                int index = entityType.Assembly.FullName.IndexOf(",");
                assemblyName = entityType.Assembly.FullName.Substring(0, index);
            }
            else
            {
                int index = entityType.FullName.IndexOf("[");
                className = className.Substring(0, index);
                index = entityType.Assembly.FullName.IndexOf(",");
                assemblyName = entityType.Assembly.FullName.Substring(0, index);
            }
        }

        #endregion


        public static bool CanAssign(object instance, Type toType)
        {
            if (instance == null)
                return true;

            Type inputConfigType = instance.GetType();

            return TypeHelper.CanAssign(inputConfigType, toType);
        }

        public static bool CanAssign(Type sourceType, Type destinyType)
        {
            if (destinyType.Equals(sourceType))
                return true;

            if (sourceType.IsSubclassOf(destinyType))
                return true;

            if (destinyType.IsAssignableFrom(sourceType))
                return true;

            return false;
        }
        
        public static List<Type> GetSubTypes(Assembly ass, Type type, bool includeAbstracts)
        {
            Type[] types = ass.GetTypes();
            List<Type> output = new List<Type>();

            foreach (Type t in types)
            {
                if (CanAssign(t, type))
                {
                    if(includeAbstracts || !t.IsAbstract )
                        output.Add(t);
                }
            }

            return output;
        }


        #region -- Methods --

        public static MethodInfo GetMethod(Type entityType, string methodName, VisibilityPropertyType visibility)
        {
            BindingFlags flags;

            if (visibility == VisibilityPropertyType.Public)
                flags = (BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            else if (visibility == VisibilityPropertyType.NonPublic)
                flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance);
            else
                flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            return entityType.GetMethod(methodName, flags);
        }

        #endregion


        #region -- Properties --

        public static object GetPropertyValue(object source, string propertyName)
        {
            PropertyInfo info = TypeHelper.GetProperty(source.GetType(), propertyName, VisibilityPropertyType.PublicAndNonPublic);
            if (info == null || !info.CanRead)
                return null;

            return info.GetValue(source);
        }

        public static void SetPropertyValue(object source, string propertyName, object value)
        {
            PropertyInfo info = TypeHelper.GetProperty(source.GetType(), propertyName, VisibilityPropertyType.PublicAndNonPublic);
            if (info != null && info.CanWrite)
                info.SetValue(source, value);
        }

        public static PropertyInfo GetProperty(Type entityType, string propertyName, VisibilityPropertyType visibility)
        {
            if (entityType == null)
                return null;

            BindingFlags flags;
            if(visibility == VisibilityPropertyType.Public)
                flags = (BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            else if (visibility == VisibilityPropertyType.NonPublic)
                flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance);
            else
                flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo info = entityType.GetProperty(propertyName, flags);

            if (info != null)
                return info;

            //busco enla clase base
            return TypeHelper.GetProperty(entityType.BaseType, propertyName, visibility);
        }

        public static List<PropertyInfo> GetGetProperties(Type entityType, VisibilityPropertyType getVisibility, bool onlyValueType)
        {
            BindingFlags flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            PropertyInfo[] infos = entityType.GetProperties(flags);
            List<PropertyInfo> _properties = new List<PropertyInfo>();

            foreach (PropertyInfo info in infos)
            {
                if (info.CanRead)
                {
                    if (!onlyValueType || IsValueType(info.PropertyType))
                    {
                        //si es privado no lo agrego
                        if ((getVisibility == VisibilityPropertyType.Public && info.GetGetMethod(true).IsPublic) ||
                            (getVisibility == VisibilityPropertyType.NonPublic && !info.GetGetMethod(true).IsPublic) ||
                            (getVisibility == VisibilityPropertyType.PublicAndNonPublic))
                            _properties.Add(info);
                    }
                }
            }

            return _properties;
        }

        public static List<PropertyInfo> GetSetProperties(Type entityType, VisibilityPropertyType setVisibility, bool onlyValueType)
        {
            BindingFlags flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            PropertyInfo[] infos = entityType.GetProperties(flags);
            List<PropertyInfo> _properties = new List<PropertyInfo>();

            foreach (PropertyInfo info in infos)
            {
                if (info.CanRead)
                {
                    if (!onlyValueType || IsValueType(info.PropertyType))
                    {
                        //si es privado no lo agrego
                        if ((setVisibility == VisibilityPropertyType.Public && info.GetGetMethod(true).IsPublic) ||
                            (setVisibility == VisibilityPropertyType.NonPublic && !info.GetGetMethod(true).IsPublic) ||
                            (setVisibility == VisibilityPropertyType.PublicAndNonPublic))
                            _properties.Add(info);
                    }
                }
            }

            return _properties;
        }

        public static List<PropertyInfo> GetProperties(Type entityType, VisibilityPropertyType getVisibility, VisibilityPropertyType setVisibility, bool onlyValueType)
        {
            BindingFlags flags = (BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            List<PropertyInfo> _properties = new List<PropertyInfo>();

            PropertyInfo[] infos = entityType.GetProperties(flags);

            foreach (PropertyInfo info in infos)
            {
                if (info.CanRead)
                {
                    if (!onlyValueType || IsValueType(info.PropertyType))
                    {
                        //si es privado no lo agrego
                        if (((getVisibility == VisibilityPropertyType.Public && info.GetGetMethod(true).IsPublic) ||
                            (getVisibility == VisibilityPropertyType.NonPublic && !info.GetGetMethod(true).IsPublic) ||
                            (getVisibility == VisibilityPropertyType.PublicAndNonPublic)) &&

                            ((setVisibility == VisibilityPropertyType.Public && info.GetGetMethod(true).IsPublic) ||
                            (setVisibility == VisibilityPropertyType.NonPublic && !info.GetGetMethod(true).IsPublic) ||
                            (setVisibility == VisibilityPropertyType.PublicAndNonPublic)))

                            _properties.Add(info);
                    }
                }
            }

            return _properties;
        }




        #endregion


        public static Delegate CreateDelegateTo(Type delegateType, Type instanceType, string methodName,bool createTarget)
        {
            MethodInfo method = GetMethod(instanceType, methodName, VisibilityPropertyType.PublicAndNonPublic);
            if (method == null)
                throw new Exception("No es posible obtener el método " + methodName + " en la instancia de tipo " +instanceType.Name);

            if (createTarget)
            {
                object target = Activator.CreateInstance(instanceType);
                return method.CreateDelegate(delegateType, target);
            }
            else
                return method.CreateDelegate(delegateType);
        }

        public static Delegate CreateDelegateTo(Type delegateType, Type instanceType, MethodInfo method, bool createTarget)
        {
            if (createTarget)
            {
                object target = Activator.CreateInstance(instanceType);
                return method.CreateDelegate(delegateType, target);
            }
            else
                return method.CreateDelegate(delegateType);
        }

        public static object GetValueFromString(Type outputType, string nullValue)
        {
            if (outputType.Equals(Types.StringType))
                return nullValue;

            if (string.IsNullOrEmpty(nullValue))
                return TypeHelper.GetDefaultValue(outputType);

            if (outputType.IsEnum)
            {
                string[] names = Enum.GetNames(outputType);
                if (names.Contains(nullValue))
                    return Enum.Parse(outputType, nullValue);
                else
                {
                    try
                    {
                        int number = int.Parse(nullValue);
                        return Enum.ToObject(outputType, number);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("No es posible convertir el valor enum " + nullValue + "al entero correspondiente.", ex);
                    }
                }
            }
            else
            {
                return Convert.ChangeType(nullValue, outputType);
            }
        }
    }
}


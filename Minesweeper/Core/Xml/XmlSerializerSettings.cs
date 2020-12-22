namespace Minesweeper.Core.Xml
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Collections.Generic;
    using Minesweeper.Core.Xml.Metadata;
    using Minesweeper.Core.Text;
    using Minesweeper.Core.Xml.Converters;

    public class XmlSerializerSettings
    {
        public const string ObjectReferenceAttributeName = "Obj-Ref";
        public const string ObjectIdAttributeName = "Obj-Id";

        #region -- Constructors --

        public XmlSerializerSettings()
        {
            this.TypeSettings = new TypeFormatInfo();

            this.WriteTypeDefinition = true;
            this.WriteDefaultValues = true;
            this.DefaultDateTimeFormat = "dd/MM/yyyy HH:mm:ss"; //XmlDefault = "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz"
            this.DefaultTimeSpanFormat = "c"; //-d'.'hh':'mm':'ss'.'fffffff";
            this.DefaultNumberFormat = "G";
            this.EnumAsString = true;
            this.IncludeDeserializationElementFilters = new Dictionary<int, List<WildcardPattern>>();
            this.ExcludeDeserializationElementFilters = new Dictionary<int, List<WildcardPattern>>();
            this.ExcludeSerializationPropertyFilters = new List<PropertyFilter>();
            this.AddNullValueInLists = true;
            this.DefaultInlineListItemSeparator = ";";
            this.UniqueSerializationForInstance = true;
            this.WriteNullValues = false;
            this.IgnoreUnknowTypes = false;
            this.ThrowErrorWithNonSerializableTypes = false;
			this.FormatXmlOutput = false;
            this.WriteRootTypeDefinition = this.WriteTypeDefinition;
            this.WriteEmptyLists = false;
        }

        #endregion

        public TypeFormatInfo TypeSettings
        {
            get;
            protected set;
        }

        public bool WriteRootTypeDefinition { get; set; }

        public bool WriteTypeDefinition
        {
            get
            {
                if (this.WriteTypeDefinitionMode == WriteTypeDefinitionMode.Never)
                    return false;
                else
                    return true;
            }
            set
            {
                if (value)
                    this.WriteTypeDefinitionMode = WriteTypeDefinitionMode.WhenRequired;
                else
                    this.WriteTypeDefinitionMode = WriteTypeDefinitionMode.Never;
            }
        }

        public WriteTypeDefinitionMode WriteTypeDefinitionMode { get; set; }

        public bool WriteDefaultValues { get; set; }

        public bool WriteNullValues { get; set; }

        public bool WriteEmptyLists { get; set; }


        public bool IgnoreUnknowTypes { get; set; }

        public bool ThrowErrorWithNonSerializableTypes { get; set; }


        public string DefaultDateTimeFormat { get; set; }

        public string DefaultTimeSpanFormat { get; set; }

        public string DefaultNumberFormat{ get; set; }

        public string DefaultInlineListItemSeparator { get; set; }

        /// <summary>
        /// Indica si los enums se serializan como string o como su valor númerico.
        /// </summary>
        public bool EnumAsString { get; set; }

        public bool AddNullValueInLists { get; set; }

        public bool UniqueSerializationForInstance { get; set; }

        public bool FormatXmlOutput { get; set; }

        #region -- Filters --

        private Dictionary<int, List<WildcardPattern>> IncludeDeserializationElementFilters { get; set; }

        public void AddIncludeDeserializationElementFilter(int level, WildcardPattern pattern)
        {
            if (!this.IncludeDeserializationElementFilters.ContainsKey(level))
                this.IncludeDeserializationElementFilters.Add(level, new List<WildcardPattern>());

            List<WildcardPattern> patterns = this.IncludeDeserializationElementFilters[level];
            patterns.Add(pattern);
        }

        public List<WildcardPattern> GetIncludeDeserializationElementFilters(int level)
        {
            if (this.IncludeDeserializationElementFilters.ContainsKey(level))
                return this.IncludeDeserializationElementFilters[level];

            return null;
        }

        public void ClearIncludeDeserializationElementFilter(int level)
        {
            if (this.IncludeDeserializationElementFilters.ContainsKey(level))
                this.IncludeDeserializationElementFilters.Remove(level);
        }

        public void ClearIncludeDeserializationElementFilter()
        {
            this.IncludeDeserializationElementFilters.Clear();
        }

        public bool ContainsIncludeDeserializationElementFilter(int level)
        {
            return this.IncludeDeserializationElementFilters.ContainsKey(level);
        }




        private Dictionary<int, List<WildcardPattern>> ExcludeDeserializationElementFilters { get; set; }

        public void AddExcludeDeserializationElementFilter(int level, WildcardPattern pattern)
        {
            if (!this.ExcludeDeserializationElementFilters.ContainsKey(level))
                this.ExcludeDeserializationElementFilters.Add(level, new List<WildcardPattern>());

            List<WildcardPattern> patterns = this.ExcludeDeserializationElementFilters[level];
            patterns.Add(pattern);
        }

        public List<WildcardPattern> GetExcludeDeserializationElementFilters(int level)
        {
            if (this.ExcludeDeserializationElementFilters.ContainsKey(level))
                return this.ExcludeDeserializationElementFilters[level];

            return null;
        }

        public void ClearExcludeDeserializationElementFilter(int level)
        {
            if (this.ExcludeDeserializationElementFilters.ContainsKey(level))
                this.ExcludeDeserializationElementFilters.Remove(level);
        }

        public void ClearExcludeDeserializationElementFilter()
        {
            this.ExcludeDeserializationElementFilters.Clear();
        }

        public bool ContainsExcludeDeserializationElementFilter(int level)
        {
            return this.ExcludeDeserializationElementFilters.ContainsKey(level);
        }



        public List<PropertyFilter> ExcludeSerializationPropertyFilters { get; protected set; }

        public void AddExcludeSerializationProperty(Type ownerType, WildcardPattern filter)
        {
            this.ExcludeSerializationPropertyFilters.Add(new PropertyFilter() { PropertyOwnerType = ownerType, Pattern = filter });
        }

        #endregion

    }

    public class PropertyFilter
    {
        public Type PropertyOwnerType { get; set; }

        public WildcardPattern Pattern { get; set; }

        public bool Match(object owner, PropertyDescriptor prop)
        {
            if (this.PropertyOwnerType.IsAssignableFrom(owner.GetType()))
                //me fijo si la propiedad cumple con el patron
                return this.Pattern.IsLike(prop.Metadata.PropertyName);

            return false;
        }
    }


    public class TypeFormatInfo
    {
        public TypeFormatInfo()
        {
            this.IncludeAssembly = true;
            this.IncludePublicKeyToken = false;
            this.IncludeVersion = false;
            this.FullQualifiedName = false;
            this.AttributeTypeName = "Obj-Type";
        }

        public bool FullQualifiedName { get; set; }

        public bool IncludeVersion { get; set; }

        public bool IncludeAssembly {get; set;}

        public bool IncludePublicKeyToken {get; set;}

        /// <summary>
        /// Incluye el nombre del tipo cuando el tipo declarado es diferente al tipo real, aunque lo pueda obtener de los know types.
        /// </summary>
        //public bool IncludeTypeNames { get; set; }

        /// <summary>
        /// Nombre del attributo que define el tipo del objecto. Por default es def-type
        /// </summary>
        public string AttributeTypeName { get; set; }
    }
}

namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class PropertyMetadataDecorator : PropertyMetadata
    {
        #region -- Constructors --

        public PropertyMetadataDecorator( PropertyMetadata deco)
        {
            this.Decoratee = deco;
        }

        #endregion


        public PropertyMetadata Decoratee
        {
            get;
            set;
        }

        public override float Order
        {
            get
            {
                return this.Decoratee.Order;
            }
            set
            {
                this.Decoratee.Order = value;
            }
        }

        public override Type OwnerType
        {
            get
            {
                return this.Decoratee.OwnerType;
            }
        }

        public override string PropertyName
        {
            get
            {
                return this.Decoratee.PropertyName;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return this.Decoratee.PropertyType;
            }
        }
        

        public override List<Attribute> Attributes
        {
            get
            {
                return EmptyAttributes;
            }
            protected set
            {
            }
        }


        public override List<Type> IncludeTypes
        {
            get
            {
                return EmptyIncludeTypes;
            }
            protected set
            {
            }
        }


        #region -- Elements --

        public override Dictionary<Type, string> TypeToElementMap
        {
            get
            {
                return PropertyMetadata.EmptyTypeToAliasMap;
            }
        }

        public override Dictionary<string, Type> ElementToTypeMap
        {
            get
            {
                return PropertyMetadata.EmptyAliasToTypeMap;
            }
        }

        #endregion


        #region -- Attributes --

        public override Dictionary<Type, string> TypeToAttributeMap
        {
            get
            {
                return PropertyMetadata.EmptyTypeToAliasMap;
            }
        }

        public override Dictionary<string, Type> AttributeToTypeMap
        {
            get
            {
                return PropertyMetadata.EmptyAliasToTypeMap;
            }
        }

        #endregion


        #region -- Items --

        public override string DefaultItemElementName
        {
            get
            {
                return null;
            }
            protected set
            {
            }
        }

        public override Dictionary<Type, string> TypeToElementItemMap
        {
            get
            {
                return PropertyMetadata.EmptyTypeToAliasMap;
            }
            protected set
            {
            }
        }

        public override Dictionary<string, Type> ElementToTypeItemMap
        {
            get
            {
                return PropertyMetadata.EmptyAliasToTypeMap;
            }
            protected set
            {
            }
        }



        public override string DefaulItemAttributeName
        {
            get
            {
                return null;
            }
            protected set
            {
            }
        }

        public override Dictionary<Type, string> TypeToAttributeItemMap
        {
            get
            {
                return PropertyMetadata.EmptyTypeToAliasMap;
            }
            protected set
            {
            }
        }

        public override Dictionary<string, Type> AttributeToTypeItemMap
        {
            get
            {
                return PropertyMetadata.EmptyAliasToTypeMap;
            }
            protected set
            {
            }
        }

        #endregion

    }
}

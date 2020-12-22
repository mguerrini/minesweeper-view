namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DictionaryItemKeyPropertyMetadata : PropertyMetadataDecorator
    {
         private Type _propertyType;
        private string _propertyName;


        #region -- Constructors --

        public DictionaryItemKeyPropertyMetadata(string propertyName, Type itemValueType, PropertyMetadata deco)
            : base(deco)
        {
            _propertyType = itemValueType;
            _propertyName = propertyName;

        }

        #endregion

        public override Type OwnerType
        {
            get
            {
                return this.Decoratee.PropertyType;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return _propertyType;
            }
        }

        public override string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }


        #region -- Elements --

        public override string DefaultElementName
        {
            get
            {
                return this.Decoratee.DefaultDictionaryKeyElementName;
            }
        }

        public override Dictionary<Type, string> TypeToElementMap 
        {
            get
            {
                return this.Decoratee.TypeToElementDictionaryKeyMap;
            }
        }

        public override Dictionary<string, Type> ElementToTypeMap
        {
            get
            {
                return this.Decoratee.ElementToTypeDictionaryKeyMap;
            }
        }

        #endregion


        #region -- Attributes --

        public override string DefaultAttributeName
        {
            get
            {
                return this.Decoratee.DefaultDictionaryKeyAttributeName;
            }
        }

        public override Dictionary<Type, string> TypeToAttributeMap
        {
            get
            {
                return this.Decoratee.TypeToAttributeDictionaryKeyMap;
            }
        }

        public override Dictionary<string, Type> AttributeToTypeMap
        {
            get
            {
                return this.Decoratee.AttributeToTypeDictionaryKeyMap;
            }
        }

        #endregion


        #region -- Set/Get Value --

        public override object GetValue(object entity)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object entity, object value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DictionaryItemValuePropertyMetadata : PropertyMetadataDecorator
    {
        private Type _propertyType;
        private string _propertyName;


        #region -- Constructors --

        public DictionaryItemValuePropertyMetadata(string propertyName, Type itemValueType, PropertyMetadata deco)
            : base(deco)
        {
            _propertyType = itemValueType;
            _propertyName = propertyName;

        }

        #endregion

        public override Type OwnerType
        {
            get
            {
                return this.Decoratee.PropertyType;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return _propertyType;
            }
        }

        public override string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }


        #region -- Elements --

        public override string DefaultElementName
        {
            get
            {
                return this.Decoratee.DefaultDictionaryValueElementName;
            }
        }

        public override Dictionary<Type, string> TypeToElementMap 
        {
            get
            {
                return this.Decoratee.TypeToElementDictionaryValueMap;
            }
        }

        public override Dictionary<string, Type> ElementToTypeMap
        {
            get
            {
                return this.Decoratee.ElementToTypeDictionaryValueMap;
            }
        }

        #endregion


        #region -- Attributes --

        public override string DefaultAttributeName
        {
            get
            {
                return this.Decoratee.DefaultDictionaryValueAttributeName;
            }
        }

        public override Dictionary<Type, string> TypeToAttributeMap
        {
            get
            {
                return this.Decoratee.TypeToAttributDictionaryValueMap;
            }
        }

        public override Dictionary<string, Type> AttributeToTypeMap
        {
            get
            {
                return this.Decoratee.AttributeToTypeDictionaryValueMap;
            }
        }

        #endregion


        #region -- Set/Get Value --

        public override object GetValue(object entity)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object entity, object value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

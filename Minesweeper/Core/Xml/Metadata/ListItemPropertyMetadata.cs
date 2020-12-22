
namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ListItemPropertyMetadata : PropertyMetadataDecorator
    {
        private Type _propertyType;
        private string _propertyName;
        private bool  _isInline;


        #region -- Constructors --

        public ListItemPropertyMetadata(string propertyName, Type itemType, bool isInline, PropertyMetadata deco) : base(deco)
        {
            _propertyType = itemType;
            _propertyName = propertyName;
            _isInline = isInline;
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
                if (_isInline)
                    return this.Decoratee.DefaultElementName;
                else
                    return this.Decoratee.DefaultItemElementName;
            }
        }

        public override Dictionary<Type, string> TypeToElementMap 
        {
            get
            {
                return this.Decoratee.TypeToElementItemMap;
            }
        }

        public override Dictionary<string, Type> ElementToTypeMap
        {
            get
            {
                return this.Decoratee.ElementToTypeItemMap;
            }
        }

        #endregion


        #region -- Attributes --

        public override string DefaultAttributeName
        {
            get
            {
                if (_isInline)
                    return this.Decoratee.DefaultAttributeName;
                else
                    return this.Decoratee.DefaulItemAttributeName;
            }
        }

        public override Dictionary<Type, string> TypeToAttributeMap
        {
            get
            {
                return this.Decoratee.TypeToAttributeItemMap;
            }
        }

        public override Dictionary<string, Type> AttributeToTypeMap
        {
            get
            {
                return this.Decoratee.AttributeToTypeItemMap;
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

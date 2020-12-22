namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Minesweeper.Core.Xml.Attributes;
    using System.Reflection;
    using System.Collections;

    public class ListPropertyDescriptor : ObjectPropertyDescriptor
    {
        private Type _declaringItemType;

        #region -- Constructors --

        public ListPropertyDescriptor(PropertyInfo info)
            : base(info)
        {
        }

        public ListPropertyDescriptor(PropertyMetadata data)
            : base(data)
        {
        }

        #endregion

        public string DefaultItemAlias { get; set; }

        public Type DeclaringItemType
        {
            get
            {
                if (_declaringItemType == null)
                {
                    Type declaringType = this.Metadata.PropertyType;

                    if (declaringType.IsGenericType)
                    {
                        Type[] genercisTypes = declaringType.GetGenericArguments();
                        _declaringItemType = genercisTypes[0];
                    }
                    else
                    {
                        if (this.Metadata.PropertyType.IsArray)
                            _declaringItemType = declaringType.GetElementType();
                        else
                            _declaringItemType = typeof(object);
                    }
                }

                return _declaringItemType;
            }
        }
        

        public bool IsInlineAttributeArray(Type entityType, XmlSerializerContext context)
        {
            return this.IsXmlAttribute(entityType, context);
        }

        public bool IsInlineElementArray(Type entityType, XmlSerializerContext context)
        {
            if (this.IsXmlElement(entityType, context))
            {
                //tiene que estar el attributo de tipo inline
                XmlInlineArrayElementAttribute att = (XmlInlineArrayElementAttribute)this.Metadata.Attributes.Find(p => (p is XmlInlineArrayElementAttribute) && (((XmlInlineArrayElementAttribute)p).Type == null || ((XmlInlineArrayElementAttribute)p).Type.Equals(entityType)));
                return att != null;
            }
            else
                return false;
        }


        public string GetInlineItemSeparator(Type entityType, XmlSerializerContext context, bool isAttribute)
        {
            if (isAttribute)
            {
                //si el tipo es nulo, entonces se busca el default
                XmlInlineArrayAttributeAttribute att = (XmlInlineArrayAttributeAttribute)this.Metadata.Attributes.Find(p => (p is XmlInlineArrayAttributeAttribute) && ((XmlInlineArrayAttributeAttribute)p).Type != null && ((XmlInlineArrayAttributeAttribute)p).Type.Equals(entityType));

                //busco el default inline
                if (att == null)
                    att = (XmlInlineArrayAttributeAttribute)this.Metadata.Attributes.Find(p => (p is XmlInlineArrayAttributeAttribute) && ((XmlInlineArrayAttributeAttribute)p).Type == null);

                if (att == null || string.IsNullOrEmpty(att.ItemSeparator))
                    return context.Settings.DefaultInlineListItemSeparator;
                else
                    return att.ItemSeparator.Trim();
            }
            else
            {
                XmlInlineArrayElementAttribute att = (XmlInlineArrayElementAttribute)this.Metadata.Attributes.Find(p => (p is XmlInlineArrayElementAttribute) && ((XmlInlineArrayElementAttribute)p).Type != null && ((XmlInlineArrayElementAttribute)p).Type.Equals(entityType));

                if (att == null)
                    att = (XmlInlineArrayElementAttribute)this.Metadata.Attributes.Find(p => (p is XmlInlineArrayElementAttribute) && ((XmlInlineArrayElementAttribute)p).Type == null);

                if (att == null || string.IsNullOrEmpty(att.ItemSeparator))
                    return context.Settings.DefaultInlineListItemSeparator;
                else
                    return att.ItemSeparator.Trim();
            }
        }

        public ListItemPropertyDescriptor GetItemPropertyDescriptor(XmlSerializerContext context, bool isInline)
        {
            ListItemPropertyMetadata metadata;
            string alias = context.GetAlias(this.DeclaringItemType);
            if (string.IsNullOrEmpty(alias))
                metadata = new ListItemPropertyMetadata("Item", this.DeclaringItemType, isInline, this.Metadata);
            else
                metadata = new ListItemPropertyMetadata(alias, this.DeclaringItemType, isInline, this.Metadata);

            ListItemPropertyDescriptor output =  new ListItemPropertyDescriptor(metadata);
            output.UseAlias = true;
            return output;
        }


        #region -- ToXml --

        protected override bool DoMustDeclareTypeNameInXmlElement(Type entityType, XmlSerializerContext context)
        {
            bool output = base.DoMustDeclareTypeNameInXmlElement(entityType, context);

            if (output)
            {
                if (entityType.IsArray)
                    return true;

                if (context.IsRootElement(this))
                {
                    if (entityType.IsGenericType)
                        return true;
                    else
                        return false;
                }

                if (this.Metadata.PropertyType.IsInterface)
                {
                    if (this.Metadata.PropertyType.IsGenericType)
                        return !this.Metadata.PropertyType.Name.StartsWith("IList");
                    else
                        return !this.Metadata.PropertyType.Equals(typeof(IList));
                }
                else
                    return true;
            }
            else
                return false;

        }

        public override string GetAttributeNameForType(Type propertyEntityType, XmlSerializerContext context)
        {
            //TODO hacer que las listas se puedan guardar en un atributo, los items separados por ;...&, algo asi
            string att = base.GetAttributeNameForType(propertyEntityType, context);
            return att;
            //if (string.IsNullOrEmpty(att))
            //{
            //    //me fijo si el tipo del item esta registrado...
            //    Type itemType = propertyEntityType.GetElementType();
            //    if (itemType != null)
            //        return base.GetAttributeNameForType(itemType, context);
            //}
        }        

        #endregion


        #region -- From Xml --

        public override Type GetTypeFromAttributeName(string attributeName, XmlSerializerContext context)
        {
            //TODO las listas no se pueden guardar en atributos, por ahora
            return base.GetTypeFromAttributeName(attributeName, context);
        }

        public override Type GetTypeFromElementName(string elementName, XmlSerializerContext context)
        {
            Type type = base.GetTypeFromElementName(elementName, context);
            
            if (type != null)
                return type;

            //si el tipo declarado es una interface....devuelvo el tipo List<T>
            if (this.Metadata.PropertyType.IsInterface)
            {
                //IList o IList<T>
                if (this.Metadata.PropertyType.IsGenericType)
                {
                    Type listType = typeof(List<>);
                    listType = listType.MakeGenericType(this.DeclaringItemType);
                    return listType;
                }
                else
                {
                    return typeof(ArrayList);
                }
            }

            return null;
        }

        #endregion
    }
}

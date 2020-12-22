namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Minesweeper.Core.Xml.Metadata;
    using System.Xml;
    using Minesweeper.Core.Xml.Exceptions;
    using Minesweeper.Core.Text;
    using Minesweeper.Core.Helpers;

    public abstract class ConverterBase : IXmlConverter
    {
        protected string TypeToString(Type entityType, TypeFormatInfo settings)
        {
            if (settings.FullQualifiedName)
                return entityType.AssemblyQualifiedName;
            else
                return TypeHelper.GetFullClassName(entityType);
        }

        protected void WriteTypeDefinition(PropertyDescriptor propDesc, Type entityType, XmlSerializerContext context, XmlTextWriter writer)
        {
            if (context.Settings.WriteTypeDefinitionMode == WriteTypeDefinitionMode.Always)
            {
                string typeName = this.TypeToString(entityType, context.Settings.TypeSettings);
                writer.WriteAttributeString(context.Settings.TypeSettings.AttributeTypeName, typeName);
            }
            else if (context.Settings.WriteTypeDefinitionMode == WriteTypeDefinitionMode.Never)
                return;
            else if (propDesc.MustDeclareTypeNameInXmlElement(entityType, context))
            {
                string typeName = this.TypeToString(entityType, context.Settings.TypeSettings);
                writer.WriteAttributeString(context.Settings.TypeSettings.AttributeTypeName, typeName);
            }
        }

        protected Type GetEntityTypeForElement(PropertyDescriptor propDesc, XmlReader reader, XmlSerializerContext context)
        {
            Type type = context.GetTypeFromAttribute(reader);

            //obtengo el type de la propiedad...con esto se registra el alias
            TypeDescriptor desc = context.GetTypeDescriptor(propDesc.Metadata.PropertyType);

            //busco por la propiedad
            if (type == null)
                type = propDesc.GetTypeFromElementName(reader.LocalName, context);

            //busco por el alias...
            if (type == null)
            {
                if (context.Settings.IgnoreUnknowTypes)
                    return null;
                else
                    type = propDesc.Metadata.PropertyType;
            }

            return type;
        }

        protected bool MustIncludeElement(string name, XmlSerializerContext context)
        {
            int level = context.Stack.InstanciesSequence.Count;
            if (context.Settings.ContainsIncludeDeserializationElementFilter(level))
            {
                //me fijo si un objeto que cumpla ya fue ingresado en la lista
                List<WildcardPattern> filter = context.Settings.GetIncludeDeserializationElementFilters(level);

                foreach (WildcardPattern p in filter)
                {
                    if (p.IsLike(name))
                        return true;
                }

                return false;
            }
            else
                return true;
        }

        protected bool MustExcludeElement(string name, XmlSerializerContext context)
        {
            int level = context.Stack.InstanciesSequence.Count;
            if (context.Settings.ContainsExcludeDeserializationElementFilter(level))
            {
                //me fijo si un objeto que cumpla ya fue ingresado en la lista
                List<WildcardPattern> filter = context.Settings.GetExcludeDeserializationElementFilters(level);

                foreach (WildcardPattern p in filter)
                {
                    if (p.IsLike(name))
                        return true;
                }

                return false;
            }
            else
                return false;
        }

        protected bool MustExcludeSerializationProperty(object parent, PropertyDescriptor metadata, object entity, XmlSerializerContext context)
        {
            foreach (var filter in context.Settings.ExcludeSerializationPropertyFilters)
            {
                if (filter.Match(parent, metadata))
                    return true;
            }

            return false;
        }


        protected long GetReferenceId(XmlReader reader, XmlSerializerContext context)
        {
            string id = reader.GetAttribute(XmlSerializerSettings.ObjectReferenceAttributeName);
            if (string.IsNullOrEmpty(id))
                return 0;
            else
                return long.Parse(id);
        }

        protected long GetInstanceId(XmlReader reader)
        {
            string id = reader.GetAttribute(XmlSerializerSettings.ObjectIdAttributeName);
            if (string.IsNullOrEmpty(id))
                return 0;
            else
                return long.Parse(id);
        }

        protected object GetInstanceByReferenceId(long id, XmlSerializerContext context)
        {
            return context.Stack.GetInstanceByReferenced(id);
        }



        public virtual object FromXml(object parent, PropertyDescriptor metadata, Type entityType, XmlReader reader, XmlSerializerContext context)
        {
            string elementName = reader.LocalName;

            if (this.MustExcludeElement(elementName, context) || !this.MustIncludeElement(elementName, context))
            {
                reader.Skip();
                return null;
            }

            try
            {
                context.Stack.InstanciesSequence.Push(parent);

                //me fijo si esta referenciando a otro objeto
                long id = this.GetReferenceId(reader, context);
                if (id == 0)
                {
                    object output = this.DoFromXml(parent, metadata, entityType, reader, context);
                    return output;
                }
                else
                {
                    reader.Skip();
                    return this.GetInstanceByReferenceId(id, context);
                }
            }
            catch (Exception ex)
            {
                throw XmlDataSerializerExceptionFactory.CreateDeserializationException(elementName, parent, metadata, entityType, context, ex);
            }
            finally
            {
                context.Stack.InstanciesSequence.Pop();
            }
        }

        public virtual void ToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context)
        {
            try
            {
                if (entity != null)
                {
                    //verifico si existe una referencia circular
                    if (!context.Settings.UniqueSerializationForInstance && context.Stack.ExistInSequence(entity))
                    {
                        context.Stack.InstanciesSequence.Push(entity);
                        throw XmlDataSerializerExceptionFactory.CreateCircularReferenceException(parent, metadata, entity, context);
                    }
                    else
                        context.Stack.InstanciesSequence.Push(entity);

                    if (!this.MustExcludeSerializationProperty(parent, metadata, entity, context))
                        this.DoToXml(parent, metadata, entity, writer, context);
                }
                else
                {
                    //si es nulo verifico si se puede escribir valores nulos
                    if (context.Settings.WriteNullValues)
                    {
                        this.DoToNullValueXml(parent, metadata, writer, context);
                    }
                }
            }
            catch (Exception ex)
            {
                throw XmlDataSerializerExceptionFactory.CreateSerializationException(parent, metadata, entity, context, ex);
            }
            finally
            {
                if (entity != null)
                    context.Stack.InstanciesSequence.Pop();
            }
        }

        protected abstract object DoFromXml(object parent, PropertyDescriptor metadata, Type entityType,  XmlReader reader, XmlSerializerContext context);

        public abstract void Register(IXmlContextData context);

        protected abstract void DoToXml(object parent, PropertyDescriptor metadata, object entity, XmlTextWriter writer, XmlSerializerContext context);

        protected abstract void DoToNullValueXml(object parent, PropertyDescriptor metadata, XmlTextWriter writer, XmlSerializerContext context);

    }
}

namespace Minesweeper.Core.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Minesweeper.Core.Xml.Metadata;
    using Minesweeper.Core.Xml.Exceptions;

    public class XmlSerializerContextStack
    {
        
        #region -- Constructors --

        public XmlSerializerContextStack()
        {
            this.ConverterMap = new Dictionary<Type, IXmlConverter>();
            this.Instancies = new List<object>();
            this.AliasMap = new Dictionary<string, List<Type>>();
            this.ReverseAliasMap = new Dictionary<Type, string>();
            this.TypeDescriptorMap = new Dictionary<Type, TypeDescriptor>();
            this.InstanciesSequence = new Stack<object>();
            this.InstanceReferenceIdMap = new Dictionary<object, long>();
            this.ReverseInstanceReferenceIdMap = new Dictionary<long, object>();
            this.InstanceCounter = 0;
        }

        #endregion

        public Dictionary<Type, TypeDescriptor> TypeDescriptorMap {get; set;}

        public Dictionary<Type, IXmlConverter> ConverterMap {get; set;}

        public Dictionary<string, List<Type>> AliasMap { get; set; }

        public Dictionary<Type, string> ReverseAliasMap { get; set; }

        private Dictionary<object, long> InstanceReferenceIdMap { get; set; }

        private Dictionary<long, object> ReverseInstanceReferenceIdMap { get; set; }



        public List<object> Instancies { get; set; }

        public Stack<object> InstanciesSequence { get; set; }

        public long InstanceCounter { get; set; }

        public bool ExistInSequence(object entity)
        {
            return this.InstanciesSequence.Contains(entity);
        }



        #region -- Alias --

        public void RegisterAlias(string alias, Type type)
        {
            if (!this.AliasMap.ContainsKey(alias))
                this.AliasMap[alias] = new List<Type>();

            List<Type> types = this.AliasMap[alias];
            if (!types.Exists(d => d.Equals(type)))
                types.Add(type);

            if (!this.ReverseAliasMap.ContainsKey(type))
                this.ReverseAliasMap.Add(type, alias);
        }

        public void RegisterAlias(TypeDescriptor tdesc)
        {
            if (!string.IsNullOrEmpty(tdesc.ElementName))
                this.RegisterAlias(tdesc.ElementName, tdesc.Type);
        }

        public string GetAlias(Type entityType)
        {
            string alias;
            if (this.ReverseAliasMap.TryGetValue(entityType, out alias))
                return alias;
            else
                return null;
        }

        public Type GetTypeFromAlias(string alias)
        {
            return this.GetTypeFromAlias(alias, null);
        }

        public Type GetTypeFromAlias(string alias, Type assignableToType)
        {
            if (this.AliasMap.ContainsKey(alias))
            {
                List<Type> aliases = this.AliasMap[alias];
                if (assignableToType == null)
                {
                    if (aliases.Count == 1)
                        return aliases[0];
                    else
                        return null;
                }
                else
                {
                    return this.GetAssignableTypesFor(assignableToType, aliases);
                }
            }
            else
                return null;
        }

        #endregion


        #region -- Instance Reference --

        public bool ContainsInstance(object obj)
        {
            return this.InstanceReferenceIdMap.ContainsKey(obj);
        }

        public long GetInstanceReferenceId(object obj)
        {
            long id = 0;
            if (this.InstanceReferenceIdMap.ContainsKey(obj))
            {
                id = this.InstanceReferenceIdMap[obj];
                if (id == 0)
                {
                    this.InstanceCounter++;
                    id = this.InstanceCounter;
                    this.InstanceReferenceIdMap[obj] = id;

                    this.ReverseInstanceReferenceIdMap.Add(id, obj);
                }

                return id;
            }

            this.InstanceCounter++;
            id = this.InstanceCounter;
            this.InstanceReferenceIdMap.Add(obj, id);
            return id;
        }

        public object GetInstanceByReferenced(long id)
        {
            if (this.ReverseInstanceReferenceIdMap.ContainsKey(id))
                return this.ReverseInstanceReferenceIdMap[id];
            else
                return null;
        }


        public long AddInstance(object obj)
        {
            this.InstanceCounter++;
            long id = this.InstanceCounter;

            this.Instancies.Add(obj);
            this.InstanceReferenceIdMap[obj] = id;
            this.ReverseInstanceReferenceIdMap.Add(id, obj);

            return id;
        }

        public long AddInstance(long id, object obj)
        {
            if (this.ReverseInstanceReferenceIdMap.ContainsKey(id))
            {
                object current = this.ReverseInstanceReferenceIdMap[id];
                throw new XmlDataSerializerException(string.Format("The Obj-Id={0} from entity of type {1} is currently assigned to entity of type {2}.", id, obj.GetType().Name, current.GetType().Name), null);
            }

            //la instancia la agrego siempre
            this.Instancies.Add(obj);

            if (id == 0)
            {
                return 0;
            }
            else
            {
                if (this.InstanceCounter < id)
                    this.InstanceCounter = id;
            }

            this.InstanceReferenceIdMap[obj] = id;
            this.ReverseInstanceReferenceIdMap.Add(id, obj);

            return id;
        }

        #endregion


        private Type GetAssignableTypesFor(Type type, List<Type> types)
        {
            int counter = 0;
            Type output = null;

            foreach (Type t in types)
            {
                if (type.IsAssignableFrom(t))
                {
                    output = t;
                    counter++;
                }
            }

            if (counter == 1)
                return output;
            else
                return null;
        }
    }
}

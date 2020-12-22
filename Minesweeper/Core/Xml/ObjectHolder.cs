namespace Minesweeper.Core.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using System.Reflection;

    [Serializable]
    public class ObjectHolder<T>
    {
        public static PropertyInfo PropertyInfo = typeof(ObjectHolder<T>).GetProperty("Value");

        public ObjectHolder()
        {
            this.Value = default(T);
        }

        public ObjectHolder(T entity)
        {
            this.Value = entity;
        }

        public T Value { get; set; }
    }
}

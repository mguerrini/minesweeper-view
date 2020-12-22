namespace Minesweeper.Core.Xml.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using Minesweeper.Core.Xml.Metadata;
    using System.Xml;

    public class IListConverter : ArrayConverter
    {
        private static readonly Type __listType = typeof(IList);
        private static readonly Type __nullType = typeof(NullType);

        public override void Register(IXmlContextData context)
        {
            context.RegisterConverter(__listType, this);
            context.RegisterAlias("List", __listType);
        }

        protected override IList CreateOutputFrom(ListPropertyDescriptor metadata, IList items, Type itemType)
        {
            return items; 
        }
    }
}

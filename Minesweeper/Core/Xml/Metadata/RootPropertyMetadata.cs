namespace Minesweeper.Core.Xml.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class RootPropertyMetadata : PropertyMetadataDecorator
    {
        public RootPropertyMetadata(string elementName, string attributeName, PropertyMetadata deco) : base(deco)
        {
            this.DefaultElementName = elementName;
            this.DefaultAttributeName = attributeName;
        }

        public override string DefaultElementName
        {
            get;
            protected set;
        }

        public override string DefaultAttributeName
        {
            get;
            protected set;
        }

        public override string PropertyName
        {
            get
            {
                return "Root";
            }
        }

        public override Type PropertyType
        {
            get
            {
                return base.PropertyType;
            }
        }
    }
}

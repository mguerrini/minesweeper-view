namespace Minesweeper.Core.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

   
    public abstract class SelfFactory<TFactory> : FactoryBase, IFactory<TFactory>
    {
        #region -- Constructors --

        protected SelfFactory()
        {
        }

        protected SelfFactory(object configuration) : base(configuration)
        {
        }

        protected SelfFactory(FactoryConfiguration configuration) : base(configuration)
        {
        }

        #endregion

        #region -- IFactory<TFactory> --

        TFactory IFactory<TFactory>.Create()
        {
            object output = this;
            return (TFactory)output;
        }

        protected override object DoCreate()
        {
            return this;
        }

        #endregion
    }
}

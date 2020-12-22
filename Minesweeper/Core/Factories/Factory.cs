using System;
using Minesweeper.Core.Helpers;

namespace Minesweeper.Core.Factories
{
    /// <summary>
    /// Specific Factory for the given entity. Creates the instance with the configuration 
    /// received from the FactoryProvider.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to instantiate.</typeparam>
    public abstract class Factory<TEntity> : FactoryBase, IFactory<TEntity>, IFactory
    {
        private TEntity _singleInstance;
        private object locker = new object();


        #region -- Constructors --

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryBase"/> class.
        /// </summary>
        protected Factory() { }

        protected Factory(object configuration): base(configuration)
        {
        }

        protected Factory(FactoryConfiguration configuration) : base(configuration)
        {
        }

        #endregion


        #region -- IFactory / IFactory<TEntity> Members --

        public virtual TEntity Create()
        {
            if (this.IsSingleInstance)
            {
                if (_singleInstance == null)
                {
                    lock (locker)
                    {
                        if (_singleInstance == null)
                        {
                            _singleInstance = this.DoCreateEntity();
                        }
                    }
                }

                return _singleInstance;
            }
            else
                return (TEntity) this.DoCreate();
        }

        object IFactory.Create()
        {
            return  this.Create();
        }


        protected override object DoCreate()
        {
            return this.DoCreateEntity();
        }

        protected abstract TEntity DoCreateEntity();

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core.Factories
{
    public class FactoryAdapter<TEntity> : Factory<TEntity>
    {
        public FactoryAdapter(IFactory  adaptee)
        {
            this.Adaptee = adaptee;
        }


        protected IFactory Adaptee { get; set; }

        public override TEntity Create()
        {
            return (TEntity) this.Adaptee.Create();
        }

        protected override TEntity DoCreateEntity()
        {
            return (TEntity) this.Adaptee.Create();
        }
    }
}

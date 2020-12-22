
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper.Core.Factories
{
    /// <summary>
    /// Interface to allow abstraction from generic Factory classes. The FactoryProvider 
    /// should be the only class required to create a proper IFactory, with its
    /// configuration set.
    /// </summary>
    /// <typeparam name="TEntity">Factory type.</typeparam>
    public interface IFactory<TEntity>
    {
        /// <summary>
        /// Creates a new instance of TEntity based on the configuration provided.
        /// </summary>
        /// <returns>The appropiate TEntity instance with the configuration applied.</returns>
        TEntity Create();
    }

    public interface IFactory
    {
        object Create();
    }
}
namespace Minesweeper.Core.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IXmlContextData
    {
        void RegisterConverter(Type type, IXmlConverter converter);
        void RegisterAlias(string alias, Type type);
    }
}

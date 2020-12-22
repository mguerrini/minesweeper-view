namespace Minesweeper.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public interface IConfigurationSectionAdapter
    {
        string Name { get; }

        Type ConfigurationType { get; }

        object Configuration { get; }
    }
}

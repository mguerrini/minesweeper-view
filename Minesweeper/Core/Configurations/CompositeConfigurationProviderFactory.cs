namespace Minesweeper.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using Minesweeper.Core.Configurations.Providers;
    using Minesweeper.Core.Configurations.Configurations;

    public class CompositeConfigurationProviderFactory : IConfigurable
    {
        private CompositeConfigurationProviderConfiguration Configuration { get; set; }

        public virtual IConfigurationProvider Create()
        {
            CompositeConfigurationProvider output = new CompositeConfigurationProvider(Minesweeper.Core.Configurations.ConfigurationProvider.CreateLocalProvider());
            AppConfigConfigurationProvider local = output.LocalConfigurationProvider as AppConfigConfigurationProvider;
            
            string baseDirectory = local.ApplicationPath;

            CompositeConfigurationProviderConfiguration config = (CompositeConfigurationProviderConfiguration) this.Configuration;

            if (config != null)
            {
                output.BaseDirectory = Path.GetFullPath(baseDirectory);

                foreach (var c in config.Providers)
                {
                    output.AddProvider(c.CreateConfigurationProvider(config));
                }
            }

            return output;
        }


        void IConfigurable.Configure(IConfiguration config)
        {
            this.Configuration = config.GetConfiguration<CompositeConfigurationProviderConfiguration>();
        }
    }
}

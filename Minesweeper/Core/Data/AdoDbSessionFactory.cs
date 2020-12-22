
namespace Minessweeper.CoreCore.Data.DbSessions
{
    using Minessweeper.CoreCore.Configurations;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class AdoDbSessionFactory : DbSessionFactory
    {
        public override DbSessionData GetSessionData(string dbSessionName)
        {
            ConnectionStringSettings sett = ConfigurationProvider.Instance.GetConnectionStringSettings(dbSessionName);

            if (sett == null)
                return null;

            DbSessionData data = new DbSessionData();
            data.ConnectionString = sett.ConnectionString;
            data.Name = dbSessionName;
            data.ProviderName = sett.ProviderName;

            return data;
        }

        public override IDbSession Create(string sessionName)
        {
            return new AdoDbSession(sessionName);
        }
    }
}

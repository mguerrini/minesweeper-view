using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minessweeper.CoreCore.Data.DbSessions
{
    public interface IDbSessionFactory
    {
        string DefaultSessionName { get; set; }

        DbSessionData GetSessionData(string dbSessionName);

        IDbSession Create();

        IDbSession Create(string sessionName);
    }
}

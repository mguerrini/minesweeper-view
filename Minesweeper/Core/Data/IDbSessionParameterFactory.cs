using System;
using System.Data;
using System.Collections.Generic;

namespace Minessweeper.CoreCore.Data.DbSessions
{
    public interface IDbSessionParameterFactory
    {
        void Clear();
        IDbSessionParameterFactory CreateParameter(string name);
        IDbSessionParameterFactory CreateParameter(string name, System.Data.DbType type);
        IDbSessionParameterFactory CreateParameter(string name, System.Data.DbType type, object value);
        IDbSessionParameterFactory CreateParameter(string name, System.Data.ParameterDirection direction);
        IDbSessionParameterFactory CreateParameter(string name, System.Data.ParameterDirection direction, System.Data.DbType type);
        IDbSessionParameterFactory CreateParameter(string name, System.Data.ParameterDirection direction, System.Data.DbType type, object value);
        IDbSessionParameterFactory CreateParameter(string name, System.Data.ParameterDirection direction, object value);
        IDbSessionParameterFactory CreateParameter(string name, object value);
        IDbSessionParameterFactory CreateParameterStringList(string name, System.Collections.IList values, string delimiter);

        IDataParameter[] GetParametersAsArray();
        List<IDataParameter> GetParametersAsList();
    }
}

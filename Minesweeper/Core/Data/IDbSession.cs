namespace Minessweeper.CoreCore.Data.DbSessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    public interface IDbSession : IDisposable
    {
        IDbSessionParameterFactory ParameterFactory();
        IDataParameter CreateParameter();
        IDataParameter[] CreateParameter(int length);
        IDataParameter CreateParameter(string name);
        IDataParameter CreateParameter(string name, object value);
        IDataParameter CreateParameter(string name, DbType type);
        IDataParameter CreateParameter(string name, DbType type, object value);
        IDataParameter CreateParameter(string name, ParameterDirection direction);
        IDataParameter CreateParameter(string name, ParameterDirection direction, object value);
        IDataParameter CreateParameter(string name, ParameterDirection direction, DbType type, object value);
        IDataParameter CreateParameter(string name, ParameterDirection direction, DbType type);
        IDataParameter CreateParameterStringList(string name, IList values, string delimiter);


        int ExecuteNonQuery(string commandName, params IDataParameter[] parameters);
        int ExecuteNonQuery(string commandName, IList<IDataParameter> parameters);

        object ExecuteScalar(string commandName, params IDataParameter[] parameters);
        object ExecuteScalar(string commandName, IList<IDataParameter> parameters);

        IDataReader ExecuteReader(string commandName);
        IDataReader ExecuteReader(string commandName, params IDataParameter[] parameters);
        IDataReader ExecuteReader(string commandName, IList<IDataParameter> parameters);

        DataSet ExecuteDataSet(string commandName);
        DataSet ExecuteDataSet(string commandName, params IDataParameter[] parameters);
        DataSet ExecuteDataSet(string commandName, IList<IDataParameter> parameters);


        object ExecuteScalarText(string query, params IDataParameter[] parameters);
        object ExecuteScalarText(string query, IList<IDataParameter> parameters);

        int ExecuteNonQueryText(string query, params IDataParameter[] parameters);
        int ExecuteNonQueryText(string query, IList<IDataParameter> parameters);

        IDataReader ExecuteReaderText(string query);
        IDataReader ExecuteReaderText(string query, params IDataParameter[] parameters);
        IDataReader ExecuteReaderText(string query, IList<IDataParameter> parameters);

        DataSet ExecuteDataSetText(string query);
        DataSet ExecuteDataSetText(string query, params IDataParameter[] parameters);
        DataSet ExecuteDataSetText(string query, IList<IDataParameter> parameters);
    }
}

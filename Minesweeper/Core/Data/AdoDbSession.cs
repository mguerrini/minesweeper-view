namespace Minessweeper.CoreCore.Data.DbSessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Common;
    using System.Configuration;
    using System.Data;
    using Core.Configurations;

    /// <summary>
    /// 
    /// </summary>
    public class AdoDbSession : DbSession
    {
        private string _sessionName = null;
        private DbProviderFactory _dbProvider;
        private string _cnn;

        #region -- Constructores --

        public AdoDbSession()
        {
        }

        public AdoDbSession(string sessionName)
        {
            this.SessionName = sessionName;
        }

        #endregion

        public string SessionName
        {
            get { return _sessionName; }
            set
            {
                _sessionName = value;
                _cnn = null;
            }
        }

        public string ConnectionString
        {
            get
            {
                if (_cnn != null)
                    return _cnn;

                ConnectionStringSettings cnn = ConfigurationProvider.Instance.GetConnectionStringSettings(this.SessionName);
                if (cnn != null)
                    _cnn = cnn.ConnectionString;

                return _cnn;
            }
            set
            {
                _cnn = value;
            }
        }
        
        protected DbProviderFactory DbProviderFactory
        {
            get
            {
                if (_dbProvider == null)
                {
                    string providerName = "System.Data.SqlClient";

                    if (!string.IsNullOrEmpty(this.SessionName))
                    {
                        ConnectionStringSettings cnn = ConfigurationProvider.Instance.GetConnectionStringSettings(this.SessionName);

                        if (cnn == null)
                            throw new Exception("La sesion de base de datos de nombre " + this.SessionName + " no existe en el archivo de configuración.");

                        if (!string.IsNullOrEmpty(cnn.ProviderName))
                            providerName = cnn.ProviderName;
                    }

                    DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

                    if (factory == null)
                        throw new Exception("El provider de base de datos de nombre " + providerName + " no existe o no está definido en el archivo de configuración.");

                    _dbProvider = factory;
                }

                return _dbProvider;
            }
            set
            {
                _dbProvider = value;
            }
        }


        #region -- Parameters --

        public override IDataParameter CreateParameter()
        {
            DbParameter output = this.DbProviderFactory.CreateParameter();
            output.Direction = ParameterDirection.Input;
            return output;
        }

        #endregion


        #region -- Connection --

        public virtual IDbConnection OpenConnection()
        {
            IDbConnection conn = this.DbProviderFactory.CreateConnection();
            conn.ConnectionString = this.ConnectionString;
            conn.Open();
            return conn;
        }

        public virtual void EndConnection(IDbConnection cnn)
        {
            try 
            { 
                if (cnn!= null)
                    cnn.Close(); 
            }
            finally { }
        }

        #endregion


        #region -- Execution --

        private IDbCommand AddParametersToCommand(IDbCommand cmd, IDataParameter[] parameters)
        {
            foreach (IDataParameter prm in parameters)
                cmd.Parameters.Add(prm);

            return cmd;
        }

        private IDbCommand CreateCommand(string commandName, IDbConnection connection)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = commandName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = connection;

            return cmd;
        }

        private IDbCommand CreateCommandText(string query, IDbConnection connection)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;

            return cmd;
        }


        public override int ExecuteNonQuery(string commandName, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            int output = 0;
            try
            {
                conn = this.OpenConnection();

                using (IDbCommand cmd = this.CreateCommand(commandName, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                        this.AddParametersToCommand(cmd, parameters);

                    output = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw this.CreateCommandSpException(ex, commandName, parameters);
            }
            finally
            {
                this.EndConnection(conn);
            }

            return output;
        }

        public override object ExecuteScalar(string commandName, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            object retVal = null;

            try
            {
                conn = this.OpenConnection();

                using (IDbCommand cmd = this.CreateCommand(commandName, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                        this.AddParametersToCommand(cmd, parameters);

                    retVal = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw this.CreateCommandSpException(ex, commandName, parameters);
            }
            finally
            {
                this.EndConnection(conn);
            }

            return retVal;
        }

        public override IDataReader ExecuteReader(string commandName, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            try
            {
                conn = this.OpenConnection();

                IDbCommand cmd = CreateCommand(commandName, conn);
                if (parameters != null && parameters.Length > 0)
                    this.AddParametersToCommand(cmd, parameters);

                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                this.EndConnection(conn);
                throw this.CreateCommandSpException(ex, commandName, parameters);
            }
        }

        public override DataSet ExecuteDataSet(string commandName, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            DataSet set = new DataSet();
            try
            {
                conn = this.OpenConnection();

                using (IDbCommand cmd = this.CreateCommand(commandName, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                        this.AddParametersToCommand(cmd, parameters);

                    var adapter = this.DbProviderFactory.CreateDataAdapter();
                    adapter.SelectCommand = cmd as DbCommand;
                    adapter.Fill(set);
                }
            }
            catch (Exception ex)
            {
                throw this.CreateCommandSpException(ex, commandName, parameters);
            }
            finally
            {
                this.EndConnection(conn);
            }

            return set;
        }




        public override int ExecuteNonQueryText(string query, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            int output = 0;
            try
            {
                conn = this.OpenConnection();

                using (IDbCommand cmd = this.CreateCommandText(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                        this.AddParametersToCommand(cmd, parameters);

                    output = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw this.CreateCommandTextException(ex, query, parameters);
            }
            finally
            {
                this.EndConnection(conn);
            }

            return output;
        }

        public override object ExecuteScalarText(string query, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            object retVal = null;

            try
            {
                conn = this.OpenConnection();

                using (IDbCommand cmd = this.CreateCommandText(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                        this.AddParametersToCommand(cmd, parameters);

                    retVal = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw this.CreateCommandTextException(ex, query, parameters);
            }
            finally
            {
                this.EndConnection(conn);
            }

            return retVal;
        }

        public override IDataReader ExecuteReaderText(string query, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            try
            {
                conn = this.OpenConnection();

                IDbCommand cmd = this.CreateCommandText(query, conn);
                if (parameters != null && parameters.Length > 0)
                    this.AddParametersToCommand(cmd, parameters);

                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                this.EndConnection(conn);
                throw this.CreateCommandTextException(ex, query, parameters);
            }
        }

        public override DataSet ExecuteDataSetText(string query, params IDataParameter[] parameters)
        {
            IDbConnection conn = null;
            DataSet set = new DataSet();
            try
            {
                conn = this.OpenConnection();

                using (IDbCommand cmd = this.CreateCommandText(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                        this.AddParametersToCommand(cmd, parameters);
                    
                    var adapter = this.DbProviderFactory.CreateDataAdapter();
                    adapter.SelectCommand = cmd as DbCommand;
                    var p = adapter.GetFillParameters();

                    adapter.Fill(set);
                }
            }
            catch (Exception ex)
            {
                throw this.CreateCommandTextException(ex, query, parameters);
            }
            finally
            {
                this.EndConnection(conn);
            }

            return set;
        }

        #endregion
    }
}

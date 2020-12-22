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
    public abstract class DbSession : IDbSession
    {
        public IDbSessionParameterFactory ParameterFactory()
        {
            return new DbSessionParameterFactory(this);
        }


        public abstract IDataParameter CreateParameter();

        public virtual IDataParameter[] CreateParameter(int length)
        {
            return new IDataParameter[length];
        }


        public virtual IDataParameter CreateParameter(string name)
        {
            var output = this.CreateParameter();
            //if (name[0] != '@')
            //    name = "@" + name;

            output.ParameterName = name;

            return output;
        }

        public virtual IDataParameter CreateParameter(string name, object value)
        {
            IDataParameter prm = this.CreateParameter(name);
            if (value == null)
                prm.Value = DBNull.Value;
            else
                prm.Value = value;

            return prm;
        }


        public virtual IDataParameter CreateParameter(string name, DbType type)
        {
            IDataParameter prm = this.CreateParameter(name);
            prm.DbType = type;
            return prm;
        }

        public virtual IDataParameter CreateParameter(string name, DbType type, object value)
        {
            IDataParameter prm = this.CreateParameter(name);
            prm.DbType = type;
            if (value == null)
                prm.Value = DBNull.Value;
            else
                prm.Value = value;
            return prm;
        }

        public virtual IDataParameter CreateParameter(string name, ParameterDirection direction)
        {
            IDataParameter prm = this.CreateParameter(name);
            prm.Direction = direction;
            return prm;
        }

        public virtual IDataParameter CreateParameter(string name, ParameterDirection direction, object value)
        {
            IDataParameter prm = this.CreateParameter(name);
            prm.Direction = direction;
            if (value == null)
                prm.Value = DBNull.Value;
            else
                prm.Value = value;
            return prm;
        }

        public virtual IDataParameter CreateParameter(string name, ParameterDirection direction, DbType type, object value)
        {
            IDataParameter prm = this.CreateParameter(name);
            prm.Direction = direction;
            prm.DbType = type;
            if (value == null)
                prm.Value = DBNull.Value;
            else
                prm.Value = value;
            return prm;
        }

        public virtual IDataParameter CreateParameter(string name, ParameterDirection direction, DbType type)
        {
            IDataParameter prm = this.CreateParameter(name);
            prm.Direction = direction;
            prm.DbType = type;
            return prm;
        }

        public virtual IDataParameter CreateParameterStringList(string name, IList values, string delimiter)
        {
            if (values == null || values.Count == 0)
                return this.CreateParameter(name, null);
            else
            {
                string value = values[0].ToString();

                for (int i = 1; i < values.Count; i++)
                    value += delimiter + values[i].ToString();

                return this.CreateParameter(name, value);
            }
        }

        public List<IDataParameter> DiscoverParameters()
        {
            throw new NotImplementedException();
        }


        public virtual int ExecuteNonQuery(string commandName, IList<IDataParameter> parameters)
        {
            if (parameters == null)
                return this.ExecuteNonQuery(commandName, (IDataParameter[])null);
            else
                return this.ExecuteNonQuery(commandName, parameters.ToArray());
        }
        
        public abstract int ExecuteNonQuery(string commandName, params IDataParameter[] parameters);


        public virtual object ExecuteScalar(string commandName, IList<IDataParameter> parameters)
        {
            if (parameters == null)
                return ExecuteScalar(commandName, (IDataParameter[])null);
            else
                return ExecuteScalar(commandName, parameters.ToArray());
        }

        public abstract object ExecuteScalar(string commandName, params IDataParameter[] parameters);



        public virtual IDataReader ExecuteReader(string commandName)
        {
            return this.ExecuteReader(commandName, (IDataParameter[])null);
        }

        public virtual IDataReader ExecuteReader(string commandName, IList<IDataParameter> parameters)
        {
            if (parameters == null)
                return ExecuteReader(commandName, (IDataParameter[])null);
            else
                return ExecuteReader(commandName, parameters.ToArray());
        }

        public abstract IDataReader ExecuteReader(string commandName, params IDataParameter[] parameters);



        public virtual int ExecuteNonQueryText(string query, IList<IDataParameter> parameters)
        {
            if (parameters == null)
                return this.ExecuteNonQueryText(query, (IDataParameter[])parameters);
            else
                return this.ExecuteNonQueryText(query, parameters.ToArray());
        }

        public abstract int ExecuteNonQueryText(string query, params IDataParameter[] parameters);



        public virtual object ExecuteScalarText(string query, IList<IDataParameter> parameters)
        {
            if (parameters == null)
                return this.ExecuteScalarText(query, (IDataParameter[])null);
            else
                return this.ExecuteScalarText(query, parameters.ToArray());
        }

        public abstract object ExecuteScalarText(string query, params IDataParameter[] parameters);



        public virtual IDataReader ExecuteReaderText(string query)
        {
            return this.ExecuteReaderText(query, null);
        }

        public virtual IDataReader ExecuteReaderText(string query, IList<IDataParameter> parameters)
        {
            if (parameters != null)
                return this.ExecuteReaderText(query, parameters.ToArray());
            else
                return this.ExecuteReaderText(query, (IDataParameter[])null);
        }

        public abstract IDataReader ExecuteReaderText(string query, params IDataParameter[] parameters);



        public virtual DataSet ExecuteDataSet(string commandName)
        {
            return this.ExecuteDataSet(commandName, null);
        }

        public virtual DataSet ExecuteDataSet(string commandName, IList<IDataParameter> parameters)
        {
            if (parameters == null)
                return this.ExecuteDataSet(commandName, (IDataParameter[])null);
            else
                return this.ExecuteDataSet(commandName, parameters.ToArray());
        }

        public abstract DataSet ExecuteDataSet(string commandName, params IDataParameter[] parameters);



        public virtual DataSet ExecuteDataSetText(string query)
        {
            return this.ExecuteDataSetText(query, null);
        }

        public virtual DataSet ExecuteDataSetText(string query, IList<IDataParameter> parameters)
        {
            if (parameters != null)
                return this.ExecuteDataSetText(query, parameters.ToArray());
            else
                return this.ExecuteDataSetText(query, (IDataParameter[])null);
        }

        public abstract DataSet ExecuteDataSetText(string query, params IDataParameter[] parameters);


        #region -- Exception --

        protected DataException CreateCommandSpException(Exception sqlEx, string commandName, IDataParameter[] parameters)
        {
            DataException newEx = new DataException(string.Format("Error invoking the stored procedure '{0}'", commandName), sqlEx);
            newEx.Data.Add("StoredProcedureName", commandName);

            if (parameters != null && parameters.Length > 0)
            {
                foreach (IDataParameter prm in parameters)
                {
                    try
                    {
                        if (prm.Value != null && !(prm.Value is DBNull))
                            newEx.Data.Add("param_" + prm.ParameterName, prm.Value.ToString());
                        else
                            newEx.Data.Add("param_" + prm.ParameterName, "NULL");
                    }
                    catch
                    {
                    }
                }
            }

            return newEx;
        }

        protected DataException CreateCommandTextException(Exception sqlEx, string query, IDataParameter[] parameters)
        {
            DataException newEx = new DataException("Error invoking query.", sqlEx);
            newEx.Data.Add("Query", query);

            if (parameters != null && parameters.Length > 0)
            {
                foreach (IDataParameter prm in parameters)
                {
                    try
                    {
                        if (prm.Value != null && !(prm.Value is DBNull))
                            newEx.Data.Add("param_" + prm.ParameterName, prm.Value.ToString());
                        else
                            newEx.Data.Add("param_" + prm.ParameterName, "NULL");
                    }
                    catch
                    {
                    }
                }
            }

            return newEx;
        }

        #endregion


        public virtual void Dispose()
        {
        }
    }
}

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
    public class DbSessionParameterFactory : IDbSessionParameterFactory
    {
        #region -- Constructors --

        public DbSessionParameterFactory(IDbSession session)
        {
            this.Session = session;
            this.Parameters = new List<IDataParameter>();
        }

        #endregion

        private IDbSession Session { get; set; }
        
        private List<IDataParameter> Parameters { get; set; }

        public IDbSessionParameterFactory CreateParameter(string name)
        {
            this.Parameters.Add(this.Session.CreateParameter(name));
            return this;
        }

        public IDbSessionParameterFactory CreateParameter(string name, object value)
        {
            this.Parameters.Add(this.Session.CreateParameter(name, value)); 
            return this;
        }

        public IDbSessionParameterFactory CreateParameter(string name, DbType type)
        {
            this.Parameters.Add(this.Session.CreateParameter(name, type));
            return this;
        }

        public IDbSessionParameterFactory CreateParameter(string name, DbType type, object value)
        {
            this.Parameters.Add(this.Session.CreateParameter(name, type, value));
            return this;
        }

        public IDbSessionParameterFactory CreateParameter(string name, ParameterDirection direction)
        {
            this.Parameters.Add(this.Session.CreateParameter(name, direction));
            return this;
        }

        public IDbSessionParameterFactory CreateParameter(string name, ParameterDirection direction, object value)
        {
            this.Parameters.Add(this.Session.CreateParameter(name, direction, value));
            return this;
        }

        public IDbSessionParameterFactory CreateParameter(string name, ParameterDirection direction, DbType type, object value)
        {
            this.Parameters.Add(this.Session.CreateParameter(name, direction, type, value));
            return this;
        }

        public IDbSessionParameterFactory CreateParameter(string name, ParameterDirection direction, DbType type)
        {
            this.Parameters.Add(this.Session.CreateParameter(name, direction, type));
            return this;
        }

        public IDbSessionParameterFactory CreateParameterStringList(string name, IList values, string delimiter)
        {
            this.Parameters.Add(this.Session.CreateParameterStringList(name, values, delimiter));
            return this;
        }

        public IDataParameter[] GetParametersAsArray()
        {
            return this.Parameters.ToArray();
        }

        public List<IDataParameter> GetParametersAsList()
        {
            return this.Parameters;
        }
        
        public void Clear()
        {
            this.Parameters.Clear();
        }
    }
}

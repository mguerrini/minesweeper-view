namespace Minessweeper.CoreCore.Data.DbSessions
{
    using Configurations;
    using Core.Configurations;
    using Minessweeper.CoreCore.Factories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    /// <summary>
    /// 
    /// </summary>
    public abstract class DbSessionFactory : SelfFactory<DbSessionFactory>, IDbSessionFactory
    { 
        #region -- Singleton --

        private static object _locker = new object();

        private static DbSessionFactory _instance = null;

        public static DbSessionFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            if (FactoryProvider.Instance.ExistsFactory<DbSessionFactory>())
                            {
                                IFactory<DbSessionFactory> factory = FactoryProvider.Instance.CreateFactory<DbSessionFactory>();
                                _instance = factory.Create();
                            }
                            else
                            {
                                DbSessionFactory.Initialize();
                            }
                        }
                    }
                }

                return _instance;
            }
            //get
            //{
            //    if (_instance == null)
            //    {
            //        lock (_locker)
            //        {
            //            if (_instance == null)
            //            {
            //                DbSessionFactory.Initialize();
            //            }
            //        }
            //    }

            //    return _instance;
            //}
            set
            {
                _instance = value;
            }
        }

        public abstract DbSessionData GetSessionData(string dbSessionName);


        /// <summary>
        /// Inicialize al singleton con la configuración por defecto. 
        /// </summary>
        protected static void Initialize()
        {
            DbSessionFactoryConfiguration config = new DbSessionFactoryConfiguration();

                var cnns = ConfigurationProvider.Instance.GetConnectionStringSettings();
                if (cnns.Count > 0)
                    config.DefaultSessionName = cnns[0].Name;

            DbSessionFactory.Initialize(config);
        }

        protected static void Initialize(DbSessionFactoryConfiguration config)
        {
            AdoDbSessionFactory impl = new AdoDbSessionFactory();
            impl.Configure(new FactoryConfiguration(config));
            DbSessionFactory.Instance = impl;
        }

        #endregion


        #region -- IDbSessionFactory --

        public virtual IDbSession Create()
        {
            if (string.IsNullOrEmpty(this.DefaultSessionName))
                throw new InvalidOperationException("DbSessionFactory no tiene un nombre de sesión definida por defecto.");

            return this.Create(this.DefaultSessionName);
        }

        public abstract IDbSession Create(string sessionName);

        #endregion


        public string DefaultSessionName { get; set; }


        #region -- Configuration --

        protected virtual void Configure(IConfiguration configData)
        {
            DbSessionFactoryConfiguration config = configData.GetConfiguration<DbSessionFactoryConfiguration>();
            if (config != null)
                this.DefaultSessionName = config.DefaultSessionName;
        }

        #endregion
    }
}

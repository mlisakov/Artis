using System;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using NLog;

namespace Artis.Data
{
    public static class Domain
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        //[ThreadStatic]
        private static object _lockObject = new object();
        private static ISessionFactory sessionFactory;
        private static Configuration _configuration;

        public static ISession Session
        {
            get
            {
                if (sessionFactory == null)
                {
                    Init();
                }
                return sessionFactory.OpenSession();
            }
        }

        public static async Task<ISession> GetSession()
        {
            if (sessionFactory == null)
            {
                await Task.Run(() => Init());
            }
            return sessionFactory.OpenSession();
        }

        private static void Init()
        {
            try
            {
                lock (_lockObject)
                {
                    if (sessionFactory == null)
                    {
                        _configuration = new Configuration();
                        _configuration.Configure(AppDomain.CurrentDomain.BaseDirectory +
                                                 "/NHibernate/Mapping/hibernate.cfg.xml");
                        //Тяжелый процесс, должен вызываться один раз
                        sessionFactory = _configuration.BuildSessionFactory();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка инициализации NHibernate",ex);
                throw new DBAccessFailedException("Ошибка доступа к Базе данных \"Артис\"");
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;

namespace Artis.Data
{
    public static class Domain
    {
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
                Logger.Log.WriteLog("NHibernate.log",
                    "Ошибка инициализации NHibernate" + System.Environment.NewLine + ex.Message +
                    System.Environment.NewLine + ex.StackTrace);
                throw new DBAccessFailedException("Ошибка доступа к Базе данных \"Артис\"");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using NHibernate;
using NHibernate.Cfg;
using NLog;
using NLog.Targets;

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
                        //_configuration = new Configuration();
                        //MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory +
                        //                "/NHibernate/Mapping/hibernate.cfg.xml", "", MessageBoxButtons.OK);
                        //_configuration.Configure(AppDomain.CurrentDomain.BaseDirectory +
                        //                         "/NHibernate/Mapping/hibernate.cfg.xml");
                        //_configuration.AddAssembly("Artis.Data");
                        ////Тяжелый процесс, должен вызываться один раз
                        //sessionFactory = _configuration.BuildSessionFactory();

                        _configuration = new Configuration();

                        //Create a dictionary to hold the properties
                        Dictionary<string,string> properties =new Dictionary<string, string>();

                        //Populate with some default properties
                        properties.Add(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                        properties.Add(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");
                        properties.Add(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSql2008Dialect");
                        //properties.Add(NHibernate.Cfg.Environment.ConnectionString, "Server=KRAKOTSPC;Initial Catalog=ARTIS;User ID=Artis;Password=pass@optima1;");
                        properties.Add(NHibernate.Cfg.Environment.ConnectionString, "Server=92.53.105.145;Initial Catalog=ARTIS;User ID=QueryBase;Password=pass@optima1;");
                        //properties.Add(NHibernate.Cfg.Environment.ConnectionString, "Server=EPRUPETW0475;Initial Catalog=ARTIS;Integrated Security = true;");

                        //Set the config up using our dictionary of properties
                        _configuration.SetProperties(properties);
                        //Add a mapping Assembly
                        _configuration.AddAssembly("Artis.Data.NHibernateMapping");
                        sessionFactory = _configuration.BuildSessionFactory();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK);
                _logger.ErrorException("Ошибка инициализации NHibernate",ex);
                throw new DBAccessFailedException("Ошибка доступа к Базе данных \"Артис\".Текущая директория:" + AppDomain.CurrentDomain.BaseDirectory);
            }
        }
    }
}
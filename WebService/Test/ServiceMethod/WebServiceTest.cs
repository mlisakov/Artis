using System;

namespace Artis.WebService.Test
{
    [TestFixture]
    public class WebServiceTest
    {
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _configuration = new Configuration();
            _configuration.Configure("NHibarnate/Mapping/hibernate.cfg.xml");
            //_configuration.AddAssembly(typeof(MainMapping).Assembly);
            //Тяжелый процесс, должен вызываться один раз
            _sessionFactory = _configuration.BuildSessionFactory();
        }

        [Test]
        public void CanGetActions()
        {
            using (ISession session = Domain.OpenSession())
            {
                IList<Artis.Objects.Action> actions = session.CreateCriteria(typeof(Artis.Objects.Action)).List<Artis.Objects.Action>();
                Assert.IsTrue(products.Count > 0);

            }
        }
    }
}


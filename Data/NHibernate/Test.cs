using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;

namespace Artis.Data
{
    [TestFixture]
    public class HNibernateTest
    {
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _configuration = new Configuration();
            _configuration.Configure(AppDomain.CurrentDomain.BaseDirectory + "/NHibernate/Mapping/hibernate.cfg.xml");
            //_configuration.AddAssembly(typeof(MainMapping).Assembly);
            //Тяжелый процесс, должен вызываться один раз
            _sessionFactory = _configuration.BuildSessionFactory();
        }

        [Test]
        public void CanGetActions()
        {
            using (ISession session = Domain.Session)
            {
                IList<Action> products = session.CreateCriteria(typeof (Action)).List<Action>();
                Assert.IsTrue(products.Count > 0);
            }
        }

        [Test]
        public void CanGetActionsData()
        {
            using (ISession session = Domain.Session)
            {
                IList<Action> products = session.CreateCriteria(typeof (Action)).List<Action>();
                Assert.IsTrue(products.Any(i => i.Data.Any()));
            }
        }

        [Test]
        public void CanGetActionsProducerData()
        {
            using (ISession session = Domain.Session)
            {
                IList<Action> products = session.CreateCriteria(typeof (Action)).List<Action>();
                Assert.IsTrue(products.Any(i => i.Producer != null));
            }
        }

        [Test]
        public void CanGetActionsActorsData()
        {
            using (ISession session = Domain.Session)
            {
                IList<Action> products = session.CreateCriteria(typeof (Action)).List<Action>();
                Assert.IsTrue(products.Any(i => i.Actor.Any()));
            }
        }

        [Test]
        public void Can_CRUD_Action()
        {
            //Action action = new Action {Name = "Тест"};
            //ActionRepository repository = new ActionRepository();
            //repository.Add(action);

            //long id = action.ID;

            //Action addedAction = repository.GetById(action.ID);
            //addedAction.Name = "Тест123";
            //repository.Update(addedAction);
            //Assert.AreNotEqual(addedAction.Name, "Тест");

            //repository.Remove(addedAction);

            //Action deletedAction = repository.GetById(id);
            //Assert.Null(deletedAction);
        }

        [Test]
        public void Can_CRUD_Area()
        {
            Area area = new Area() {Addres = "ул.Кирова, 34/3", Metro = new Metro(){Name = "Нарвская"}, Name = "ДК Кирова"};
            AreaRepository repository = new AreaRepository();
            repository.Add(area);

            long id = area.ID;

            Area addedArea = repository.GetById(area.ID);
            addedArea.Name = "Тест123";
            repository.Update(addedArea);
            Assert.AreNotEqual(addedArea.Name, "ДК Кирова");

            repository.Remove(addedArea);

            Area deletedArea = repository.GetById(id);
            Assert.Null(deletedArea);
        }

    }
}
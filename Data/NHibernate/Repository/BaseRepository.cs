using System.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace Artis.Data
{
    public abstract class BaseRepository<T> : IRepository<T>
    {
        public void Add(T item)
        {
            using (ISession session = Domain.Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(item);
                transaction.Commit();
            }
        }

        public void Update(T item)
        {
            using (ISession session = Domain.Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Update(item);
                transaction.Commit();
            }
        }

        public void Remove(T item)
        {
            using (ISession session = Domain.Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Delete(item);
                transaction.Commit();
            }
        }

        public T GetById(long itemGuid)
        {
            using (ISession session = Domain.Session)
                return session.Get<T>(itemGuid);
        }

        public T GetByName(string name, string field = "Name")
        {
            using (ISession session = Domain.Session)
            {
                T product = session
                    .CreateCriteria(typeof (T))
                    .Add(Restrictions.Eq(field, name))
                    .List<T>().FirstOrDefault();
                return product;
            }
        }

        public bool IsExists(string name, string field = "Name")
        {
            using (ISession session = Domain.Session)
            {
                T product = session
                    .CreateCriteria(typeof(T))
                    .Add(Restrictions.Eq(field, name))
                    .List<T>().FirstOrDefault();
                return product!=null;
            }
        }
    }
}
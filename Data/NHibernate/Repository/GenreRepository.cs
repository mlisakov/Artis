using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NLog;

namespace Artis.Data
{
    public class GenreRepository : BaseRepository<Genre>
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly GenreRepository Genre;

        static GenreRepository()
        {
            Genre = new GenreRepository();
        }

        public async Task<bool> Save(Genre genre)
        {
            try
            {
                Genre.Update(genre);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Ошибка записи изображения ", ex);
                return false;
            }
            return true;
        }

        public async Task<bool> Remove(long id)
        {
            try
            {
                Genre toDelete;
                using (ISession session = Domain.Session)
                {
                    //В случае, если площадка используется в мероприятиях не даем возможности удалить площадку.
                    if (session.Query<Action>().Any(i => i.Genre.ID == id))
                        return false;
                    toDelete = session.Query<Genre>().First(i => i.ID == id);
                }
                Genre.Delete(toDelete);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("DataRequestFactory:Не удалось удалить сущность ", ex);
                return false;
            }
            return true;
        }
    }
}
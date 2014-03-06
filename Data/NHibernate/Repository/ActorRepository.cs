using System;
using System.Threading.Tasks;
using NLog;

namespace Artis.Data
{
    public class ActorRepository : BaseRepository<Actor>
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static ActorRepository _actorRepository;
        private static ActionRepository _actionRepository;

        static ActorRepository()
        {
            _actorRepository = new ActorRepository();
            _actionRepository=new ActionRepository();
        }

        public static async Task<bool> AddActor(Actor actor,Action action)
        {
            try
            {
                _actorRepository.Add(actor);
                action.Actor.Add(actor);
                _actionRepository.Update(action);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка добавления актера для мероприятия ", ex);
                return false;
            }
            return true;
        }
    }
}
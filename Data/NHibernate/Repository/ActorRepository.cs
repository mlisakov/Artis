using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public static async Task<bool> Remove(Actor actor,Action action)
        {
            try
            {
                //_actorRepository.Delete(actor);
                action.Actor.Remove(actor);
                _actionRepository.Update(action);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка удаления актера из мероприятия ", ex);
                return false;
            }
            return true;
        }

        //public async Task<bool> Save(Actor actor, List<string> addedImages, List<long> deletedImages)
        //{
        //    try
        //    {
        //        if (deletedImages != null)
        //        {
        //            Actor currentActor = GetById(actor.ID);
        //            actor.Data = new Collection<Data>(currentActor.Data.ToList());
        //            foreach (long idImage in deletedImages)
        //            {
        //                Data item = actor.Data.First(i => i.ID == idImage);
        //                actor.Data.Remove(item);
        //            }
        //        }

        //        if (addedImages != null)
        //            foreach (string image in addedImages)
        //            {
        //                DataRepository _dataRepository = new DataRepository();
        //                Data data = new Data() { Base64StringData = image };
        //                _dataRepository.Add(data);
        //                if (actor.Data == null)
        //                    actor.Data = new Collection<Data>();
        //                actor.Data.Add(data);
        //            }

        //        Update(actor);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("Ошибка записи изображения ", ex);
        //        return false;
        //    }
        //    return true;
        //}
    }
}
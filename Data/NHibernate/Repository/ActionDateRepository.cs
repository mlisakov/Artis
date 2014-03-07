using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace Artis.Data
{
    public class ActionDateRepository:BaseRepository<ActionDate>
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static ActionDateRepository _actionDateRepository;
        private static ActionRepository _actionRepository;

        static ActionDateRepository()
        {
            _actionDateRepository=new ActionDateRepository();
            _actionRepository=new ActionRepository();
        }

        public static async Task<bool> Save(ActionDate actionDate, List<Data> addedImages, List<long> deletedImages)
        {
            try
            {
                if (deletedImages != null)
                    foreach (long idImage in deletedImages)
                    {
                        Data item = actionDate.Action.Data.First(i => i.ID == idImage);
                        actionDate.Action.Data.Remove(item);
                    }

                if (addedImages != null)
                    foreach (Data data in addedImages)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        _dataRepository.Add(data);
                        if (actionDate.Action.Data == null)
                            actionDate.Action.Data = new Collection<Data>();
                        actionDate.Action.Data.Add(data);
                    }

                _actionDateRepository.Update(actionDate);
                _actionRepository.Update(actionDate.Action);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка обновления мероприятия ", ex);
                return false;
            }
            return true;
        }

        public static async Task<bool> Add(ActionDate actionDate, List<Data> images)
        {
            try
            {
                Action action = actionDate.Action;
                _actionRepository.Add(action);

                if (images != null)
                    foreach (Data data in images)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        _dataRepository.Add(data);
                        if (action.Data == null)
                            action.Data = new Collection<Data>();
                        action.Data.Add(data);
                    }
                _actionRepository.Update(action);
                _actionDateRepository.Add(actionDate);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка добавления мероприятия ", ex);
                return false;
            }
            return true;
        }

        public static async Task<bool> AddActionDate(ActionDate actionDate)
        {
            try
            {
                _actionDateRepository.Add(actionDate);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка добавления даты проведения мероприятия ", ex);
                return false;
            }
            return true;
        }
    }
}

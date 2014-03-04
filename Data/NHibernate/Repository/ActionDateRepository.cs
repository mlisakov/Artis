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

        static ActionDateRepository()
        {
            _actionDateRepository=new ActionDateRepository();
        }

        public static async Task<bool> Save(ActionDate actionDate, List<Data> addedImages, List<long> deletedImages)
        {
            try
            {
                //if (deletedImages != null)
                //    foreach (long idImage in deletedImages)
                //    {
                //        Data item = area.Data.First(i => i.ID == idImage);
                //        area.Data.Remove(item);
                //    }

                //if (addedImages != null)
                //    foreach (Data data in addedImages)
                //    {
                //        DataRepository _dataRepository = new DataRepository();
                //        _dataRepository.Add(data);
                //        if (area.Data == null)
                //            area.Data = new Collection<Data>();
                //        area.Data.Add(data);
                //    }

                //_actionDateRepository.Update(area);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка записи изображения ", ex);
                return false;
            }
            return true;
        }
    }
}

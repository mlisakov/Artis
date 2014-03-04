using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NLog;

namespace Artis.Data
{
    public class AreaRepository : BaseRepository<Area>
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static AreaRepository _area;

        static AreaRepository()
        {
            _area=new AreaRepository();
        }

        public static async Task<bool> Save(Area area, List<Data> addedImages, List<long> deletedImages)
        {
            try
            {
                if (deletedImages != null)
                    foreach (long idImage in deletedImages)
                    {
                        Data item = area.Data.First(i => i.ID == idImage);
                        area.Data.Remove(item);
                    }

                if (addedImages != null)
                    foreach (Data data in addedImages)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        _dataRepository.Add(data);
                        if (area.Data == null)
                            area.Data = new Collection<Data>();
                        area.Data.Add(data);
                    }

                _area.Update(area);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка записи изображения ", ex);
                return false;
            }
            return true;
        }

        public static async Task<bool> Add(Area area, List<Data> images)
        {
            try
            {

                if (images != null)
                    foreach (Data data in images)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        _dataRepository.Add(data);
                        if (area.Data == null)
                            area.Data = new Collection<Data>();
                        area.Data.Add(data);
                    }

                _area.Add(area);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка записи изображения ", ex);
                return false;
            }
            return true;
        }

        public static async Task<bool> Remove(Area area)
        {
            try
            {
                using (ISession session = Domain.Session)
                {
                    if (session.Query<ActionDate>().Any(i => i.Area.ID == area.ID))
                        return false;
                }
                _area.Delete(area);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("DataRequestFactory:Не удалось удалить сущность ", ex);
                return false;
            }
            return true;
        }
    }
}
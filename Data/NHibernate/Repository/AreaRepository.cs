using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Id;
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

        //private static AreaRepository _area;

        static AreaRepository()
        {
           // _area=new AreaRepository();
        }

        public async Task<bool> Save(Area area, List<string> addedImages, List<long> deletedImages)
        {
            try
            {
                if (deletedImages != null)
                {
                    Area currentArea = this.GetById(area.ID);
                    area.Data=new Collection<Data>(currentArea.Data.ToList());
                    foreach (long idImage in deletedImages)
                    {
                        Data item = area.Data.First(i => i.ID == idImage);
                        area.Data.Remove(item);
                    }
                }

                if (addedImages != null)
                    foreach (string image in addedImages)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        Data data=new Data(){Base64StringData = image};
                        _dataRepository.Add(data);
                        if (area.Data == null)
                            area.Data = new Collection<Data>();
                        area.Data.Add(data);
                    }

                Update(area);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка записи изображения ", ex);
                return false;
            }
            return true;
        }

        public async Task<long> Add(Area area, List<string> images)
        {
            try
            {

                if (images != null)
                    foreach (string image in images)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        Data data = new Data() { Base64StringData = image };
                        _dataRepository.Add(data);
                        if (area.Data == null)
                            area.Data = new Collection<Data>();
                        area.Data.Add(data);
                    }

                Add(area);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка добавления площадки ", ex);
                return -1;
            }
            return area.ID;
        }

        public async Task<bool> Remove(long id)
        {
            try
            {
                Area toDelete;
                using (ISession session = Domain.Session)
                {
                    //В случае, если площадка используется в мероприятиях не даем возможности удалить площадку.
                    if (session.Query<ActionDate>().Any(i => i.Area.ID == id))
                        return false;
                    toDelete = session.Query<Area>().First(i => i.ID == id);
                }
                Delete(toDelete);
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
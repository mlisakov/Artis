using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Artis.Data;
using NLog;

namespace Artis.Service
{
    public class ArtisAdminToolService : IArtisAdminTool
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private AreaRepository _areaRepository;

        public ArtisAdminToolService()
        {
            _areaRepository=new AreaRepository();
        }

        public async Task<string> GetAreaAsync(long idArea)
        {
            ObservableCollection<Area> originalAreas;
            try
            {
            if (idArea == -1)
                originalAreas = await DataRequestFactory.GetAreas();
            else
            {
                Area area = await DataRequestFactory.GetOriginalArea(idArea);
                originalAreas = new ObservableCollection<Area> {area};
            }
            AreasXmlProvider xmlAreasXmlProvider = new AreasXmlProvider(originalAreas);
            return xmlAreasXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить плолщадку с идентификатором:" +idArea,ex);
            }
            return string.Empty;
        }

        public async Task<string> GetSearchAreaAsync(string filter)
        {
            try
            {
                ObservableCollection<Area> originalAreas = await DataRequestFactory.GetAreas(filter);

                AreasXmlProvider xmlAreasXmlProvider = new AreasXmlProvider(originalAreas);
                return xmlAreasXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить площадки по имени:" + filter, ex);
            }
            return string.Empty;
        }

        public async Task<bool> SaveAreaAsync(string area, List<string> addedImages, List<long> deletedImages)
        {
            try
            {
            AreasXmlProvider xmlProvider=new AreasXmlProvider();
            Area originalArea=xmlProvider.FromXml(area).First();
            return await _areaRepository.Save(originalArea, addedImages, deletedImages);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить значения площадки!", ex);
            }
            return false;
        }

        public async Task<string> GetAreaImagesAsync(long idArea)
        {
            try
            {
                List<Data.Data> areaImages = await DataRequestFactory.GetImages("Area", idArea);
                DataXmlProvider xmlProvider = new DataXmlProvider(areaImages);
                return xmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить изображения для площадки с идентификатором:" + idArea, ex);
            }
            return string.Empty;
        }

        public async Task<long> AddAreaAsync(string area, List<string> images)
        {
            try
            {
                AreasXmlProvider xmlProvider = new AreasXmlProvider();
                Area originalArea = xmlProvider.FromXml(area).First();
                return await _areaRepository.Add(originalArea, images);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось создать площадку", ex);
            }
            return -1;
        }

        public async Task<bool> RemoveAreaAsync(long id)
        {
            try
            {
                return await _areaRepository.Remove(id);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось удалить площадку", ex);
            }
            return false;
        }

        public async Task<string> GetSearchActionsAsync(string filterName, string filterAreaName, DateTime fromDate, DateTime toDate)
        {
            try
            {
                ActionDateXmlProvider xmlAreasXmlProvider =
                    new ActionDateXmlProvider(
                        await DataRequestFactory.GetActions(filterName, filterAreaName, fromDate, toDate));
                return xmlAreasXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить мероприятия", ex);
            }
            return string.Empty;
        }

        public async Task<string> GetActionImagesAsync(long idAction)
        {
            try
            {
                List<Data.Data> areaImages = await DataRequestFactory.GetImages("Action", idAction);
                DataXmlProvider xmlProvider = new DataXmlProvider(areaImages);
                return xmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить изображения для мероприятия с идентификатором:" + idAction, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetActionActorsAsync(long idAction)
        {
            try
            {
                ActorsXmlProvider xmlAreasXmlProvider =
                     new ActorsXmlProvider(
                         await DataRequestFactory.GetActorsForAction(idAction));
                return xmlAreasXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить актеров для мероприятия:" + idAction, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetActionProducersAsync(long idAction)
        {
            try
            {
                ProducersXmlProvider xmlAreasXmlProvider =
                     new ProducersXmlProvider(
                         await DataRequestFactory.GetProducersForAction(idAction));
                return xmlAreasXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить режиссеров для мероприятия:" + idAction, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetGenresAsync()
        {
            try
            {
                GenreXmlProvider xmlAreasXmlProvider = new GenreXmlProvider(await DataRequestFactory.GetGenres());
                return xmlAreasXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить жанры", ex);
            }
            return string.Empty;
        }
    }
}

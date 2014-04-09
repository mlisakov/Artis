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
        private DataParser _dataParser;
        private AreaRepository _areaRepository;
        private GenreRepository _genreRepository;
        private ActionDateRepository _actionDateRepository;
        private GuiSectionRepository _guiSectionRepository;

        public ArtisAdminToolService()
        {
            _areaRepository = new AreaRepository();
            _genreRepository = new GenreRepository();
            _actionDateRepository = new ActionDateRepository();
            _guiSectionRepository = new GuiSectionRepository();
            _dataParser = new DataParser();
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
                AreasXmlProvider xmlProvider = new AreasXmlProvider();
                Area originalArea = xmlProvider.FromXml(area).First();
                return await _areaRepository.Save(originalArea, addedImages, deletedImages);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить значения площадки!", ex);
            }
            return false;
        }

        public async Task<bool> SaveActionDateAsync(string actionDate, List<string> addedImages, List<long> deletedImages,List<string> smallAddedImages, string actors, string producers)
        {
            try
            {
                ActionDateXmlProvider xmlProvider = new ActionDateXmlProvider();
                ActionDate originalactionDate = xmlProvider.FromXml(actionDate).First();
                ActorsXmlProvider actorsXmlProvider = new ActorsXmlProvider();
                ProducersXmlProvider producersXmlProvider = new ProducersXmlProvider();
                return await _actionDateRepository.Save(originalactionDate,
                       addedImages.Select(i => new Data.Data() { Base64StringData = i }).ToList(),
                       deletedImages, smallAddedImages.Select(i => new Data.Data() { Base64StringData = i }).ToList(), actorsXmlProvider.FromXml(actors), producersXmlProvider.FromXml(producers));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить значения мероприятия!", ex);
            }
            return false;
        }

        public async Task<bool> AddActionDateAsync(string actionDate)
        {
            try
            {
                ActionDateXmlProvider xmlProvider = new ActionDateXmlProvider();
                ActionDate originalactionDate = xmlProvider.FromXml(actionDate).First();

                return await _actionDateRepository.AddActionDate(originalactionDate);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить значения мероприятия!", ex);
            }
            return false;
        }

        public async Task<bool> CreateActionDateAsync(string actionDate, List<string> Images, string actors, string producers)
        {
            try
            {
                ActionDateXmlProvider xmlProvider = new ActionDateXmlProvider();
                ActionDate originalactionDate = xmlProvider.FromXml(actionDate).First();
                ActorsXmlProvider actorsXmlProvider = new ActorsXmlProvider();
                ProducersXmlProvider producersXmlProvider = new ProducersXmlProvider();
                return await _actionDateRepository.Add(originalactionDate,
                       Images.Select(i => new Data.Data() { Base64StringData = i }).ToList(),
                       actorsXmlProvider.FromXml(actors), producersXmlProvider.FromXml(producers));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить значения мероприятия!", ex);
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

        public async Task<long> AddGenreAsync(string genre)
        {
            try
            {
                var xmlProvider = new GenreXmlProvider();
                Genre originalGenre = xmlProvider.FromXml(genre).First();

                _genreRepository.Add(originalGenre);
                return originalGenre.ID;                
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось создать площадку", ex);
            }
            return -1;
        }

        public async Task<bool> SaveGenreAsync(string genre)
        {
            try
            {
                var xmlProvider = new GenreXmlProvider();
                var originalArea = xmlProvider.FromXml(genre).First();

                return await _genreRepository.Save(originalArea);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить значения площадки!", ex);
            }
            return false;
        }

        public async Task<bool> RemoveGenreAsync(long id)
        {
            try
            {
                return await _genreRepository.Remove(id);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось удалить жанр", ex);
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

        public async Task<string> GetActionSmallImagesAsync(long idAction)
        {
            try
            {
                List<Data.Data> areaImages = await DataRequestFactory.GetSmallImages(idAction);
                DataXmlProvider xmlProvider = new DataXmlProvider(areaImages);
                return xmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить маленькие изображения для мероприятия с идентификатором:" + idAction, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetActorImagesAsync(long idActor)
        {
            try
            {
                List<Data.Data> images = await DataRequestFactory.GetImages("Actor", idActor);
                DataXmlProvider xmlProvider = new DataXmlProvider(images);
                return xmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить изображения для актера с идентификатором:" + idActor, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetProducerImagesAsync(long idProducer)
        {
            try
            {
                List<Data.Data> images = await DataRequestFactory.GetImages("Producer", idProducer);
                DataXmlProvider xmlProvider = new DataXmlProvider(images);
                return xmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить изображения для продюсера с идентификатором:" + idProducer, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetProducers(string filter)
        {
            try
            {
                ObservableCollection<Producer> originalProducers = await DataRequestFactory.GetProducers(filter);

                ProducersXmlProvider producersXmlProvider = new ProducersXmlProvider(originalProducers);
                return producersXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить продюсеров по имени:" + filter, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetActors(string filter)
        {
            try
            {
                ObservableCollection<Actor> originalProducers = await DataRequestFactory.GetActors(filter);

                ActorsXmlProvider actorsXmlProvider = new ActorsXmlProvider(originalProducers);
                return actorsXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить актеров по имени:" + filter, ex);
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

        public async Task<string> GetGuiSectionGenres(long idSection)
        {
            try
            {
                 GenreXmlProvider xmlGenreXmlProvider =
                     new GenreXmlProvider(await DataRequestFactory.GetGuiSectionGenres(idSection));
                 return xmlGenreXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить жанры для секции GUI" + idSection, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetGuiSectionRestGenres(long idSection)
        {
            try
            {
                GenreXmlProvider xmlGenreXmlProvider =
                    new GenreXmlProvider(await DataRequestFactory.GetGuiSectionRestGenres(idSection));
                return xmlGenreXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить жанры для секции GUI" + idSection, ex);
            }
            return string.Empty;
        }

        public async Task<string> GetGuiSectios()
        {
            try
            {
                GuiSectionXmlProvider guiSectionXmlProvider =
                    new GuiSectionXmlProvider(await DataRequestFactory.GetGuiSections());
                return guiSectionXmlProvider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить категории GUI", ex);
            }
            return string.Empty;
        }

        public async Task<bool> UpdateGuiSectionGenres(long idSection, string innerXml)
        {
           try
            {
                GenreXmlProvider xmlGenreXmlProvider =new GenreXmlProvider();
                return await _guiSectionRepository.Save(idSection, xmlGenreXmlProvider.FromXml(innerXml));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось обновить жанры для секции GUI" + idSection, ex);
            }
            return false;
        }

        public async Task<int> ParseAction(string actionWeb)
        {
            try
            {
                ActionWebXmlProvider provider=new ActionWebXmlProvider();
                return await _dataParser.Parse(provider.FromXml(actionWeb).First());
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка записи загруженного мероприятия", ex);
            }
            return 0;
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

        public async Task<string> GetSearchGenreAsync(string filter)
        {
            try
            {
                ObservableCollection<Genre> originalGenres = await DataRequestFactory.GetGenres(filter);
                GenreXmlProvider provider = new GenreXmlProvider(originalGenres);

                return provider.ToXml().InnerXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось загрузить жанры по имени:" + filter, ex);
            }
            return string.Empty;
        }

        #region IArtisAdminTool Members

        #endregion
    }
}

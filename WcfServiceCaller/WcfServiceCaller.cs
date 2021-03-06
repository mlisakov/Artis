﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Artis.Consts;
using Artis.Service;
using NLog;

namespace Artis.Data
{
    public class WcfServiceCaller
    {
        private IArtisAdminTool serviceAdminTool = null;
        
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        public WcfServiceCaller()
        {
            BasicHttpBinding myBinding = new BasicHttpBinding();
            EndpointAddress myEndpoint = new EndpointAddress(ServiceAddress.ArtisConnectionString+"ArtisAdminToolService.svc");            

            myBinding.CloseTimeout = TimeSpan.FromSeconds(80000);
            myBinding.ReceiveTimeout = TimeSpan.FromSeconds(80000);
            myBinding.SendTimeout = TimeSpan.FromSeconds(80000);
            myBinding.MaxBufferSize = Int32.MaxValue;

            myBinding.MaxBufferPoolSize = Int32.MaxValue;
            myBinding.MaxReceivedMessageSize = Int32.MaxValue;

            myBinding.ReaderQuotas.MaxDepth = 32;
            myBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            myBinding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
            myBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;


            ChannelFactory<IArtisAdminTool> myChannelFactory = new ChannelFactory<IArtisAdminTool>(myBinding, myEndpoint);
            serviceAdminTool = myChannelFactory.CreateChannel();
        }

        public async Task<ObservableCollection<Area>> GetAreas(long id)
        {
            try
            {
                string result = await serviceAdminTool.GetAreaAsync(id);
                AreasXmlProvider provider = new AreasXmlProvider();
                return new ObservableCollection<Area>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить площадки!",ex);
                throw new Exception("Не удалось получить площадки!Проверьте правильность адреса административного сервиса!");
            }

        }

        public async Task<ObservableCollection<Genre>> GetGenres()
        {
            try
            {
                string result = await serviceAdminTool.GetGenresAsync();
                GenreXmlProvider provider = new GenreXmlProvider();
                return new ObservableCollection<Genre>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить данры!", ex);
                throw new Exception("Не удалось получить площадки! Проверьте правильность адреса административного сервиса!");
            }

        }

        public async Task<ObservableCollection<Genre>> GetGenres(string filter)
        {
            try
            {
                string result = await serviceAdminTool.GetSearchGenreAsync(filter);
                GenreXmlProvider provider = new GenreXmlProvider();
                
                return new ObservableCollection<Genre>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить жанры!", ex);
                throw new Exception("Не удалось получить площадки! Проверьте правильность адреса административного сервиса!");
            }

        }

        public async Task<ObservableCollection<Area>> GetAreas(string filter)
        {
            try
            {
                string result = await serviceAdminTool.GetSearchAreaAsync(filter);
                AreasXmlProvider provider = new AreasXmlProvider();
                return new ObservableCollection<Area>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить площадки!", ex);
                throw new Exception("Не удалось получить площадки!Проверьте правильность адрес административного сервиса!");
            }

        }

        public async Task<bool> SaveArea(Area currentArea, List<Data> addedImages, List<long> deletedImages)
        {
            try
            {
            AreasXmlProvider areasXmlProvider=new AreasXmlProvider(new List<Area>(){currentArea});
           return
                await
                    serviceAdminTool.SaveAreaAsync(areasXmlProvider.ToXml().InnerXml, addedImages.Select(i => i.Base64StringData).ToList(),
                        deletedImages);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить площадкy!", ex);
                throw new Exception("Не удалось сохранить площадкy!");
            }
        }

        //public async Task<bool> SaveActor(Actor currentActor, List<Data> addedImages, List<long> deletedImages)
        //{
        //    try
        //    {
        //        ActorsXmlProvider actorsXmlProvider = new ActorsXmlProvider(new List<Actor>() { currentActor });
        //        return
        //             await
        //                 serviceAdminTool.SaveActorAsync(actorsXmlProvider.ToXml().InnerXml, addedImages.Select(i => i.Base64StringData).ToList(),
        //                     deletedImages);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("Не удалось сохранить площадкy!", ex);
        //        throw new Exception("Не удалось сохранить площадкy!");
        //    }
        //}

        public async Task<bool> SaveGenre(Genre currentArea)
        {
            try
            {
                var provider = new GenreXmlProvider(new List<Genre>() { currentArea });
                return
                    await
                        serviceAdminTool.SaveGenreAsync(provider.ToXml().InnerXml);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить площадкy!", ex);
                throw new Exception("Не удалось сохранить площадкy!");
            }
        }

        public async Task<bool> SaveActionDate(ActionDate currentActionDate, 
            List<Data> addedImages, 
            List<long> deletedImages,
            List<Data> addedSmallImages,
            List<Actor> actors,
            List<Producer> producers)
        {
            try
            {
                ActionDateXmlProvider actionDateXmlProvider = new ActionDateXmlProvider(new List<ActionDate>() { currentActionDate });
                ActorsXmlProvider actorsXmlProvider = new ActorsXmlProvider( actors );
                ProducersXmlProvider producerXmlProvider = new ProducersXmlProvider(producers);
                var t = producerXmlProvider.ToXml().InnerXml;
                 _logger.Debug(t);
                Debug.WriteLine(t);
                return
                    await
                        serviceAdminTool.SaveActionDateAsync(
                            actionDateXmlProvider.ToXml().InnerXml,
                            addedImages.Select(i => i.Base64StringData).ToList(),
                            deletedImages,
                            addedSmallImages.Select(i => i.Base64StringData).ToList(),
                            actorsXmlProvider.ToXml().InnerXml,
                            producerXmlProvider.ToXml().InnerXml);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить площадкy!", ex);
                throw new Exception("Не удалось сохранить площадкy!");
            }
        }

        public async Task<bool> AddActionDate(ActionDate currentActionDate)
        {
            try
            {
                ActionDateXmlProvider actionDateXmlProvider =
                    new ActionDateXmlProvider(new List<ActionDate>() {currentActionDate});
                return
                    await
                        serviceAdminTool.AddActionDateAsync(
                            actionDateXmlProvider.ToXml().InnerXml);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить мероприятие!", ex);
                throw new Exception("Не удалось сохранить мероприятиеy!");
            }
        }

        public async Task<bool> AddActionDate(ActionDate currentActionDate, List<string> images, List<Actor> actors,List<Producer> producers)
        {
            try
            {
                ActionDateXmlProvider actionDateXmlProvider =
                    new ActionDateXmlProvider(new List<ActionDate>() { currentActionDate });
                ActorsXmlProvider actorsXmlProvider = new ActorsXmlProvider(actors);
                ProducersXmlProvider producerXmlProvider = new ProducersXmlProvider(producers);
                return
                    await
                        serviceAdminTool.CreateActionDateAsync(
                            actionDateXmlProvider.ToXml().InnerXml, images, actorsXmlProvider.ToXml().InnerXml, producerXmlProvider.ToXml().InnerXml);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить мероприятие!", ex);
                throw new Exception("Не удалось сохранить мероприятиеy!");
            }
        }

        public async Task<ObservableCollection<Data>> GetAreaImages(long idArea)
        {
            try
            {
                string result = await serviceAdminTool.GetAreaImagesAsync(idArea);
                DataXmlProvider provider = new DataXmlProvider();
                return new ObservableCollection<Data>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить изображения!", ex);
                throw new Exception("Не удалось получить изображения!");
            }
        }

        public async Task<long> AddArea(Area currentArea, List<Data> images)
        {
            try
            {
                AreasXmlProvider areasXmlProvider = new AreasXmlProvider(new List<Area>() {currentArea});
                return await
                    serviceAdminTool.AddAreaAsync(areasXmlProvider.ToXml().InnerXml,
                        images.Select(i => i.Base64StringData).ToList());
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось добавить площадку!", ex);
                throw new Exception("Не удалось добавить площадку!");
            }

        }

        public async Task<bool> RemoveArea(long id)
        {
            try
            {
                return await serviceAdminTool.RemoveAreaAsync(id);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось удалить площадку!", ex);
                throw new Exception("Не удалось удалить площадку!");
            }

        }

        public async Task<long> AddGenre(Genre currentGenre)
        {
            try
            {
                var areasXmlProvider = new GenreXmlProvider(new List<Genre> { currentGenre });
                return await
                    serviceAdminTool.AddGenreAsync(areasXmlProvider.ToXml().InnerXml);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось добавить жанр!", ex);
                throw new Exception("Не удалось добавить жанр!");
            }

        }

        public async Task<bool> RemoveGenre(long id)
        {
            try
            {
                return await serviceAdminTool.RemoveGenreAsync(id);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось удалить жанр!", ex);
                throw new Exception("Не удалось удалить жанр!");
            }
        }

        public async Task<ObservableCollection<ActionDate>> GetActions(string Name, Area Area, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string filterName=string.Empty, filterArea=string.Empty;
                DateTime filterStartDate = DateTime.MinValue, filterFinishDate = DateTime.MaxValue;
                if (Name != null)
                    filterName = Name;
                if (Area != null)
                    filterArea = Area.Name;
                if (fromDate != null)
                    filterStartDate = fromDate.Value;
                if (toDate != null)
                    filterFinishDate = toDate.Value;

                string result = await serviceAdminTool.GetSearchActionsAsync(filterName, filterArea, filterStartDate, filterFinishDate);
                ActionDateXmlProvider provider = new ActionDateXmlProvider();
                return new ObservableCollection<ActionDate>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить мероприятия!", ex);
                throw new Exception("Не удалось получить мероприятия!Проверьте правильность адрес административного сервиса!");
            }

        }

        public async Task<ObservableCollection<Data>> GetActionImages(long idAction)
        {
            try
            {
                string result = await serviceAdminTool.GetActionImagesAsync(idAction);
                DataXmlProvider provider = new DataXmlProvider();
                return new ObservableCollection<Data>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить изображения!", ex);
                throw new Exception("Не удалось получить изображения!");
            }
        }

        public async Task<ObservableCollection<Data>> GetActionSmallImages(long idAction)
        {
            try
            {
                string result = await serviceAdminTool.GetActionSmallImagesAsync(idAction);
                DataXmlProvider provider = new DataXmlProvider();
                return new ObservableCollection<Data>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить изображения!", ex);
                throw new Exception("Не удалось получить изображения!");
            }
        }

        public async Task<ObservableCollection<Data>> GetActorImages(long idActor)
        {
            try
            {
                string result = await serviceAdminTool.GetActorImagesAsync(idActor);
                DataXmlProvider provider = new DataXmlProvider();
                return new ObservableCollection<Data>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить изображения!", ex);
                throw new Exception("Не удалось получить изображения!");
            }
        }

        public async Task<ObservableCollection<Actor>> GetActionActors(long idAction)
        {
            try
            {
                string result = await serviceAdminTool.GetActionActorsAsync(idAction);
                if (string.IsNullOrEmpty(result))
                    return new ObservableCollection<Actor>();
                ActorsXmlProvider provider = new ActorsXmlProvider();
                return new ObservableCollection<Actor>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить актеров!", ex);
                throw new Exception("Не удалось получить актеров!");
            }
        }

        public async Task<ObservableCollection<Producer>> GetActionProducers(long idAction)
        {
            try
            {
                string result = await serviceAdminTool.GetActionProducersAsync(idAction);
                ProducersXmlProvider provider = new ProducersXmlProvider();
                return new ObservableCollection<Producer>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить актеров!", ex);
                throw new Exception("Не удалось получить актеров!");
            }
        }

        public async Task<ObservableCollection<Genre>> GetGuiSectionGenres(long idSection)
        {
            try
            {
                string result = await serviceAdminTool.GetGuiSectionGenres(idSection);
                GenreXmlProvider provider = new GenreXmlProvider();
                return new ObservableCollection<Genre>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить список жанров для категории!"+idSection, ex);
                throw new Exception("Не удалось получить список жанров для категории!");
            }
        }

        public async Task<ObservableCollection<Genre>> GetGuiSectionRestGenres(long idSection)
        {
            try
            {
                string result = await serviceAdminTool.GetGuiSectionRestGenres(idSection);
                GenreXmlProvider provider = new GenreXmlProvider();
                return new ObservableCollection<Genre>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить список жанров, не выбранных для категории!" + idSection, ex);
                throw new Exception("Не удалось получить список жанров, не выбранных для категории!");
            }
        }

        public async Task<ObservableCollection<GuiSection>> GetGuiSections()
        {
            try
            {
                string result = await serviceAdminTool.GetGuiSectios();
                GuiSectionXmlProvider provider = new GuiSectionXmlProvider();
                return new ObservableCollection<GuiSection>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить список категорий GUI!", ex);
                throw new Exception("Не удалось получить список  категорий GUI!");
            }
        }

        public async Task<bool> UpdateGuiSectionGenres(long idSection,ObservableCollection<Genre> usedGenres)
        {
            try
            {
                GenreXmlProvider provider = new GenreXmlProvider(usedGenres);
                return await serviceAdminTool.UpdateGuiSectionGenres(idSection,provider.ToXml().InnerXml);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сохранить список жанров!", ex);
                throw new Exception("Не удалось сохранить список жанров!");
            }
        }

        public async Task<int> ParseActionAsync(ActionWeb actionWeb)
        {
            try
            {
                ActionWebXmlProvider provider = new ActionWebXmlProvider(new List<ActionWeb>(){actionWeb});
                return await serviceAdminTool.ParseAction(provider.ToXml().InnerXml);
            }
            catch (Exception ex )
            {
                _logger.ErrorException("Не удалось сохранить загруженное мероприятие!", ex);
                throw new Exception("Не удалось сохранить загруженное мероприятие!");
            }
        }

        public async Task<ObservableCollection<Producer>> GetProducersAsync(string filter="")
        {
            try
            {
                string result = await serviceAdminTool.GetProducers(filter);
                ProducersXmlProvider provider = new ProducersXmlProvider();
                return new ObservableCollection<Producer>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить список продюсеров!", ex);
                throw new Exception("Не удалось получить список продюсеров!");
            }
        }

        public async Task<ObservableCollection<Actor>> GetActorsAsync(string filter = "")
        {
            try
            {
                string result = await serviceAdminTool.GetActors(filter);
                ActorsXmlProvider provider = new ActorsXmlProvider();
                return new ObservableCollection<Actor>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить список продюсеров!", ex);
                throw new Exception("Не удалось получить список продюсеров!");
            }
        }
    }
}

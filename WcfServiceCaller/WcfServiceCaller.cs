using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
            EndpointAddress myEndpoint = new EndpointAddress("http://localhost:55601/ArtisAdminToolService.svc");

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
                throw new Exception("Не удалось получить площадки!Проверьте правильность адрес административного сервиса!");
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

        public async Task<ObservableCollection<Data>> GetAreaImages(long idArea)
        {
            try
            {
                string result = await serviceAdminTool.GetAreaImages(idArea);
                DataXmlProvider provider = new DataXmlProvider();
                return new ObservableCollection<Data>(provider.FromXml(result));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось получить изображения!", ex);
                throw new Exception("Не удалось получить изображения!");
            }
        }
    }
}

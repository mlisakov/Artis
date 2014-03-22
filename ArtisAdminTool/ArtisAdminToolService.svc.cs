using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Artis.Consts;
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

        public async Task<string> GetAreaImages(long idArea)
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
    }
}

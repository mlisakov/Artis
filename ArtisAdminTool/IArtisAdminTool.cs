using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Artis.Data;

namespace Artis.Service
{
    [ServiceContract]
    public interface IArtisAdminTool
    {
        /// <summary>
        /// Получение Площадки по ее идентификатору
        /// </summary>
        /// <param name="idArea">Идентификатор площадки. Если он будет пустой-вернуться все площадки</param>
        /// <returns>Сериализованное описание площадки</returns>
        [OperationContract]
        Task<string> GetAreaAsync(long idArea);

        /// <summary>
        /// Поиск площадки по имени
        /// </summary>
        /// <param name="filter">Параметр фильтра</param>
        /// <returns>Сериализованное описание площадок</returns>
        [OperationContract]
        Task<string> GetSearchAreaAsync(string filter);

        /// <summary>
        /// Сохранение изменений в площадке
        /// </summary>
        /// <param name="area">xml описание площадки</param>
        /// <param name="addedImages">Новые изображения</param>
        /// <param name="deletedImages">Удаленные изображения</param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> SaveAreaAsync(string area, List<string> addedImages, List<long> deletedImages);

        /// <summary>
        /// Получение изображений для площадки
        /// </summary>
        /// <param name="idArea">Идентификатор площадки</param>
        /// <returns>Сериализованный список изображения</returns>
        [OperationContract]
        Task<string> GetAreaImages(long idArea);
    }
}

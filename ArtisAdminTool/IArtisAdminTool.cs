using System;
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
        /// Получение жанров 
        /// </summary>
        /// <returns>Сериализованное описание жанров</returns>
        [OperationContract]
        Task<string> GetGenresAsync();

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
        Task<string> GetAreaImagesAsync(long idArea);

        /// <summary>
        /// Добавление новой площадки
        /// </summary>
        /// <param name="area">xml описание площадки</param>
        /// <param name="images">изображения</param>
        /// <returns></returns>
        [OperationContract]
        Task<long> AddAreaAsync(string area, List<string> images);

        /// <summary>
        /// Удаление площадки
        /// </summary>
        /// <param name="id">Идентификатор площадки</param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> RemoveAreaAsync(long id);

        /// <summary>
        /// Поиск мероприятий
        /// </summary>
        /// <param name="filterName">Фильтр для имени</param>
        /// <param name="filterAreaName">Фильтр по площадке</param>
        /// <param name="fromDate">Фильтр по дате начала</param>
        /// <param name="toDate">Фильтр по дате окончания</param>
        /// <returns></returns>
        [OperationContract]
        Task<string> GetSearchActionsAsync(string filterName, string filterAreaName, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Получение изображений для мероприятия
        /// </summary>
        /// <param name="idAction">Идентификатор мероприятия</param>
        /// <returns>Сериализованный список изображения</returns>
        [OperationContract]
        Task<string> GetActionImagesAsync(long idAction);

        /// <summary>
        /// Получение актеров для мероприятия
        /// </summary>
        /// <param name="idAction">Идентификатор мероприятия</param>
        /// <returns>Сериализованный список актеров</returns>
        [OperationContract]
        Task<string> GetActionActorsAsync(long idAction);

        /// <summary>
        /// Получение режисеров для мероприятия
        /// </summary>
        /// <param name="idAction">Идентификатор мероприятия</param>
        /// <returns>Сериализованный список режисеров</returns>
        [OperationContract]
        Task<string> GetActionProducersAsync(long idAction);
    }
}

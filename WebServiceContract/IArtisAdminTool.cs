using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;

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
        /// Поиск жанра по имени
        /// </summary>
        /// <param name="filter">Параметр фильтра</param>
        /// <returns>Сериализованное описание площадок</returns>
        [OperationContract]
        Task<string> GetSearchGenreAsync(string filter);

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
        /// Сохранение изменений в жанре
        /// </summary>
        /// <param name="genre">xml описание жанра</param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> SaveGenreAsync(string genre);

        /// <summary>
        /// Сохранение изменений по мероприятию
        /// </summary>
        /// <param name="actionDate">Мероприятие</param>
        /// <param name="addedImages">Новые изображения</param>
        /// <param name="deletedImages">Удаленные изображения</param>
        /// <param name="actors">Сериализованный список актеров</param>
        /// <param name="producers">Сериализованный список режисеров</param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> SaveActionDateAsync(string actionDate, List<string> addedImages, List<long> deletedImages,List<string> smallAddedImages, string actors,string producers);

        /// <summary>
        /// Добавление новой даты проведения мероприятия
        /// </summary>
        /// <param name="innerXml">Сереализованное мероприятие</param>
        /// <returns>Результат добавления</returns>
        [OperationContract]
        Task<bool> AddActionDateAsync(string innerXml);

        /// <summary>
        /// Добавление нового мероприятия
        /// </summary>
        /// <param name="actionDate">Сереализованное мероприятие</param>
        /// <param name="images">Изображения</param>
        /// <param name="actors">Сериализованный список актеров</param>
        /// <param name="producers">Сериализованный список режиссеров</param>
        /// <returns>Результат добавления</returns>
        [OperationContract]
        Task<bool> CreateActionDateAsync(string actionDate, List<string> images, string actors, string producers);

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
        /// Добавление новой жанра
        /// </summary>
        /// <param name="genre">xml описание жанра</param>
        /// <returns></returns>
        [OperationContract]
        Task<long> AddGenreAsync(string genre);

        /// <summary>
        /// Удаление жанра
        /// </summary>
        /// <param name="id">Идентификатор жанра</param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> RemoveGenreAsync(long id);

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
        /// Получение маленьких изображений для мероприятия
        /// </summary>
        /// <param name="idAction">Идентификатор мероприятия</param>
        /// <returns>Сериализованный список изображения</returns>
        [OperationContract]
        Task<string> GetActionSmallImagesAsync(long idAction);

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

        /// <summary>
        /// Получение списка жанров для категории GUI
        /// </summary>
        /// <param name="idSection">Идентификатор категории GUI</param>
        /// <returns>Сериализованный список жанров</returns>
        [OperationContract]
        Task<string> GetGuiSectionGenres(long idSection);

        /// <summary>
        /// Получение списка жанров, не выбранных для категории GUI
        /// </summary>
        /// <param name="idSection">Идентификатор категории GUI</param>
        /// <returns>Сериализованный список жанров</returns>
        [OperationContract]
        Task<string> GetGuiSectionRestGenres(long idSection);

        /// <summary>
        /// Обновление списка жанров для категории
        /// </summary>
        /// <param name="idSection">Идентификатор секции</param>
        /// <param name="innerXml">Список жанров для категории</param>
        /// <returns>Результат обновления</returns>
        [OperationContract]
        Task<bool> UpdateGuiSectionGenres(long idSection,string innerXml);

        /// <summary>
        /// Запись загруженного мероприятия в БД
        /// </summary>
        /// <param name="actionWeb">Сериализованное мероприятие</param>
        /// <returns>Результат записи
        /// -1 - Фатальная ошибка
        /// 0 - Не удалось записать мероприятие
        /// 1 - Запись прошла успешно
        /// </returns>
        [OperationContract]
        Task<int> ParseAction(string actionWeb);

        /// <summary>
        /// Получение списка категорий GUI(Театр, Цирк, Экскурсия....)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Task<string> GetGuiSectios();

        /// <summary>
        /// Получение списка изображений для актера
        /// </summary>
        /// <param name="idActor">Идентификатор актера</param>
        /// <returns></returns>
        [OperationContract]
        Task<string> GetActorImagesAsync(long idActor);

        /// <summary>
        /// Получение списка изображений для продюсера
        /// </summary>
        /// <param name="idProducer">Идентификатор продюсера</param>
        /// <returns></returns>
        [OperationContract]
        Task<string> GetProducerImagesAsync(long idProducer);

        /// <summary>
        /// Поиск продюсеров
        /// </summary>
        /// <param name="filter">Фильтрующий параметр для ФИО</param>
        /// <returns></returns>
        [OperationContract]
        Task<string> GetProducers(string filter);

        /// <summary>
        /// Поиск актеров
        /// </summary>
        /// <param name="filter">Фильтрующий параметр для ФИО</param>
        /// <returns></returns>
        [OperationContract]
        Task<string> GetActors(string filter);
    }
}

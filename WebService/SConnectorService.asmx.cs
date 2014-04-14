#define DEBUG

using System;
using System.Web.Services;
using Artis.Data;
using NLog;

namespace Artis.Service
{
    [WebService(Namespace = "http://obsspb.ru/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class SConnectorService : WebService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Получение мероприятий
        /// </summary>
        /// <param name="startDate">Начальная дата фильтра для даты проведения мероприятия</param>
        /// <param name="finishDate">Конечная дата фильтра для даты проведения мероприятия</param>
        /// <returns></returns>
        [WebMethod]
        public string GetAction(string startDate,string finishDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(finishDate))
                {
                    _logger.Error("WebService.GetAction:Пустая дата! Дата начала-" + startDate + ";Дата окончания-" + finishDate);
                    return "Error";
                }

                DateTime parsedStartDate;
                DateTime parsedEndDate;

                if (!DateTime.TryParse(startDate, out parsedStartDate))
                {
                    _logger.Error("WebService.GetAction:Ошибка распознования строки " + startDate);
                    return "Error";
                }

                if (!DateTime.TryParse(finishDate, out parsedEndDate))
                {
                    _logger.Error("WebService.GetAction:Ошибка распознования строки " + finishDate);
                    return "Error";
                }

                string actions = DataRequestFactory.GetAction(parsedStartDate, parsedEndDate);

                return actions;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetAction:Ошибка получения мероприятий", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение мероприятий
        /// </summary>
        /// <param name="startDate">Начальная дата фильтра для даты проведения мероприятия</param>
        /// <param name="finishDate">Конечная дата фильтра для даты проведения мероприятия</param>
        /// <param name="topCount">Кол-во мероприятий</param>
        /// <returns></returns>
        [WebMethod]
        public string GetTopAction(string startDate, string finishDate,int topCount)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(finishDate))
                {
                    _logger.Error("WebService.GetTopAction:Пустая дата! Дата начала-" + startDate + ";Дата окончания-" + finishDate);
                    return "Error";
                }

                DateTime parsedStartDate;
                DateTime parsedEndDate;

                if (!DateTime.TryParse(startDate, out parsedStartDate))
                {
                    _logger.Error("WebService.GetTopAction:Ошибка распознования строки " + startDate);
                    return "Error";
                }

                if (!DateTime.TryParse(finishDate, out parsedEndDate))
                {
                    _logger.Error("WebService.GetTopAction:Ошибка распознования строки " + finishDate);
                    return "Error";
                }

                string actions = DataRequestFactory.GetTopAction(parsedStartDate, parsedEndDate, topCount);

                return actions;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetTopAction:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение экскурсий
        /// </summary>
        /// <param name="startDate">Начальная дата фильтра для даты проведения мероприятия</param>
        /// <param name="finishDate">Конечная дата фильтра для даты проведения мероприятия</param>
        /// <param name="topCount">Кол-во экскурсий</param>
        /// <returns></returns>
        //[WebMethod]
        //public string GetTopTour(string startDate, string finishDate, int topCount)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(finishDate))
        //        {
        //            _logger.Error("WebService.GetTopTour:Пустая дата! Дата начала-" + startDate + ";Дата окончания-" + finishDate);
        //            return "Error";
        //        }

        //        DateTime parsedStartDate;
        //        DateTime parsedEndDate;

        //        if (!DateTime.TryParse(startDate, out parsedStartDate))
        //        {
        //            _logger.Error("WebService.GetTopTour:Ошибка распознования строки " + startDate);
        //            return "Error";
        //        }

        //        if (!DateTime.TryParse(finishDate, out parsedEndDate))
        //        {
        //            _logger.Error("WebService.GetTopTour:Ошибка распознования строки " + finishDate);
        //            return "Error";
        //        }

        //        string actions = DataRequestFactory.GetTop(parsedStartDate, parsedEndDate, topCount, "Экскурсии");

        //        return actions;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("WebService.GetTopTour:Ошибка!", ex);
        //        _logger.Error(ex.Message + " " + ex.StackTrace);
        //    }
        //    return "Error";
        //}

        /// <summary>
        /// Получение концертов
        /// </summary>
        /// <param name="startDate">Начальная дата фильтра для даты проведения мероприятия</param>
        /// <param name="finishDate">Конечная дата фильтра для даты проведения мероприятия</param>
        /// <param name="topCount">Кол-во концертов</param>
        /// <returns></returns>
        //[WebMethod]
        //public string GetTopConcert(string startDate, string finishDate, int topCount)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(finishDate))
        //        {
        //            _logger.Error("WebService.GetTopConcert:Пустая дата! Дата начала-" + startDate + ";Дата окончания-" + finishDate);
        //            return "Error";
        //        }

        //        DateTime parsedStartDate;
        //        DateTime parsedEndDate;

        //        if (!DateTime.TryParse(startDate, out parsedStartDate))
        //        {
        //            _logger.Error("WebService.GetTopConcert:Ошибка распознования строки " + startDate);
        //            return "Error";
        //        }

        //        if (!DateTime.TryParse(finishDate, out parsedEndDate))
        //        {
        //            _logger.Error("WebService.GetTopConcert:Ошибка распознования строки " + finishDate);
        //            return "Error";
        //        }

        //        string actions = DataRequestFactory.GetTop(parsedStartDate, parsedEndDate, topCount, "Концерты");

        //        return actions;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("WebService.GetTopConcert:Ошибка!", ex);
        //        _logger.Error(ex.Message + " " + ex.StackTrace);
        //    }
        //    return "Error";
        //}

        /// <summary>
        /// Получение цирковых представлений
        /// </summary>
        /// <param name="startDate">Начальная дата фильтра для даты проведения мероприятия</param>
        /// <param name="finishDate">Конечная дата фильтра для даты проведения мероприятия</param>
        /// <param name="topCount">Кол-во концертов</param>
        /// <returns></returns>
        //[WebMethod]
        //public string GetTopСircus(string startDate, string finishDate, int topCount)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(finishDate))
        //        {
        //            _logger.Error("WebService.GetTopСircus:Пустая дата! Дата начала-" + startDate + ";Дата окончания-" + finishDate);
        //            return "Error";
        //        }

        //        DateTime parsedStartDate;
        //        DateTime parsedEndDate;

        //        if (!DateTime.TryParse(startDate, out parsedStartDate))
        //        {
        //            _logger.Error("WebService.GetTopСircus:Ошибка распознования строки " + startDate);
        //            return "Error";
        //        }

        //        if (!DateTime.TryParse(finishDate, out parsedEndDate))
        //        {
        //            _logger.Error("WebService.GetTopСircus:Ошибка распознования строки " + finishDate);
        //            return "Error";
        //        }

        //        string actions = DataRequestFactory.GetTop(parsedStartDate, parsedEndDate, topCount, "Цирк");

        //        return actions;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("WebService.GetTopConcert:Ошибка!", ex);
        //        _logger.Error(ex.Message + " " + ex.StackTrace);
        //    }
        //    return "Error";
        //}

        /// <summary>
        /// Получение информации по мероприятию
        /// </summary>
        /// <param name="Id">Идентификатор даты проведения мероприятия</param>
        /// <returns></returns>
        [WebMethod]
        public string GetActionInfo(long Id)
        {
            try
            {
                if (Id <= 0)
                {
                    _logger.Error("WebService.GetActionInfo:Идентификатор мероприятия должен быть больше нуля!");
                    return "Error";
                }
                   
                string action = DataRequestFactory.GetAction(Id);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetActionInfo:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение изображений для мероприятия
        /// </summary>
        /// <param name="Id">Идентификатор мероприятия</param>
        /// <returns></returns>
        [WebMethod]
        public string GetActionImage(long Id)
        {
            try
            {
                if (Id <= 0)
                {
                    _logger.Error("WebService.GetActionImage:Идентификатор мероприятия должен быть больше нуля!");
                    return "Error";
                }

                string action = DataRequestFactory.GetActionImages(Id);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetActionImage:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение площадки
        /// </summary>
        /// <param name="Id">Идентификатор площадки</param>
        /// <returns></returns>
        [WebMethod]
        public string GetArea(long Id)
        {
            try
            {
                if (Id <= 0)
                {
                    _logger.Error("WebService.GetArea:Идентификатор площадки должен быть больше нуля!");
                    return "Error";
                }

                string action = DataRequestFactory.GetArea(Id);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetArea:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение изображений для площадки
        /// </summary>
        /// <param name="Id">Идентификатор площадки</param>
        /// <returns></returns>
        [WebMethod]
        public string GetAreaImage(long Id)
        {
            try
            {
                if (Id <= 0)
                {
                    _logger.Error("WebService.GetAreaImage:Идентификатор площадки должен быть больше нуля!");
                    return "Error";
                }

                string action = DataRequestFactory.GetAreaImages(Id);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetArea:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение мероприятий для площадки
        /// </summary>
        /// <param name="idArea">Идентификатор площадки</param>
        /// <param name="startDate">Минимальная дата проведения мероприятия</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="page">Номер страницы</param>
        /// <returns>Сериализованный список меорприятий</returns>
        [WebMethod]
        public string GetAreaActions(long idArea, string startDate, int pageSize, int page)
        {
            try
            {
                if (idArea <= 0)
                {
                    _logger.Error("WebService.GetAreaActions:Идентификатор площадки должен быть больше нуля!");
                    return "Error";
                }
                DateTime parsedStartDate;

                if (!DateTime.TryParse(startDate, out parsedStartDate))
                {
                    _logger.Error("WebService.GetAreaActions:Ошибка распознования строки " + startDate);
                    return "Error";
                }
                string action = DataRequestFactory.GetAreaActions(idArea, parsedStartDate, pageSize, page);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetAreaActions:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение списка площадок
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetAllArea()
        {
            try
            {
                string area = DataRequestFactory.GetAllArea();
                return area;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetAllArea:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение мероприятий
        /// </summary>
        /// <param name="ActionName">название или часть названия</param>
        /// <param name="idGenre">Жанр</param>
        /// <param name="idArea">Площадка</param>
        /// <param name="startDate">Начальная дата для фильтра по дате начала мероприятия</param>
        /// <param name="finishDate">Конечная дата для фильтра по дате начала мероприятия</param>
        /// <param name="PageSize">Размер страниц</param>
        /// <param name="Page">Номер страницы</param>
        /// <returns></returns>
        [WebMethod]
        public string GetActions(string ActionName, long idGenre, long idArea, string startDate,string finishDate, int PageSize, int Page)
        {
            try
            {
                int page = Page;
                int pageSize = PageSize;
                DateTime dateStart = DateTime.MinValue;
                DateTime dateFinish = DateTime.MinValue;
                if (!string.IsNullOrEmpty(startDate) && !DateTime.TryParse(startDate, out dateStart))
                    return "Error.Parse Date";

                if (!string.IsNullOrEmpty(finishDate) && !DateTime.TryParse(finishDate, out dateFinish))
                    return "Error.Parse Date";

                if (page == 0)
                    page = 1;

                if (pageSize == 0)
                    pageSize = 1;

                string action = DataRequestFactory.GetActions(ActionName, idGenre, idArea, dateStart,dateFinish, pageSize, page);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetActions:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение списка возможных дат проведения мероприятий и соответственных площадок
        /// </summary>
        /// <param name="idActionDate">Идентификатор даты проведения мероприятие</param>
        /// <param name="startDate">Минимальная дата начала</param>
        /// <param name="PageSize">Размер страниц</param>
        /// <param name="Page">Номер страницы</param>
        /// <returns></returns>
        [WebMethod]
        public string GetActionAreas(long idActionDate, string startDate, int PageSize, int Page)
        {
            try
            {
                int page = Page;
                int pageSize = PageSize;
                DateTime date = DateTime.MinValue;
                if (!string.IsNullOrEmpty(startDate) && !DateTime.TryParse(startDate, out date))
                    return "Error.Parse Date";

                if (page == 0)
                    page = 1;

                if (pageSize == 0)
                    pageSize = 1;

                string action = DataRequestFactory.GetActions(idActionDate, date, pageSize, page);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetActions:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение мероприятий, в которых участвует человек
        /// </summary>
        /// <param name="idPeople">Идентификатор человека</param>
        /// <param name="isActor">Является ли человек актером</param>
        /// <param name="startDate">Начальная дата для фильтрации мероприятий</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <returns>Список мероприятий</returns>
        [WebMethod]
        public string GetPeopleActions(long idPeople, bool isActor, string startDate, int pageSize, int pageNumber)
        {
            try
            {
                int page = pageNumber;
                int pageSizeLocal = pageSize;
                DateTime date = DateTime.MinValue;
                if (!string.IsNullOrEmpty(startDate) && !DateTime.TryParse(startDate, out date))
                    return "Error.Parse Date";

                if (page == 0)
                    page = 1;

                if (pageSizeLocal == 0)
                    pageSizeLocal = 1;

                string action = DataRequestFactory.GetPeopleActions(idPeople, isActor, date, pageSizeLocal, page);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetPeopleActions:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение площадок
        /// </summary>
        /// <param name="AreaName">название или часть названия</param>
        /// <param name="idMetro">Ближайшее метро</param>
        /// <param name="PageSize">Кол-во элементов на странице</param>
        /// <param name="Page">Номер страницы</param>
        /// <returns></returns>
        [WebMethod]
        public string GetAreas(string AreaName, long idMetro,int PageSize, int Page)
        {
            try
            {
                int page = Page;
                int pageSize = PageSize;
              
                if (page == 0)
                    page = 1;

                if (pageSize == 0)
                    pageSize = 1;

                string action = DataRequestFactory.GetAreas(AreaName, idMetro, pageSize, page);

                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetAreas:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение списка жанров
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetAllGenre()
        {
            try
            {
                string genre = DataRequestFactory.GetAllGenre();
                return genre;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetAllGenre:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение списка станций метро
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetAllMetro()
        {
            try
            {
                string metro = DataRequestFactory.GetAllMetro();
                return metro;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetAllMetro:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

        /// <summary>
        /// Получение мероприятий
        /// </summary>
        /// <param name="ActionName">название или часть названия</param>
        /// <param name="idArea">Площадка</param>
        /// <param name="PageSize">Кол-во элементов на странице</param>
        /// <param name="Page">Номер страницы</param>
        /// <returns></returns>
        [WebMethod]
        public string  GetTours(string ActionName,long idArea,  int PageSize, int Page)
        {
            try
            {
                int page = Page;
                int pageSize = PageSize;

                if (page == 0)
                    page = 1;

                if (pageSize == 0)
                    pageSize = 1;

                string action = DataRequestFactory.GetTours(ActionName, idArea, pageSize, page);
                return action;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetTours:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

         [WebMethod]
        public string GetTheatricalsAreas()
        {
            try
            {
                string areas = DataRequestFactory.GetTheatricalsAreas();
                return areas;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetTheatricalsAreas:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }

         [WebMethod]
         public string GetTourAreas()
         {
             try
             {
                 string areas = DataRequestFactory.GetTourAreas();
                 return areas;
             }
             catch (Exception ex)
             {
                 _logger.ErrorException("WebService.GetTourAreas:Ошибка!", ex);
                 _logger.Error(ex.Message);
                 _logger.Error(ex.StackTrace);
             }
             return "Error";
         }

        /// <summary>
        /// Получение человека для мероприятия 
        /// </summary>
        /// <param name="id">Идентификатор человека</param>
        /// <param name="isActor">Признак, является ли человек актером</param>
        /// <returns></returns>
        [WebMethod]
        public string GetPeople(long id, bool isActor)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.Error("WebService.GetPeople:Идентификатор мероприятия должен быть больше нуля!");
                    return "Error";
                }
                string people = DataRequestFactory.GetPeople(id, isActor);
                return people;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("WebService.GetPeople:Ошибка!", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return "Error";
        }
    }
}

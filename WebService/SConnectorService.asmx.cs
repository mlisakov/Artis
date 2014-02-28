#define DEBUG

using System;
using System.Web.Services;
using Artis.Data;
using NLog;

namespace SofitConnectorService.Service
{
    [WebService(Namespace = "http://obsspb.ru/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class SConnectorService : WebService
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

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
            }
            return "Error";
        }

        /// <summary>
        /// Получение информации по мероприятию
        /// </summary>
        /// <param name="Id">Идентификатор мероприятия</param>
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
             }
             return "Error";
         }

    }
}

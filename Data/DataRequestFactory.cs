using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NLog;
using Remotion.Linq.Collections;

namespace Artis.Data
{
    /// <summary>
    /// Класс для запроса данных с БД
    /// </summary>
    public static class DataRequestFactory
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static List<long> TheatricalsAreaTypes = new List<long>() {1,7};
        private static List<long> TourAreaTypes = new List<long>() {4};

        /// <summary>
        /// Получение мероприятий по дате проведения
        /// </summary>
        /// <param name="startDate">Начальная дата для фильра</param>
        /// <param name="finishDate">Конечная дата для фильтра</param>
        /// <returns></returns>
        public static string GetAction(DateTime startDate,DateTime finishDate)
        {
            using (ISession session = Domain.Session)
            {
                List<ShortAction> middleActions =
                    session.Query<ActionDate>()
                        .Where(i => i.Date >= startDate && i.Date <= finishDate)
                        .Select(i => new ShortAction(i.Action,i.Area, i.Date, i.Time, i.PriceRange)).ToList();

                return new JavaScriptSerializer().Serialize(middleActions);

            }
        }

        /// <summary>
        /// Получение мероприятий по дате проведения
        /// </summary> 
        /// <param name="actionName">Наименовение мероприятия</param>
        /// <param name="area">Площадка, на которой проводится мероприятие</param>
        /// <param name="startDate">Начальная дата для фильра</param>
        /// <param name="finishDate">Конечная дата для фильтра</param>
      
        /// <returns></returns>
        public static async Task<IList<ActionDate>> GetActions(string actionName,Area area,DateTime? startDate, DateTime? finishDate)
        {
            using (ISession session = Domain.Session)
            {
                //IQueryable<ActionDate> act =
                //    session.Query<ActionDate>()
                //        .Where(i => i.Date >= startDate && i.Date <= finishDate);
                //return act.ToList();
                ICriteria criteria = session.CreateCriteria<ActionDate>("actionDate");
                if (!string.IsNullOrEmpty(actionName))
                {
                    criteria.CreateAlias("actionDate.Action", "Action");
                    criteria.Add(Restrictions.Like("Action.Name", "%" + actionName + "%").IgnoreCase());
                }

                if (area != null)
                {
                    criteria.CreateAlias("actionDate.Area", "Area");
                    criteria.Add(Restrictions.Eq("Area.Name", area.Name).IgnoreCase());
                }

                if (startDate != null && finishDate!=null)
                {
                    criteria.Add(Restrictions.Where<ActionDate>(i => i.Date >= startDate.Value.Date && i.Date <= finishDate.Value.Date));
                }

                return  criteria.List<ActionDate>();

            }
        }


        public static string GetTopAction(DateTime startDate, DateTime finishDate,int count)
        {
            using (ISession session = Domain.Session)
            {
                //IList<Action> act = session.QueryOver<Action>()
                //    .Where(
                //        i =>
                //            i.DateStart >= startDate && i.DateStart <= finishDate)
                //    .WhereStringIsNotNullOrEmpty(i => i.Name)
                //    .WhereStringIsNotNullOrEmpty(i => i.Description)
                //    .Take(count)
                //    .List<Action>();
                //IList<ShortAction> shortAction = act.Select(i => new ShortAction(i)).ToList();

                List<ShortAction> shortActions =
                   session.Query<ActionDate>()
                       .Where(i => i.Date >= startDate && i.Date <= finishDate).Take(count)
                       .Select(i => new ShortAction(i.Action, i.Area, i.Date, i.Time, i.PriceRange)).ToList();
                return new JavaScriptSerializer().Serialize(shortActions);

            }
        }
        /// <summary>
        /// Получение информации о мероприятии
        /// </summary>
        /// <param name="ID">Идентификатор мероприятия</param>
        /// <returns>Десериализованная в json информация о мероприятии</returns>
        public static string GetAction(long ID)
        {
            using (ISession session = Domain.Session)
            {
                Action act = session.Query<Action>().First(i => i.ID == ID);
                MiddleAction middleAction = new MiddleAction(act);
                return new JavaScriptSerializer().Serialize(middleAction);
            }
            return string.Empty;
        }

        /// <summary>
        /// Получение изображений для мероприятия
        /// </summary>
        /// <param name="ID">Идентификатор мероприятия</param>
        /// <returns>Десериализованная в json информация о мероприятии</returns>
        public static string GetActionImages(long ID)
        {
            using (ISession session = Domain.Session)
            {
                Action act = session.Query<Action>().First(i => i.ID == ID);
                return new JavaScriptSerializer().Serialize(act.Data);
            }
        }

        /// <summary>
        /// Получение информации о площадке
        /// </summary>
        /// <param name="ID">Идентификатор площадки</param>
        /// <returns>Десериализованная в json информация о площадке</returns>
        public static string GetArea(long ID)
        {
            using (ISession session = Domain.Session)
            {
                Area area = session.Query<Area>().First(i => i.ID == ID);
                MiddleArea middleArea = new MiddleArea(area);
                return new JavaScriptSerializer().Serialize(middleArea);
            }
        }

        /// <summary>
        /// Получениеизображения для площадке
        /// </summary>
        /// <param name="ID">Идентификатор площадки</param>
        /// <returns>Десериализованная в json информация о площадке</returns>
        public static string GetAreaImages(long ID)
        {
            using (ISession session = Domain.Session)
            {
                Area area = session.Query<Area>().First(i => i.ID == ID);
                return new JavaScriptSerializer().Serialize(area.Data);
            }
        }

        /// <summary>
        /// Получение списка площадок
        /// </summary>
        /// <returns></returns>
        public static string GetAllArea()
        {
            using (ISession session = Domain.Session)
            {
                IEnumerable<ShortArea> areas = session.Query<Area>().Select(i => new ShortArea(i));
                return new JavaScriptSerializer().Serialize(areas);
            }
        }

        public static string GetActions(string ActionName, long idGenre, long idArea, DateTime dateStart, DateTime dateFinish, int PageSize, int Page)
        {
            using (ISession session = Domain.Session)
            {

                ICriteria criteria = session.CreateCriteria<ActionDate>("actionDate");
                ICriteria customCriteria = null;
                if (!string.IsNullOrEmpty(ActionName))
                    customCriteria =
                        criteria.CreateCriteria("actionDate.Action", JoinType.LeftOuterJoin)
                            .Add(Restrictions.Like("Name", "%" + ActionName + "%").IgnoreCase());

                if (idGenre != 0)
                {
                    if (customCriteria != null)
                        customCriteria.Add(Restrictions.Where<Action>(i => i.Genre.ID == idGenre));
                    else
                        criteria.CreateCriteria("actionDate.Action", JoinType.LeftOuterJoin)
                            .Add(Restrictions.Where<Action>(i => i.Genre.ID == idGenre));
                }

                if (idArea != 0)
                    criteria.Add(Restrictions.Where<ActionDate>(i => i.Area.ID == idArea));

                if (dateStart != DateTime.MinValue && dateFinish != DateTime.MinValue)
                    criteria.Add(Restrictions.Where<ActionDate>(i => i.Date >= dateStart && i.Date <= dateFinish));

                IList<ActionDate> allActions = criteria.List<ActionDate>();
                List<ShortAction> act = allActions.Skip((Page - 1) * PageSize).Take(PageSize).Select(i => new ShortAction(i.Action,i.Area,i.Date,i.Time,i.PriceRange)).ToList();

                int count = allActions.Count() / PageSize;
                if ((allActions.Count() % PageSize) != 0)
                    count++;

                KeyValuePair<long, IEnumerable<ShortAction>> itemsKVP = new KeyValuePair<long, IEnumerable<ShortAction>>(count, act);
                return new JavaScriptSerializer().Serialize(itemsKVP);
            }
        }

        public static string GetTours(string ActionName,long idArea, int PageSize, int Page)
        {
            using (ISession session = Domain.Session)
            {

                ICriteria criteria = session.CreateCriteria<ActionDate>("actionDate");

                if (idArea != 0)
                    criteria.Add(Restrictions.Where<ActionDate>(i => i.Area.ID == idArea));

                ICriteria customCriteria = null;
                if (!string.IsNullOrEmpty(ActionName))
                {
                    customCriteria =
                        criteria.CreateCriteria("actionDate.Action", "Action", JoinType.LeftOuterJoin)
                            .Add(Restrictions.Like("Name", "%" + ActionName + "%").IgnoreCase());
                }

                if (customCriteria != null)
                {
                    customCriteria.CreateCriteria("Action.Genre", JoinType.LeftOuterJoin)
                        .Add(Restrictions.Like("Name", "%" + "Экскурсия" + "%").IgnoreCase());
                }
                else
                {
                    criteria.CreateAlias("actionDate.Action", "Action");
                    criteria.CreateCriteria("Action.Genre", JoinType.LeftOuterJoin)
                        .Add(Restrictions.Like("Name", "%" + "Экскурсия" + "%").IgnoreCase());
                }

                IList<ActionDate> allActions = criteria.List<ActionDate>();
                List<ShortAction> act =
                    allActions.Skip((Page - 1)*PageSize)
                        .Take(PageSize)
                        .Select(i => new ShortAction(i.Action, i.Area, i.Date, i.Time, i.PriceRange))
                        .ToList();
                int count = allActions.Count() / PageSize;
                if ((allActions.Count() % PageSize) != 0)
                    count++;
                KeyValuePair<long, IEnumerable<ShortAction>> itemsKVP = new KeyValuePair<long, IEnumerable<ShortAction>>(count, act);
                return new JavaScriptSerializer().Serialize(itemsKVP);

            }
        }

        /// <summary>
        /// Поиск площадок
        /// </summary>
        /// <param name="AreaName">Название площадки</param>
        /// <param name="idMetro">Метро</param>
        /// <param name="PageSize">Кол-во элементов на странице</param>
        /// <param name="Page">Номер страницы</param>
        /// <returns></returns>
        public static string GetAreas(string AreaName, long idMetro,int PageSize, int Page)
        {
            using (ISession session = Domain.Session)
            {

                ICriteria criteria = session.CreateCriteria(typeof(Area));

                if (!string.IsNullOrEmpty(AreaName))
                    criteria.Add(Restrictions.Like("Name", "%" + AreaName + "%").IgnoreCase());
                if (idMetro != 0)
                    criteria.Add(Restrictions.Where<Area>(i => i.Metro.ID == idMetro));

                IList<Area> allAreas = criteria.List<Area>();
                List<ShortArea> area = allAreas.Skip((Page - 1) * PageSize).Take(PageSize).Select(i => new ShortArea(i)).ToList();
                int count = allAreas.Count() / PageSize;
                if ((allAreas.Count() % PageSize) != 0)
                    count++;
                KeyValuePair<long, IEnumerable<ShortArea>> itemsKVP = new KeyValuePair<long, IEnumerable<ShortArea>>(count, area);
                return new JavaScriptSerializer().Serialize(itemsKVP);

            }
        }

        /// <summary>
        /// Получение всех площадок
        /// </summary>
        /// <returns>Список площадок, отсортированный по имени</returns>
        public static async Task<IList<Area>> GetAreas(string areaName = "")
        {
            using (ISession session = Domain.Session)
            {
                if (string.IsNullOrEmpty(areaName))
                    return session.Query<Area>().Select(i => i).OrderBy(i => i.Name).ToList();

                ICriteria criteria = session.CreateCriteria(typeof (Area));
                criteria.Add(Restrictions.Like("Name", "%" + areaName + "%").IgnoreCase());
                return criteria.List<Area>().OrderBy(i => i.Name).ToList();
            }

        }

        /// <summary>
        /// Получение списка жанров
        /// </summary>
        /// <returns></returns>
        public static string GetAllGenre()
        {
            using (ISession session = Domain.Session)
            {
                IEnumerable<Genre> genres = session.Query<Genre>();
                return new JavaScriptSerializer().Serialize(genres);
            }
        }

        /// <summary>
        /// Получение списка жанров
        /// </summary>
        /// <returns>Список всех жанров</returns>
        public static async Task<List<Genre>> GetGenres()
        {
            using (ISession session = Domain.Session)
            {
                IEnumerable<Genre> genres = session.Query<Genre>();
                return genres.ToList();
            }
        }

        /// <summary>
        /// Получение списка станций метро
        /// </summary>
        /// <returns></returns>
        public static string GetAllMetro()
        {
            using (ISession session = Domain.Session)
            {
                IEnumerable<Metro> metro = session.Query<Metro>();
                return new JavaScriptSerializer().Serialize(metro);
            }
        }

        public static string GetTheatricalsAreas()
        {
            using (ISession session = Domain.Session)
            {
                //IList<Area> area = session.QueryOver<Area>()
                //        .Where(i => i.AreaType.ID.IsIn(TheatricalsAreaTypes)).List<Area>();
                //List<ShortArea> shortArea = area.Select(i => new ShortArea(i)).ToList();
                ICriteria criteria = session.CreateCriteria<Area>("area");
                criteria.CreateCriteria("area.AreaType", JoinType.LeftOuterJoin)
                          .Add(Restrictions.In("ID", TheatricalsAreaTypes));

                IList<Area> areas = criteria.List<Area>();
                List<ShortArea> shortArea = areas.Select(i => new ShortArea(i)).ToList();

                return new JavaScriptSerializer().Serialize(shortArea);
            }
        }

        public static string GetTourAreas()
        {
            using (ISession session = Domain.Session)
            {
                ICriteria criteria = session.CreateCriteria<ActionDate>("actionDate");

                criteria.CreateCriteria("actionDate.Action", "Action", JoinType.LeftOuterJoin)
                            .Add(Restrictions.Like("Name", "%Экскурсия%").IgnoreCase());

                List<ShortArea> shortArea =
                    criteria.List<ActionDate>().Select(j => j.Area).ToList().Distinct(new AreaComparer()).Select(i=>new ShortArea(i)).ToList();

                return new JavaScriptSerializer().Serialize(shortArea);
            }
        }

        public static async Task<bool> Save(Area area, List<Data> addedImages, List<long> deletedImages)
        {
            try
            {
                if (deletedImages != null)
                    foreach (long idImage in deletedImages)
                    {
                        Data item = area.Data.First(i => i.ID == idImage);
                        area.Data.Remove(item);
                    }

                if (addedImages != null)
                    foreach (Data data in addedImages)
                    {
                        DataRepository _dataRepository = new DataRepository();
                        _dataRepository.Add(data);
                        area.Data.Add(data);
                    }

                Save(area);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка записи изображения ", ex);
                return false;
            }
            return true;
        }

        public static bool Save<T>(T item)
        {
            IRepository<T> rep = new BaseRepository<T>();
            try
            {
                rep.Update(item);
                return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("DataRequestFactory: Не удалось обновить сущность ",ex);
                return false;
            }

        }
    }
}
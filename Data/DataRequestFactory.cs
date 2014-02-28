﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;

namespace Artis.Data
{
    /// <summary>
    /// Класс для запроса данных с БД
    /// </summary>
    public static class DataRequestFactory
    {

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
                IList<Area> area = session.QueryOver<Area>()
                        .Where(i => i.AreaType.ID.IsIn(TheatricalsAreaTypes)).List<Area>();
                List<ShortArea> shortArea = area.Select(i => new ShortArea(i)).ToList();
                return new JavaScriptSerializer().Serialize(shortArea);
            }
        }

        public static string GetTourAreas()
        {
            using (ISession session = Domain.Session)
            {
                IList<Area> area =
                    session.QueryOver<Action>()
                        .Where(Restrictions.Like("Name", "%Экскурсия%").IgnoreCase())
                        .Select(j => j.Area).List<Area>().Distinct(new AreaComparer()).ToList();

                //IList<Area> area = session.QueryOver<Area>()
                //        .Where(i => i.AreaType.ID.IsIn(TourAreaTypes)).List<Area>();

                List<ShortArea> shortArea = area.Select(i => new ShortArea(i)).ToList();
                return new JavaScriptSerializer().Serialize(shortArea);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NLog;

namespace Artis.Data
{
    /// <summary>
    /// Класс для запроса данных с БД
    /// </summary>
    public static partial class DataRequestFactory
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
                List<SmallAction> middleActions =
                    session.Query<ActionDate>()
                        .Where(i => i.Date >= startDate && i.Date <= finishDate)
                        .Select(i => new SmallAction(i)).ToList();

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
        public static async Task<ObservableCollection<ActionDate>> GetActions(string actionName,string areaName,DateTime? startDate, DateTime? finishDate)
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

                if (!string.IsNullOrEmpty(areaName))
                {
                    criteria.CreateAlias("actionDate.Area", "Area");
                    criteria.Add(Restrictions.Eq("Area.Name", areaName).IgnoreCase());
                }

                if (startDate != null && finishDate!=null)
                {
                    criteria.Add(Restrictions.Where<ActionDate>(i => i.Date >= startDate.Value.Date && i.Date <= finishDate.Value.Date));
                }

                return  new ObservableCollection<ActionDate>(criteria.List<ActionDate>());

            }
        }


        public static string GetTop(DateTime startDate, DateTime finishDate,int count,string GUITabName)
        {
            using (ISession session = Domain.Session)
            {
                GuiSection guiSection = session.Query<GuiSection>().First(i => i.Name.Equals(GUITabName));

                ICriteria criteria = session.CreateCriteria<ActionDate>("actionDate");
                criteria.Add(Restrictions.Eq("actionDate.Date", startDate));
                criteria.CreateAlias("actionDate.Action","Action");
                criteria.CreateAlias("Action.Genre", "Genre");
                criteria.Add(Restrictions.In("Genre.ID", guiSection.Genre.Select(i => i.ID).ToList()));
                var shortActions =
                    criteria.List<ActionDate>()
                        .OrderByDescending(i => i.Action.Rating)
                        .Take(count)
                        .Select(i => new ShortAction(i));
                   //session.Query<ActionDate>()
                   //     .Where(i => i.Date == startDate).OrderByDescending(i=>i.Action.Rating).Take(count)
                   //     .Select(i => new ShortAction(i));
                JavaScriptSerializer js = new JavaScriptSerializer {MaxJsonLength = Int32.MaxValue};
                return js.Serialize(shortActions);
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
                ActionDate act = session.Query<ActionDate>().First(i => i.ID == ID);
                MiddleAction middleAction = new MiddleAction(act);
                return new JavaScriptSerializer().Serialize(middleAction);
            }
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
                ActionDate act = session.Query<ActionDate>().First(i => i.ID == ID);
                return new JavaScriptSerializer().Serialize(act.Action.Data);
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
                List<SmallAction> act = allActions.Skip((Page - 1) * PageSize).Take(PageSize).Select(i => new SmallAction(i)).ToList();

                int count = allActions.Count() / PageSize;
                if ((allActions.Count() % PageSize) != 0)
                    count++;

                KeyValuePair<long, IEnumerable<SmallAction>> itemsKVP = new KeyValuePair<long, IEnumerable<SmallAction>>(count, act);

                return new JavaScriptSerializer().Serialize(itemsKVP);
            }
        }

        public static string GetActions(long idActionDate,DateTime dateStart, int PageSize, int Page)
        {
            using (ISession session = Domain.Session)
            {
                ActionDate actionDate = session.Query<ActionDate>().First(i => i.ID == idActionDate);
                long actionID = actionDate.Action.ID;
                IList<ActionDate> actions =
                    session.Query<ActionDate>()
                        .Where(i => i.Action.ID == actionID && i.Date >= dateStart)
                        .Select(j => j)
                        .ToList();
                List<SmallAction> act = actions.Skip((Page - 1) * PageSize).Take(PageSize).Select(i => new SmallAction(i)).ToList();

                int count = act.Count() / PageSize;
                if ((act.Count() % PageSize) != 0)
                    count++;

                KeyValuePair<long, IEnumerable<SmallAction>> itemsKVP = new KeyValuePair<long, IEnumerable<SmallAction>>(count, act);

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
                List<SmallAction> act =
                    allActions.Skip((Page - 1)*PageSize)
                        .Take(PageSize)
                        .Select(i => new SmallAction(i))
                        .ToList();
                int count = allActions.Count() / PageSize;
                if ((allActions.Count() % PageSize) != 0)
                    count++;
                KeyValuePair<long, IEnumerable<SmallAction>> itemsKVP = new KeyValuePair<long, IEnumerable<SmallAction>>(count, act);
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
        public static async Task<ObservableCollection<Area>> GetAreas(string areaName = "")
        {
            using (ISession session = await Domain.GetSession())
            {
                if (string.IsNullOrEmpty(areaName))
                    return new ObservableCollection<Area>(session.Query<Area>().Select(i => i).OrderBy(i => i.Name));

                ICriteria criteria = session.CreateCriteria(typeof (Area));
                criteria.Add(Restrictions.Like("Name", "%" + areaName + "%").IgnoreCase());
                return new ObservableCollection<Area>(criteria.List<Area>().OrderBy(i => i.Name));
            }
        }

        public static async Task<Area> GetOriginalArea(long ID)
        {
            using (ISession session = Domain.Session)
            {
                Area area = session.Query<Area>().First(i => i.ID == ID);
                return area;
            }
        }

        public static async Task<ObservableCollection<GuiSection>> GetGuiSections()
        {
            using (ISession session = await Domain.GetSession())
            {
                return new ObservableCollection<GuiSection>(session.Query<GuiSection>());
            }
        }

        /// <summary>
        /// Получение списка жанров для категории
        /// </summary>
        /// <param name="sectionGuid">Идентификатор категории</param>
        /// <returns>Список жанров для указанной категории</returns>
        public static async Task<ObservableCollection<Genre>> GetGuiSectionGenres(long sectionGuid)
        {
            using (ISession session = await Domain.GetSession())
            {
                return new ObservableCollection<Genre>(session.Query<GuiSection>().First(i => i.ID == sectionGuid).Genre);
            }
        }

        public static async Task<ObservableCollection<Genre>> GetGuiSectionRestGenres(long sectionGuid)
        {
            using (ISession session = await Domain.GetSession())
            {
                ObservableCollection<Genre> guiSectionGenres=new ObservableCollection<Genre>(session.Query<GuiSection>().First(i => i.ID == sectionGuid).Genre);
                ICriteria criteria = session.CreateCriteria<Genre>("genre").Add(Restrictions.Not(Restrictions.In("ID", guiSectionGenres.Select(i=>i.ID).ToList())));
                return new ObservableCollection<Genre>(criteria.List<Genre>());
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
        public static async Task<ObservableCollection<Genre>> GetGenres(string genreName="")
        {
            using (ISession session = await Domain.GetSession())
            {
                if (string.IsNullOrEmpty(genreName))
                    return new ObservableCollection<Genre>(session.Query<Genre>());

                ICriteria criteria = session.CreateCriteria(typeof (Genre));
                Genre f;
                criteria.Add(Restrictions.Like("Name", "%" + genreName + "%").IgnoreCase());
                return new ObservableCollection<Genre>(criteria.List<Genre>().OrderBy(i => i.Name));
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

        public static async Task<List<Data>> GetImages(string source, long id)
        {
            switch (source)
            {
                case "Area":
                    using (ISession session = Domain.Session)
                    {
                        Area area = session.Query<Area>().First(i => i.ID == id);
                        return area.Data.ToList();
                    }
                    break;
                case "Action":
                    using (ISession session = Domain.Session)
                    {
                        Action action = session.Query<Action>().First(i => i.ID == id);
                        return action.Data.ToList();
                    }
                    break;
                case "Actor":
                    using (ISession session = Domain.Session)
                    {
                        Actor actor = session.Query<Actor>().First(i => i.ID == id);
                        return actor.Data.ToList();
                    }
                    break;
                case "Producer":
                    using (ISession session = Domain.Session)
                    {
                        Producer producer = session.Query<Producer>().First(i => i.ID == id);
                        return producer.Data.ToList();
                    }
                    break;
            }
            return new List<Data>();
        }

        public static async Task<List<Data>> GetSmallImages(long id)
        {
            using (ISession session = Domain.Session)
            {
                Action action = session.Query<Action>().First(i => i.ID == id);
                return action.DataSmall.ToList();
            }
        }

        public static async Task<List<Actor>> GetActorsForAction(long idAction)
        {
            using (ISession session = Domain.Session)
            {
                Action action = session.Query<Action>().First(i => i.ID == idAction);
                return action.Actor.ToList();
            }
        }

        public static async Task<List<Producer>> GetProducersForAction(long idAction)
        {
            using (ISession session = Domain.Session)
            {
                Action action = session.Query<Action>().First(i => i.ID == idAction);
                return action.Producer.ToList();
            }
        }

        public static async Task<ObservableCollection<Producer>> GetProducers(string FIO)
        {
            using (ISession session = await Domain.GetSession())
            {
                if (string.IsNullOrEmpty(FIO))
                    return new ObservableCollection<Producer>(session.Query<Producer>().Select(i => i).OrderBy(i => i.FIO));

                ICriteria criteria = session.CreateCriteria(typeof(Producer));
                criteria.Add(Restrictions.Like("FIO", "%" + FIO + "%").IgnoreCase());
                return new ObservableCollection<Producer>(criteria.List<Producer>().OrderBy(i => i.FIO));
            }
        }

        public static async Task<ObservableCollection<Actor>> GetActors(string FIO)
        {
            using (ISession session = await Domain.GetSession())
            {
                if (string.IsNullOrEmpty(FIO))
                    return new ObservableCollection<Actor>(session.Query<Actor>().Select(i => i).OrderBy(i => i.FIO));

                ICriteria criteria = session.CreateCriteria(typeof(Actor));
                criteria.Add(Restrictions.Like("FIO", "%" + FIO + "%").IgnoreCase());
                return new ObservableCollection<Actor>(criteria.List<Actor>().OrderBy(i => i.FIO));
            }
        }
    }
}
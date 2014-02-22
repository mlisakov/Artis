using System;

namespace Artis.Data
{
    public class ShortAction
    {
        private long _id;
        private string _name;
        private DateTime _dateStart;
        private string _time;
        private string _genreName;
        private string _priceRange;

        private ShortArea _area;

        public long ID
        {
            get { return _id; }
            protected set { _id = value; }
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Дата проведения мероприятия
        /// </summary>
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; }
        }

        /// <summary>
        /// Время проведения мероприятия
        /// </summary>
        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>
        /// Ценовой диапазон
        /// </summary>
        public virtual string PriceRange
        {
            get { return _priceRange; }
            set { _priceRange = value; }
        }

        /// <summary>
        /// Наименование жанра
        /// </summary>
        public string GenreName
        {
            get { return _genreName; }
            set { _genreName = value; }
        }

        /// <summary>
        /// Наименование площадки
        /// </summary>
        public ShortArea Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public ShortAction(Action action)
        {
            Name = action.Name;
            DateStart = action.DateStart;
            Time = action.Time;
            ID = action.ID;
            GenreName = action.Genre.Name;
            PriceRange = action.PriceRange;
            Area = new ShortArea(action.Area);
        }
    }
}

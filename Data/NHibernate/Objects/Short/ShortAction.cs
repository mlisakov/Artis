using System;
using System.Collections.Generic;

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

        private ICollection<ShortArea> _area;

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
        public ICollection<ShortArea> Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public ShortAction(Action action,DateTime date,string time,string priceRange)
        {
            Name = action.Name;
            DateStart = date;
            Time = time;
            ID = action.ID;
            GenreName = action.Genre.Name;
            PriceRange = priceRange;

            foreach (Area area in action.Area)
                Area.Add(new ShortArea(area));
        }
    }
}

using System;

namespace Artis.Data
{
    public class SmallAction
    {
        private long _id;
        private string _name;
        private DateTime _dateStart;
        private string _time;
        private string _genreName;
        private string _priceRange;
        private string _areaAddress;
        private string _areaMetro;

        private string _area;


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
        public string Area
        {
            get { return _area; }
            set { _area = value; }
        }

        /// <summary>
        /// Адрес площадки
        /// </summary>
        public string AreaAddress
        {
            get { return _areaAddress; }
            set { _areaAddress = value; }
        }

        /// <summary>
        /// Ближайшее к площадке метро площадки
        /// </summary>
        public string AreaMetro
        {
            get { return _areaMetro; }
            set { _areaMetro = value; }
        }

        public SmallAction(ActionDate actionDate)
        {
            Name = actionDate.Action.Name;
            DateStart = actionDate.Date;
            Time = actionDate.Time;
            ID = actionDate.ID;
            if (actionDate.Action.Genre != null)
                GenreName = actionDate.Action.Genre.Name;
            PriceRange = actionDate.PriceRange;
            Area = actionDate.Area.Name;
            AreaAddress = actionDate.Area.Addres;
            AreaMetro = actionDate.Area.Metro.Name;
        }
    }
}

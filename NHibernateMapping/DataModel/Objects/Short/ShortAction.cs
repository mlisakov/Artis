using System;
using System.Linq;

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
        private string _smallImage;

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
        /// Маленькое изображение для мероприятия
        /// </summary>
        public string SmallImage
        {
            get { return _smallImage; }
            set { _smallImage = value; }
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

        public ShortAction(ActionDate actionDate)
        {
            Name = actionDate.Action.Name;
            if (actionDate.Action.DataSmall != null && actionDate.Action.DataSmall.Any())
                SmallImage = actionDate.Action.DataSmall.First().Base64StringData;
            DateStart = actionDate.Date;
            Time = actionDate.Time;
            ID = actionDate.ID;
            if (actionDate.Action.Genre != null)
                GenreName = actionDate.Action.Genre.Name;
            PriceRange = actionDate.PriceRange;
            Area = actionDate.Area.Name;
        }
    }
}

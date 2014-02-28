using System;

namespace Artis.Data
{
    public class ActionDate
    {
        private long _id;
        private DateTime _date;
        private string _time;
        private int _minPrice;
        private int _maxPrice;
        private string _priceRange;

        private Action _action;
        private Area _area;

        /// <summary>
        /// Идентификатор
        /// </summary>
        public virtual long ID
        {
            get { return _id; }
            protected set { _id = value; }
        }
        /// <summary>
        /// Дата проведения мероприятия
        /// </summary>
        public virtual DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        /// <summary>
        /// Время проведения мероприятия
        /// </summary>
        public virtual string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>
        /// Минимальная цена билета
        /// </summary>
        public virtual int MinPrice
        {
            get { return _minPrice; }
            set { _minPrice = value; }
        }

        /// <summary>
        /// Максимальная цена билета
        /// </summary>
        public virtual int MaxPrice
        {
            get { return _maxPrice; }
            set { _maxPrice = value; }
        }

        /// <summary>
        /// Диапазон цен на билета
        /// </summary>
        public virtual string PriceRange
        {
            get { return _priceRange; }
            set { _priceRange = value; }
        }

        /// <summary>
        /// Мероприятие
        /// </summary>
        public virtual Action Action
        {
            get { return _action; }
            set { _action = value; }
        }

        /// <summary>
        /// Площадка
        /// </summary>
        public virtual Area Area
        {
            get { return _area; }
            set { _area = value; }
        }
    }
}

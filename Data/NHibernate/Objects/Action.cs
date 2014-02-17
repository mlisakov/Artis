using System;
using System.Collections.Generic;

namespace Artis.Data
{
    /// <summary>
    /// Мероприятие
    /// </summary>
    public class Action
    {
        private long _id;
        private string _name;
        private DateTime _dateStart;
        private string _time;
        private string _description;
        private string _priceRange;

        private Area _area;
        private Genre _genre;
        private State _state;

        private ICollection<Data> _data;
        private ICollection<Actor> _actors;
        private ICollection<Producer> _producers;


        public virtual long ID
        {
            get { return _id; }
            protected set { _id = value; }
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Дата проведения мероприятия
        /// </summary>
        public virtual DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; }
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
        /// Описание мероприятия
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Ценовой диапазон билетов на мероприятие
        /// </summary>
        public virtual string PriceRange
        {
            get { return _priceRange; }
            set { _priceRange = value; }
        }

        /// <summary>
        /// Площадка
        /// </summary>
        public virtual Area Area
        {
            get { return _area; }
            set { _area = value; }
        }

        /// <summary>
        /// Жанр
        /// </summary>
        public virtual Genre Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

        /// <summary>
        /// Состояние
        /// </summary>
        public virtual State State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Список изображений мероприятия
        /// </summary>
        public virtual ICollection<Data> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Список актеров мероприятия
        /// </summary>
        public virtual ICollection<Actor> Actor
        {
            get { return _actors; }
            set { _actors = value; }
        }

        /// <summary>
        /// Список актеров мероприятия
        /// </summary>
        public virtual ICollection<Producer> Producer
        {
            get { return _producers; }
            set { _producers = value; }
        }
    }
}
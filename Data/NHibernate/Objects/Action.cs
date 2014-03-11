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
        private string _description;
        private int _rating;

        private Genre _genre;
        private State _state;

        private ICollection<Area> _area;
        private ICollection<ActionDate> _date;
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
        /// Наименование
        /// </summary>
        public virtual int Rating
        {
            get { return _rating; }
            set { _rating = value; }
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
        /// Площадки
        /// </summary>
        public virtual ICollection<Area> Area
        {
            get { return _area; }
            set { _area = value; }
        }

        /// <summary>
        /// Список актеров мероприятия
        /// </summary>
        public virtual ICollection<ActionDate> ActionDate
        {
            get { return _date; }
            set { _date = value; }
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
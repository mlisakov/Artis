﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Artis.Data
{
    /// <summary>
    /// Мероприятие
    /// </summary>
    public class Action
    {
        private long _id;
        private string _name;
        private string _englishName;
        private string _description;
        private string _englishDescription;
        private int _rating;
        private string _duration;
        private bool _isVerticalSmallImage;

        private Genre _genre;
        private State _state;

        private ICollection<Area> _area;
        private ICollection<ActionDate> _date;
        private ICollection<Data> _data;
        private ICollection<Data> _dataSmall;
        private ICollection<Actor> _actors;
        private ICollection<Producer> _producers;


        public virtual long ID
        {
            get { return _id; }
            set { _id = value; }
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
        /// Наименование на инглише
        /// </summary>
        public virtual string EnglishName
        {
            get { return _englishName; }
            set { _englishName = value; }
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
        /// Продолжительность
        /// </summary>
        public virtual string Duration
        {
            get { return _duration; }
            set { _duration = value; }
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
        /// Описание на инглише
        /// </summary>
        public virtual string EnglishDescription
        {
            get { return _englishDescription; }
            set { _englishDescription = value; }
        }

        /// <summary>
        /// Тип маленького изображения
        /// </summary>
        public virtual bool IsVerticalSmallImage
        {
            get { return _isVerticalSmallImage; }
            set { _isVerticalSmallImage = value; }
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
        [XmlIgnoreAttribute]
        public virtual ICollection<Area> Area
        {
            get { return _area; }
            set { _area = value; }
        }

        /// <summary>
        /// Список актеров мероприятия
        /// </summary>
        [XmlIgnoreAttribute]
        public virtual ICollection<ActionDate> ActionDate
        {
            get { return _date; }
            set { _date = value; }
        }
        /// <summary>
        /// Список изображений мероприятия
        /// </summary>
        [XmlIgnoreAttribute]
        public virtual ICollection<Data> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Список урезанных изображений мероприятия
        /// </summary>
        [XmlIgnoreAttribute]
        public virtual ICollection<Data> DataSmall
        {
            get { return _dataSmall; }
            set { _dataSmall = value; }
        }

        /// <summary>
        /// Список актеров мероприятия
        /// </summary>
        [XmlIgnoreAttribute]
        public virtual ICollection<Actor> Actor
        {
            get { return _actors; }
            set { _actors = value; }
        }

        /// <summary>
        /// Список актеров мероприятия
        /// </summary>
        [XmlIgnoreAttribute]
        public virtual ICollection<Producer> Producer
        {
            get { return _producers; }
            set { _producers = value; }
        }
    }
}
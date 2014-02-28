using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Artis.Data
{
    public class MiddleAction
    {
        private long _id;
        private string _name;
        private string _description;
        private string _genreName;
        
        private ICollection<Actor> _actors;
        private ICollection<Producer> _producers;

        /// <summary>
        /// Идентификатор
        /// </summary>
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
        /// Наименование жанра
        /// </summary>
        public string GenreName
        {
            get { return _genreName; }
            set { _genreName = value; }
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

        public MiddleAction(Action action)
        {
            Name = action.Name;
            ID = action.ID;
            if (action.Genre != null)
                GenreName = action.Genre.Name;
            Description = action.Description;
            Actor = new Collection<Actor>(action.Actor.ToList());
            Producer = new Collection<Producer>(action.Producer.ToList());
        }

    }
}

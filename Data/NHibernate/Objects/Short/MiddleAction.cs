using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Artis.Data
{
    public class MiddleAction:ShortAction
    {
        private string _description;
        
        private ICollection<Actor> _actors;
        private ICollection<Producer> _producers;


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

        public MiddleAction(Action action, DateTime date, string time, string priceRange)
            : base(action, date, time, priceRange)
        {
            Description = action.Description;

            Actor = new Collection<Actor>(action.Actor.ToList());
            Producer = new Collection<Producer>(action.Producer.ToList());
        }
    }
}

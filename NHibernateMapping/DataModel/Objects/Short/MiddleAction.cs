using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Artis.Data
{
    public class MiddleAction:ShortAction
    {
        private string _description;
        private string _areaAddress;
        private long _idArea;
        
        private ICollection<Actor> _actors;
        private ICollection<Producer> _producers;

        /// <summary>
        /// Адрес площадки
        /// </summary>
        public string AreaAddress
        {
            get { return _areaAddress; }
            set { _areaAddress = value; }
        }

        /// <summary>
        /// Идентификатор площадки
        /// </summary>
        public long IDArea
        {
            get { return _idArea; }
            set { _idArea = value; }
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

        public MiddleAction(ActionDate actionDate):base(actionDate)
        {
            AreaAddress = actionDate.Area.Addres;
            IDArea = actionDate.Area.ID;

            Description = actionDate.Action.Description;
            Actor = new Collection<Actor>(actionDate.Action.Actor.ToList());
            Producer = new Collection<Producer>(actionDate.Action.Producer.ToList());
        }

    }
}

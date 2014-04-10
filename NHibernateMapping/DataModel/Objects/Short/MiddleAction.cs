using System.Collections.Generic;
using System.Linq;

namespace Artis.Data
{
    public class MiddleAction:SmallAction
    {
        private string _description;
        private string _areaAddress;
        private long _idArea;
        
        private List<People> _actors;
        private List<People> _producers;

        ///// <summary>
        ///// Адрес площадки
        ///// </summary>
        //public string AreaAddress
        //{
        //    get { return _areaAddress; }
        //    set { _areaAddress = value; }
        //}

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
        public virtual List<People> Actor
        {
            get { return _actors; }
            set { _actors = value; }
        }

        /// <summary>
        /// Список актеров мероприятия
        /// </summary>
        public virtual List<People> Producer
        {
            get { return _producers; }
            set { _producers = value; }
        }

        public MiddleAction(ActionDate actionDate):base(actionDate)
        {
            AreaAddress = actionDate.Area.Addres;
            IDArea = actionDate.Area.ID;

            Description = actionDate.Action.Description;
            Actor =actionDate.Action.Actor.Select(i=>new People(i)).ToList();
            Producer = actionDate.Action.Producer.Select(i => new People(i)).ToList();
        }

    }
}

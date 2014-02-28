namespace Artis.Data
{
    public class ShortArea
    {
        private long _id;
        private string _name;
        private string _address;
        private string _areaType;

        /// <summary>
        /// Идентификатор
        /// </summary>
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
        /// Адрес
        /// </summary>
        public virtual string Addres
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// Тип площадки
        /// </summary>
        public virtual string AreaType
        {
            get { return _areaType; }
            set { _areaType = value; }
        }

        public ShortArea(Area area)
        {
            ID = area.ID;
            Name = area.Name;
            Addres = area.Addres;
            if (area.AreaType != null)
                AreaType = area.AreaType.Name;
        }
    }
}

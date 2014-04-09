using System.Collections.Generic;

namespace Artis.Data
{
    /// <summary>
    /// Площадка
    /// </summary>
    public class Area
    {
        private long _id;
        private string _name;
        private string _englishName;
        private int _areaIndex;
        private string _address;
        private string _englishAddress;
        private string _description;
        private string _englishDescription;
        private string _schema;
        
        private Metro _metro;
        private AreaType _areaType;

        private ICollection<Data> _data;

        /// <summary>
        /// Идентификатор
        /// </summary>
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
        /// Индекс
        /// </summary>
        public virtual int AreaIndex
        {
            get { return _areaIndex; }
            set { _areaIndex = value; }
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
        /// Адрес на инглише
        /// </summary>
        public virtual string EnglishAddress
        {
            get { return _englishAddress; }
            set { _englishAddress = value; }
        }

        /// <summary>
        /// Индекс
        /// </summary>
        public virtual Metro Metro
        {
            get { return _metro; }
            set { _metro = value; }
        }

        /// <summary>
        /// Описание
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
        /// Тип площадки
        /// </summary>
        public virtual AreaType AreaType
        {
            get { return _areaType; }
            set { _areaType = value; }
        }

        /// <summary>
        /// Схема проезда
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public virtual string SchemaImage
        {
            get { return _schema; }
            set { _schema = value; }
        }

        /// <summary>
        /// Список изображений мероприятия
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public virtual ICollection<Data> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        protected bool Equals(Area other)
        {
            return _id == other._id && string.Equals(_name, other._name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_id.GetHashCode() * 397) ^ (_name != null ? _name.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Area) obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
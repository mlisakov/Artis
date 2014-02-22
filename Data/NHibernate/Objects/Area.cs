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
        private int _areaIndex;
        private string _address;
        private string _description;
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
        /// Индекс
        /// </summary>
        public virtual int AreaIndex
        {
            get { return _areaIndex; }
            set { _areaIndex = value; }
        }

        /// <summary>
        /// Индекс
        /// </summary>
        public virtual string Addres
        {
            get { return _address; }
            set { _address = value; }
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
        public virtual string SchemaImage
        {
            get { return _schema; }
            set { _schema = value; }
        }

        /// <summary>
        /// Список изображений мероприятия
        /// </summary>
        public virtual ICollection<Data> Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
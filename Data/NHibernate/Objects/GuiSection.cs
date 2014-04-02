using System.Collections.Generic;
using System.Xml.Serialization;

namespace Artis.Data
{
    public class GuiSection
    {
        private long _id;
        private string _name;

        private ICollection<Genre> _genre;

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
        /// Связанные с секцией жанры
        /// </summary>
        [XmlIgnoreAttribute]
        public virtual ICollection<Genre> Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }
    }
}

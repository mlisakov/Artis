using System.Collections.Generic;

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
        /// Связанные с секцией жанры
        /// </summary>
        public virtual ICollection<Genre> Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }
    }
}

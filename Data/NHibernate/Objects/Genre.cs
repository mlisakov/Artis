namespace Artis.Data
{
    public class Genre
    {
        protected bool Equals(Genre other)
        {
            return _id == other._id && string.Equals(_name, other._name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_id.GetHashCode()*397) ^ (_name != null ? _name.GetHashCode() : 0);
            }
        }

        private long _id;
        private string _name;

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


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Genre) obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
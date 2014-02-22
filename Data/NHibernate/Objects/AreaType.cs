namespace Artis.Data
{
    public class AreaType
    {
        private long _id;
        private string _name;

        /// <summary>
        /// Идентификатор
        /// </summary>
        public virtual long ID
        {
            get { return _id; }
            protected set
            {
                _id = value;
            }
        }
        /// <summary>
        /// Наименование
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            protected set
            {
                _name = value;
            }
        }
    }
}

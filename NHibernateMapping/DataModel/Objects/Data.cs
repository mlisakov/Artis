namespace Artis.Data
{
    public class Data
    {
        private long _id;
        private string _data;

        public virtual long ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Данные(картинка в виде BaseString64)
        /// </summary>
        public virtual string Base64StringData
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
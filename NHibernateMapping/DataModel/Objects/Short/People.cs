namespace Artis.Data
{
    public class People:IPeople
    {
        private long _id;
        private string _FIO;
        private string _englishFIO;
        /// <summary>
        /// Идентификатор
        /// </summary>
        public virtual long ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// ФИО
        /// </summary>
        public virtual string FIO
        {
            get { return _FIO; }
            set { _FIO = value; }
        }

        /// <summary>
        /// ФИО на инглише
        /// </summary>
        public virtual string EnglishFIO
        {
            get { return _englishFIO; }
            set { _englishFIO = value; }
        }

        public People(IPeople people)
        {
            ID = people.ID;
            FIO = people.FIO;
            EnglishFIO = people.EnglishFIO;
        }
    }
}

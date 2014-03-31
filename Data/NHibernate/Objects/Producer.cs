using System;

namespace Artis.Data
{
    public class Producer
    {
        private long _id;
        private string _FIO;
        private string _englishFIO;
        private string _description;
        private string _englishDescription;

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
    }
}
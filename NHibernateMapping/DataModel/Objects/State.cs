﻿namespace Artis.Data
{
    public class State
    {
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
    }
}
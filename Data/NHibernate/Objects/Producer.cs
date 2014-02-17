﻿using System;

namespace Artis.Data
{
    public class Producer
    {
        private long _id;
        private string _FIO;
        private string _description;

        /// <summary>
        /// Идентификатор
        /// </summary>
        public virtual long ID
        {
            get { return _id; }
            protected set { _id = value; }
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
        /// Описание
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
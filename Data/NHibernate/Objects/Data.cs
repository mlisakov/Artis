using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artis.Data
{
    public class Data
    {
        private long _id;
        private string _data;

        public virtual long ID
        {
            get { return _id; }
            protected set { _id = value; }
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
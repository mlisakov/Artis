using System;

namespace Artis.Data
{
    /// <summary>
    /// Ошибка доступа к базе данных
    /// </summary>
    public class DBAccessFailedException:Exception
    {
        public DBAccessFailedException(string message):base(message)
        {}
    }
}

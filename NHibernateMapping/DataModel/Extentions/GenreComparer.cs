using System.Collections.Generic;

namespace Artis.Data
{
    public class GenreComparer : IEqualityComparer<Genre>
    {
        public bool Equals(Genre x, Genre y)
        {
            if (x.ID==y.ID)
                return true;
            return false;
        }

        public int GetHashCode(Genre obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}

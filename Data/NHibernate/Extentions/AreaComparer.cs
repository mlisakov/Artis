using System.Collections.Generic;

namespace Artis.Data
{
    public class AreaComparer : IEqualityComparer<Area>
    {
        public bool Equals(Area x, Area y)
        {
            if (x.Name.Equals(y.Name) && x.Addres.Equals(y.Addres))
                return true;
            return false;
        }

        public int GetHashCode(Area obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}

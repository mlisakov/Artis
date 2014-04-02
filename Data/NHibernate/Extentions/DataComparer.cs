using System.Collections.Generic;

namespace Artis.Data
{
    public class DataComparer : IEqualityComparer<Data>
    {
        public bool Equals(Data x, Data y)
        {
            if (x.Base64StringData.Equals(y.Base64StringData))
                return true;
            return false;
        }

        public int GetHashCode(Data obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}

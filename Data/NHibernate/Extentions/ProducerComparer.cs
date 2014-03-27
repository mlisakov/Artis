using System.Collections.Generic;

namespace Artis.Data
{
    public class ProducerComparer:IEqualityComparer<Producer>
    {
        public bool Equals(Producer x, Producer y)
        {
            if (x.ID == y.ID)
                return true;
            return false;
        }

        public int GetHashCode(Producer obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}

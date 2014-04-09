using System.Collections.Generic;

namespace Artis.Data
{
    public class ActorComparer:IEqualityComparer<Actor>
    {
        public bool Equals(Actor x, Actor y)
        {
            if (x.ID == y.ID)
                return true;
            return false;
        }

        public int GetHashCode(Actor obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}

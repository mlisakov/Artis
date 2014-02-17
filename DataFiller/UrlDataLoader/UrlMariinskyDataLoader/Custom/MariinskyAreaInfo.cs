using System.Collections.Generic;

namespace Artis.DataLoader
{
    public class MariinskyAreaInfo
    {
        public string Description { get; set; }
        public List<string> Images { get; set; }

        public MariinskyAreaInfo(string description, List<string> images)
        {
            Description = description;
            Images = images;
        }
    }
}

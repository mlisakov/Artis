using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Artis.Data
{
    public sealed class XmlActor:Actor
    {
        private List<Data> _images;

        [XmlArray("Images"), XmlArrayItem(typeof(Data), ElementName = "Image")]
        public List<Data> Images
        {
            get { return _images; }
            set { _images = value; }
        }

        public XmlActor()
        {
        }

        public XmlActor(Actor actor)
        {
            ID = actor.ID;
            FIO = actor.FIO;
            EnglishFIO = actor.EnglishFIO;
            Description = actor.Description;
            EnglishDescription = actor.EnglishDescription;
            Images = actor.Data.ToList();
        }

        public Actor ToActor()
        {
            return new Actor()
            {
                ID = ID,
                FIO = FIO,
                EnglishFIO = EnglishFIO,
                Description = Description,
                EnglishDescription = EnglishDescription,
                Data = Images
            };
        }
    }
}

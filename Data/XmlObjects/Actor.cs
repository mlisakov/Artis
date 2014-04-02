using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Artis.Data.XmlObjects
{
    public sealed class XmlActor:Actor
    {
        private List<Data> _images;
        //private List<long> _deletedImages;


        [XmlArray("Images"), XmlArrayItem(typeof(Data), ElementName = "Image")]
        public List<Data> Images
        {
            get { return _images; }
            set { _images = value; }
        }

        //[XmlArray("DeletedImages"), XmlArrayItem(typeof(long), ElementName = "DeletedImage")]
        //public List<long> DeletedImages
        //{
        //    get { return _deletedImages; }
        //    set { _deletedImages = value; }
        //}

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

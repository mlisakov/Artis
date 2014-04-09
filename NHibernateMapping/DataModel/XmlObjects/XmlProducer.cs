using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Artis.Data
{
    public sealed class XmlProducer:Producer
    {
         private List<Data> _images;

        [XmlArray("{ProducerImages"), XmlArrayItem(typeof(Data), ElementName = "ProducerImage")]
        public List<Data> Images
        {
            get { return _images; }
            set { _images = value; }
        }

        public XmlProducer()
        {
        }

        public XmlProducer(Producer producer)
        {
            ID = producer.ID;
            FIO = producer.FIO;
            EnglishFIO = producer.EnglishFIO;
            Description = producer.Description;
            EnglishDescription = producer.EnglishDescription;
            Images = producer.Data.ToList();
        }

        public Producer ToProducer()
        {
            return new Producer()
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

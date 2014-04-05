using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class ProducersXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("Producers"), XmlArrayItem(typeof(XmlProducer), ElementName = "Producer")]
        public List<XmlProducer> Producers { get; set; }

        public ProducersXmlProvider()
        {
            Producers = new List<XmlProducer>();
        }


        public ProducersXmlProvider(IEnumerable<Producer> producers):this()
        {
            foreach (Producer producer in producers)
                Producers.Add(new XmlProducer(producer));
           // Producers = new List<XmlProducer>(producers);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(ProducersXmlProvider));
                StringWriter stringWriter = new StringWriter();
                XmlWriter writer = XmlWriter.Create(stringWriter);

                xmlserializer.Serialize(writer, this);

                XmlDocument serializeXml = new XmlDocument();
                serializeXml.LoadXml(stringWriter.ToString());

                writer.Close();
                return serializeXml;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Не удалось сериализовать мероприятия",ex);
                throw new Exception("Не удалось сериализовать мероприятия");
            }
        }

        public List<Producer> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(ProducersXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            ProducersXmlProvider result = (ProducersXmlProvider)xmlserializer.Deserialize(reader);
            reader.Close();
            return result.Producers.Select(xmlActor => xmlActor.ToProducer()).ToList();
        }
    }
}

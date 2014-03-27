using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class ProducersXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("Producers"), XmlArrayItem(typeof(Producer), ElementName = "Producer")]
        public List<Producer> Producers { get; set; }

        public ProducersXmlProvider()
        {
            Producers = new List<Producer>();
        }


        public ProducersXmlProvider(IEnumerable<Producer> producers)
        {
            Producers = new List<Producer>(producers);
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
            return result.Producers;
        }
    }
}

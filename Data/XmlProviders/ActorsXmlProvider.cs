using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class ActorsXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("Actors"), XmlArrayItem(typeof(Actor), ElementName = "Actor")]
        public List<Actor> Actors { get; set; }

        public ActorsXmlProvider()
        {
            Actors = new List<Actor>();
        }


        public ActorsXmlProvider(IEnumerable<Actor> actors)
        {
            Actors = new List<Actor>(actors);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(ActorsXmlProvider));
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

        public List<Actor> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(ActorsXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            ActorsXmlProvider result = (ActorsXmlProvider)xmlserializer.Deserialize(reader);
            reader.Close();
            return result.Actors;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Artis.Data.XmlObjects;
using NLog;

namespace Artis.Data
{
    public class ActorsXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("Actors"), XmlArrayItem(typeof(XmlActor), ElementName = "Actor")]
        public List<XmlActor> Actors { get; set; }

        public ActorsXmlProvider()
        {
            Actors = new List<XmlActor>();
        }


        public ActorsXmlProvider(IEnumerable<Actor> actors):this()
        {
            foreach (Actor actor in actors)
                Actors.Add(new XmlActor(actor));
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
            return result.Actors.Select(xmlActor => xmlActor.ToActor()).ToList();
        }
    }
}

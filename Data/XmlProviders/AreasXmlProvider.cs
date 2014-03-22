using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Artis.Data;
using NLog;

namespace Artis.Consts
{
    public class AreasXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("Areas"), XmlArrayItem(typeof(Area), ElementName = "Area")]
        public List<Area> Areas { get; set; }

        public AreasXmlProvider()
        {
            Areas=new List<Area>();
        }


        public AreasXmlProvider(IEnumerable<Area> areas)
        {
            Areas=new List<Area>(areas);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(AreasXmlProvider));
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
                _logger.ErrorException("Не удалось сериализовать площадки",ex);
                throw new Exception("Не удалось сериализовать площадки");
            }
        }

        public List<Area> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(AreasXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            AreasXmlProvider result = (AreasXmlProvider) xmlserializer.Deserialize(reader);
            reader.Close();
            return result.Areas;
        }
    }
}
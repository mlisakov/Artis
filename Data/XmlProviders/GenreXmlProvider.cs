using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class GenreXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("Genres"), XmlArrayItem(typeof(Genre), ElementName = "Genre")]
        public List<Genre> Genres { get; set; }

        public GenreXmlProvider()
        {
            Genres=new List<Genre>();
        }


        public GenreXmlProvider(IEnumerable<Genre> areas)
        {
            Genres=new List<Genre>(areas);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(GenreXmlProvider));
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
                _logger.ErrorException("Не удалось сериализовать жанры",ex);
                throw new Exception("Не удалось сериализовать жанры");
            }
        }

        public List<Genre> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(GenreXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            GenreXmlProvider result = (GenreXmlProvider)xmlserializer.Deserialize(reader);
            reader.Close();
            return result.Genres;
        }
    }
}

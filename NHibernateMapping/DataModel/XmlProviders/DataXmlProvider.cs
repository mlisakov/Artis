using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class DataXmlProvider
    {
         private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("ListData"), XmlArrayItem(typeof(Data), ElementName = "Data")]
        public List<Data> DataList { get; set; }

        public DataXmlProvider()
        {
            DataList = new List<Data>();
        }


        public DataXmlProvider(IEnumerable<Data> data)
        {
            DataList = new List<Data>(data);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(DataXmlProvider));
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
                _logger.ErrorException("Не удалось сериализовать изображения",ex);
                throw new Exception("Не удалось сериализовать изображения");
            }
        }

        public List<Data> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(DataXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            DataXmlProvider result = (DataXmlProvider) xmlserializer.Deserialize(reader);
            reader.Close();
            return result.DataList;
        }
    }
}

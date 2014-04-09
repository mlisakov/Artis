using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class ActionWebXmlProvider
    {
         private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("WebActions"), XmlArrayItem(typeof(ActionWeb), ElementName = "ActionWeb")]
        public List<ActionWeb> WebActions { get; set; }

        public ActionWebXmlProvider()
        {
            WebActions = new List<ActionWeb>();
        }


        public ActionWebXmlProvider(IEnumerable<ActionWeb> webAction)
        {
            WebActions = new List<ActionWeb>(webAction);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(ActionWebXmlProvider));
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
                _logger.ErrorException("Не удалось сериализовать загруженное мероприятие",ex);
                throw new Exception("Не удалось сериализовать загруженное мероприятие");
            }
        }

        public List<ActionWeb> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(ActionWebXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            ActionWebXmlProvider result = (ActionWebXmlProvider)xmlserializer.Deserialize(reader);
            reader.Close();
            return result.WebActions;
        }
    }
}

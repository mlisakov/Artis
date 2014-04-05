using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class ActionDateXmlProvider
    {
         private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("ActionDates"), XmlArrayItem(typeof(ActionDate), ElementName = "ActionDate")]
        public List<ActionDate> ActionDates { get; set; }

        public ActionDateXmlProvider()
        {
            ActionDates = new List<ActionDate>();
        }


        public ActionDateXmlProvider(IEnumerable<ActionDate> actions)
        {
            ActionDates = new List<ActionDate>(actions);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(ActionDateXmlProvider));
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

        public List<ActionDate> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(ActionDateXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            ActionDateXmlProvider result = (ActionDateXmlProvider)xmlserializer.Deserialize(reader);
            reader.Close();
            return result.ActionDates;
        }
    }
}

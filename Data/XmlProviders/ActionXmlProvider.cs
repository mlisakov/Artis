using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class ActionXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("Actions"), XmlArrayItem(typeof(Action), ElementName = "Action")]
        public List<Action> Actions { get; set; }

        public ActionXmlProvider()
        {
            Actions=new List<Action>();
        }


        public ActionXmlProvider(IEnumerable<Action> actions)
        {
            Actions = new List<Action>(actions);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(ActionXmlProvider));
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

        public List<Action> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(ActionXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            ActionXmlProvider result = (ActionXmlProvider)xmlserializer.Deserialize(reader);
            reader.Close();
            return result.Actions;
        }

    }
}

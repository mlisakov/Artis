using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace Artis.Data
{
    public class GuiSectionXmlProvider
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        [XmlArray("GuiSections"), XmlArrayItem(typeof(GuiSection), ElementName = "GuiSection")]
        public List<GuiSection> GuiSections { get; set; }

        public GuiSectionXmlProvider()
        {
            GuiSections = new List<GuiSection>();
        }


        public GuiSectionXmlProvider(IEnumerable<GuiSection> actions)
        {
            GuiSections = new List<GuiSection>(actions);
        }

        public XmlDocument ToXml()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(GuiSectionXmlProvider));
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
                _logger.ErrorException("Не удалось сериализовать вкладки GUI",ex);
                throw new Exception("Не удалось сериализовать вкладки GUI");
            }
        }

        public List<GuiSection> FromXml(string doc)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof(GuiSectionXmlProvider));
            StringReader stringReader = new StringReader(doc);
            XmlReader reader = XmlReader.Create(stringReader);

            GuiSectionXmlProvider result = (GuiSectionXmlProvider)xmlserializer.Deserialize(reader);
            reader.Close();
            return result.GuiSections;
        }
    }
}

namespace Minesweeper.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Xml;
    using Minesweeper.Core.Xml;
    using System.Xml.Linq;


    public class XmlHelper
    {
        public static String FormatIndentXML(String xml)
        {
            string formattedXml = XElement.Parse(xml).ToString();
            return formattedXml;

/*
            Console.WriteLine(formattedXml);

            String result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                result = FormattedXML;
            }
            catch (XmlException)
            {
            }

            mStream.Close();
            writer.Close();

            return result;
 */
        }

        public static string GetElementName(string xml)
        {
            string output = "";
            using (var reader = new XmlTextReader(new StringReader(xml)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        output = reader.Name;
                        break;
                    }
                }
            }

            return output;
        }

        public static string GetElementNameFromFile(string file)
        {
            string output = "";
            using (var reader = new XmlTextReader(file))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        output = reader.Name;
                        break;
                    }
                }
            }

            return output;
        }


        public static string Serialize(object entity)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return serializer.Serialize(entity);
        }

        public static string Serialize(object entity, string rootElementName)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return serializer.Serialize(entity, rootElementName);
        }

        public static string Serialize(object entity, string rootElementName, XmlSerializerSettings settings)
        {
            XmlDataSerializer serializer = new XmlDataSerializer(settings);
            if (string.IsNullOrEmpty(rootElementName))
            return serializer.Serialize(entity);
            else
            return serializer.Serialize(entity, rootElementName);
        }



        public static object Deserialize(string xmlObject)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return serializer.Deserialize(xmlObject);
        }

        public static object Deserialize(Type configType, string xmlObject)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return serializer.Deserialize(configType, xmlObject);
        }

        public static object Deserialize(Type rootType, string rootElementName, string xmlObject)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return serializer.Deserialize(rootType, rootElementName, xmlObject);
        }

        public static object Deserialize(string xmlObject, XmlSerializerSettings settings)
        {
            XmlDataSerializer serializer = new XmlDataSerializer(settings);
            return serializer.Deserialize(xmlObject);
        }

        public static object Deserialize(Type rootType, string rootElementName, string xmlObject, XmlSerializerSettings settings)
        {
            XmlDataSerializer serializer = new XmlDataSerializer(settings);
            return serializer.Deserialize(rootType, rootElementName, xmlObject);
        }


        public static TObj Deserialize<TObj>(string xml)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return (TObj)serializer.Deserialize(typeof(TObj), xml);
        }

        public static TObj Deserialize<TObj>(string rootElementName, string xmlObject)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return (TObj)serializer.Deserialize(typeof(TObj), rootElementName, xmlObject);
        }

        public static TObj Deserialize<TObj>(string xml, XmlSerializerSettings settings)
        {
            XmlDataSerializer serializer = new XmlDataSerializer(settings);
            return (TObj)serializer.Deserialize(typeof(TObj), xml);
        }

        public static TObj Deserialize<TObj>(string rootElementName, string xmlObject, XmlSerializerSettings settings)
        {
            XmlDataSerializer serializer = new XmlDataSerializer(settings);
            return (TObj)serializer.Deserialize(typeof(TObj), rootElementName, xmlObject);
        }



        public static TObj Deserialize<TObj>(XmlReader reader)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return (TObj)serializer.Deserialize(typeof(TObj), reader);
        }

        public static TObj Deserialize<TObj>(string rootElementName, XmlReader reader)
        {
            XmlDataSerializer serializer = new XmlDataSerializer();
            return (TObj)serializer.Deserialize(typeof(TObj), rootElementName, reader);
        }

        public static TObj Deserialize<TObj>(XmlReader reader, XmlSerializerSettings settings)
        {
            XmlDataSerializer serializer = new XmlDataSerializer(settings);
            return (TObj)serializer.Deserialize(typeof(TObj), reader);
        }

        public static TObj Deserialize<TObj>(string rootElementName, XmlReader reader, XmlSerializerSettings settings)
        {
            XmlDataSerializer serializer = new XmlDataSerializer(settings);
            return (TObj)serializer.Deserialize(typeof(TObj), rootElementName, reader);
        }


        public static XmlReader GetXmlReader(string file)
        {
            return new XmlTextReader(file);
        }

        public static XmlReader GetXmlReaderFromString(string xml)
        {
            return new XmlTextReader(new StringReader(xml));
        }
    }
}

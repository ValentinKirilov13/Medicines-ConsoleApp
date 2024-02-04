using System.Text;
using System.Xml.Serialization;

namespace Medicines.JsonXml
{
    public static class Xml
    {
        public static string SerializeObject<T>(T data, string rootElement) where T : class
        {
            string result = null!;

            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));

            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, data, xmlSerializerNamespaces);

                result = Encoding.UTF8.GetString(ms.ToArray());
            }

            return result;
        }

        public static T DeserializeObject<T>(string xml, string rootElement) where T : class
        {
            T result = null!;

            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                result = (T)serializer.Deserialize(ms);
            }

            return result;
        }
    }
}

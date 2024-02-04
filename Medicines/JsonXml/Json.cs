using Newtonsoft.Json;
using System.Globalization;

namespace Medicines.JsonXml
{
    public static class Json
    {
        public static string SerializeObject<T>(T entity) where T : class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture
            };

            return JsonConvert.SerializeObject(entity, settings);
        }

        public static T? DeserializeObject<T>(string inputJson)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture
            };

            return JsonConvert.DeserializeObject<T>(inputJson, settings);
        }
    }
}

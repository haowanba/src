using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Weixin.Tools
{
    class JsonConverter
    {
        public static T Parse<T>(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string Stringify(object jsonObject)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}

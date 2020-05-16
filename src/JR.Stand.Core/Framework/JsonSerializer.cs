using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JR.Stand.Core.Framework
{
    public static class JsonSerializer
    {
        public static Encoding Encoding = Encoding.UTF8;

        public static string SerializeObject<T>(T t)
        {
            using (var ms = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof (T));
                ser.WriteObject(ms, t);
                string result = Encoding.GetString(ms.ToArray());
                ser = null;
                return result;
            }
        }

        public static T DeserializeObject<T>(string json)
        {
            using (var ms = new MemoryStream(Encoding.GetBytes(json)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof (T));

                T t = (T) ser.ReadObject(ms);
                ser = null;
                return t;
            }
        }
    }
}
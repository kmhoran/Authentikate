using System;
using System.Text;
using Newtonsoft.Json;

namespace Utils.Extentions
{
    public static class Object
    {
        public static byte[] ToBytes<T>(this T obj){
            if(obj == null) return null;
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public static T FromBytes<T>(this byte[] bytes)
        {
            if(bytes == null || bytes.Length == 0) return default(T);
            try
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to convert byte array to {nameof(T)}");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return default(T);
            }

        }
    }
}
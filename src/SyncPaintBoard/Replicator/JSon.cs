using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SyncPaintBoard.Replicator
{
    public class Json
    {
        public class JsonInt32Converter : JsonConverter
        {
            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return (reader.TokenType == JsonToken.Integer)
                    ? Convert.ToInt32(reader.Value)     
                    : serializer.Deserialize(reader);   
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(int) ||
                        objectType == typeof(long) ||
                        objectType == typeof(int) ||
                        objectType == typeof(object)
                    ;
            }
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, GetSerializerSettings());
        }

        public static object Deserialize(string obj)
        {
            return JsonConvert.DeserializeObject(obj, GetSerializerSettings());
        }

        public static object Deserialize(string obj, Type type)
        {
            return JsonConvert.DeserializeObject(obj, type, GetSerializerSettings());
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Converters = new List<JsonConverter> { new JsonInt32Converter() },
            };
        }
    }
}

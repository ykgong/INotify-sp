using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Message.WebApi.Extensions
{
    public static class JsonSerializerExtension
    {
        public static string Serialize<T>(this T t) where T : class
        {
            return t.ToJson();
        }

        public static T Deserialize<T>(this string value) where T : class
        {
            return value.FromJson<T>();
        }

        private static JsonSerializerSettings _jsonSettings;

        private static JsonSerializerSettings GetJsonSettings(JsonSerializerSettings jsonSettings)
        {
            return jsonSettings ?? (jsonSettings = new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateFormatString = "yyyy-MM-ddTHH:mm:sszzz",
                Converters = new JsonConverter[] { new JsGuidConverter() },
                DefaultValueHandling = DefaultValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ConstructorHandling = ConstructorHandling.Default,
            });
        }

        public static void Set(this JsonSerializerSettings jsonSerializer)
        {
            GetJsonSettings(jsonSerializer);
        }

        /// <summary>
        /// 格式化请求参数 添加最外层input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonAddInput<T>(this T obj)
        {
            return new { input = obj }.ToJson();
        }

        public static string ToJson<T>(this T obj)
        {
            return obj != null ? JsonConvert.SerializeObject(obj, GetJsonSettings(_jsonSettings)) : string.Empty;
        }

        public static string ToJson(this object obj, Type type)
        {
            return obj != null ? JsonConvert.SerializeObject(obj, type, GetJsonSettings(_jsonSettings)) : string.Empty;
        }

        public static T FromJson<T>(this string json)
        {
            return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json, GetJsonSettings(_jsonSettings));
        }
        public static T FromJson<T>(this JToken json)
        {
            return json == null ? default(T) : json.ToObject<T>();
        }
        public static T FromJson<T>(this JObject json)
        {
            return json == null ? default(T) : json.ToObject<T>();
        }

        public static object FromJson(this string json, Type type)
        {
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json, type, GetJsonSettings(_jsonSettings));
        }
    }

    public class JsGuidConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            try
            {
                var type = value.GetType();
                if (typeof(Guid) == type)
                {
                    var item = (Guid)value;
                    writer.WriteValue($"{item:N}");
                    writer.Flush();
                }
                else if (typeof(Guid?) == type)
                {
                    var item = (Guid?)value;
                    writer.WriteValue($"{item.Value:N}");
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
            {
                return Guid.Empty;
            }
            if (!(typeof(Guid) == objectType || typeof(Guid?) == objectType))
                return Guid.Empty;
            try
            {
                var boolText = reader.Value.ToString();
                if (string.IsNullOrWhiteSpace(boolText))
                {
                    return Guid.Empty;
                }
                return Guid.TryParse(boolText, out Guid result) ? result : Guid.Empty;
            }
            catch (Exception)
            {
                throw new Exception($"Error converting value {reader.Value} to type '{objectType}'");
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid);
        }
    }

}

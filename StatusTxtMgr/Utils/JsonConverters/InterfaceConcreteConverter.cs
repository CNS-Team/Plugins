using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatusTxtMgr.Utils.Attrs;
using System.Reflection;

namespace StatusTxtMgr.Utils.JsonConverters
{
    // 评价：一坨使
    // 懒得解释了
    public class InterfaceConcreteConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanConvert(Type objectType) => objectType.IsInterface;


        public InterfaceConcreteConverter()
        {

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var jsonObj = JObject.Load(reader);
                object target = null;
                JToken jsonTypeName;
                if (jsonObj.TryGetValue("TypeName", out jsonTypeName) && jsonTypeName is JValue)
                {
                    foreach (Type t in objectType.GetCustomAttribute<ImplementsAttribute>()?.ImplementsTypes)
                    {
                        var propInfo = t.GetProperty("TypeName", BindingFlags.Public | BindingFlags.Static);
                        if (propInfo == null || propInfo.PropertyType != typeof(string))
                            continue;
                        if ((string)propInfo.GetValue(null) == jsonTypeName.Value<string>())
                        {
                            target = Activator.CreateInstance(t);
                            break;
                        }
                    }
                }
                if (target == null)
                    throw new Exception("Could not find a corresponding concrete class");
                serializer.Populate(jsonObj.CreateReader(), target);
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to convert", ex);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

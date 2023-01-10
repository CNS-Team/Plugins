using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StatusTxtMgr.SettingsModel;

namespace StatusTxtMgr
{
    public class STMSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Utils.LogLevel LogLevel = Utils.LogLevel.INFO;
        public List<IStatusTextSetting> StatusTextSettings = new();
    }
}

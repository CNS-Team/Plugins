using Newtonsoft.Json;
using TShockAPI;

namespace StatusTxtMgr.SettingsModel
{
    public class StaticText : IStatusTextSetting, IStatusTextUpdateHandler
    {
        [JsonProperty]
        public static string TypeName => "static_text";
        public string Text { get; set; }

        public void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx) =>
            processedHandlers.Add(this);
            

        public bool Invoke(TSPlayer tsplr, bool forceUpdate = false) => forceUpdate;

        public string GetPlrST(TSPlayer tsplr) => Text;
    }
}

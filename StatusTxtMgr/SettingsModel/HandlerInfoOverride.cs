using Newtonsoft.Json;

namespace StatusTxtMgr.SettingsModel
{
    public class HandlerInfoOverride : IStatusTextSetting
    {
        [JsonProperty]
        public static string TypeName => "handler_info_override";
        public string PluginName { get; set; }
        public bool Enabled { get; set; }
        public ulong UpdateInterval { get; set; }

        public void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx)
        {
            var handlerMatched = handlers.Find(h => h.AssemblyName == PluginName);
            if (handlerMatched == null)
                return;

            handlers.Remove(handlerMatched);
            if (Enabled)
            {
                if (UpdateInterval > 0)
                    handlerMatched.UpdateInterval = UpdateInterval;
                processedHandlers.Add(handlerMatched);
            }
        }
    }
}

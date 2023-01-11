using Newtonsoft.Json;
using StatusTxtMgr.Utils.Attrs;
using StatusTxtMgr.Utils.JsonConverters;

namespace StatusTxtMgr.SettingsModel
{
    [JsonConverter(typeof(InterfaceConcreteConverter))]
    [Implements(typeof(StaticText), typeof(HandlerInfoOverride))]
    public interface IStatusTextSetting
    {
        // 实际上还需要加一个
        // public static string TypeName => "handler_info_override";
        // 供 Converter 使用

        /// <summary>
        /// 令 Handler 处理 Handler List
        /// </summary>
        /// <param name="handlers">由插件而来未经处理的 Handlers</param>
        /// <param name="processedHandlers">最终交由 HandlerList 依序调用的 Handlers</param>
        /// <param name="settingsIdx">实例在 Setting 中的 index</param>
        void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx);
    }
}

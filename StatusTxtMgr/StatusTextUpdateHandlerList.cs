﻿using StatusTxtMgr.Utils;
using System.Text;
using TShockAPI;

namespace StatusTxtMgr;

// 从旧版 Tshock HandlerList 抄的，建议光速重写
public class StatusTextUpdateHandlerList
{
    private List<StatusTextUpdateHandlerItem> Handlers { get; set; } = new();
    private List<IStatusTextUpdateHandler> ProcessedHandlers { get; set; } = new();
    private readonly object HandlersLock = new object();

    public StatusTextUpdateHandlerList()
    {

    }

    public void Register(StatusTextUpdateDelegate handler, ulong updateInterval = 60)
    {
        this.Register(new StatusTextUpdateHandlerItem(handler, updateInterval));
    }

    public void Register(StatusTextUpdateHandlerItem handlerItem)
    {
        lock (this.HandlersLock)
        {
            this.Handlers.Add(handlerItem);
            this.LoadSettings();
        }
    }

    public void Deregister(StatusTextUpdateDelegate handler)
    {
        lock (this.HandlersLock)
        {
            this.Handlers.RemoveAll(hi => hi.UpdateDelegate == handler);
            this.LoadSettings();
        }
    }

    public bool Invoke(TSPlayer tsplr, StringBuilder sb, bool forceUpdate = false)
    {
        try
        {
            List<IStatusTextUpdateHandler> list;
            lock (this.HandlersLock)
            {
                list = new List<IStatusTextUpdateHandler>(this.ProcessedHandlers);
            }
            var isUpdateRequired = list.Aggregate(false, (current, hi) => hi.Invoke(tsplr, forceUpdate) || current);
            // 轮询 Handlers 对应玩家是否需要更新 Status Text
            if (isUpdateRequired)
            {
                // 将更新后的 Status Text 组合起来
                foreach (var hi in list)
                {
                    sb.Append(hi.GetPlrST(tsplr));
                }
            }
            return isUpdateRequired; // 对应玩家是否需要更新 Status Text
        }
        catch (Exception ex)
        {
            Logger.Warn("Exception occur in StatusTextUpdateHandlerList.Invoke, Ex: " + ex);
            return false;
        }
    }

    public void LoadSettings()
    {
        lock (this.HandlersLock)
        {
            // 依次加载所有 Setting Handlers
            var handlers = new List<StatusTextUpdateHandlerItem>(this.Handlers);
            this.ProcessedHandlers.Clear();
            var idx = 0;
            foreach (var sts in StatusTxtMgr.Settings.StatusTextSettings)
            {
                sts.ProcessHandlers(handlers, this.ProcessedHandlers, idx);
                idx++;
            }
            // 将所有未被 Setting Handlers '认领' 的 Plugin Handlers 加入到 Processed Handlers 中
            this.ProcessedHandlers.AddRange(handlers);
        }
    }
}
﻿using StatusTxtMgr.Utils;
using System.Text;
using Terraria;
using TShockAPI;

namespace StatusTxtMgr;

public interface IStatusTextUpdateHandler
{
    bool Invoke(TSPlayer tsplr, bool forceUpdate = false);
    string GetPlrST(TSPlayer tsplr);
}


public delegate void StatusTextUpdateDelegate(StatusTextUpdateEventArgs args);

public class StatusTextUpdateEventArgs
{
    public TSPlayer? tsplayer { get; set; }
    public StringBuilder? statusTextBuilder { get; set; }
}

public class StatusTextUpdateHandlerItem : IStatusTextUpdateHandler
{
    public StatusTextUpdateDelegate UpdateDelegate;
    public ulong UpdateInterval = 60;
    public string AssemblyName;

    private readonly StringBuilder?[] plrSBs = new StringBuilder[Main.maxPlayers];

    public StatusTextUpdateHandlerItem(StatusTextUpdateDelegate updateDelegate, ulong updateInterval = 60)
    {
        this.UpdateDelegate = updateDelegate ?? throw new ArgumentNullException(nameof(updateDelegate));
        this.UpdateInterval = updateInterval > 0 ? updateInterval : throw new ArgumentException("cannot be 0", nameof(updateInterval));
        this.AssemblyName = updateDelegate.Method.DeclaringType?.Assembly.GetName().Name ?? "";
    }

    public bool Invoke(TSPlayer tsplr, bool forceUpdate = false)
    {
        try
        {
            // 检查对应玩家是否需要更新 Status Text
            if (forceUpdate || (Utils.Common.TickCount + (ulong) tsplr.Index) % this.UpdateInterval == 0)
            {
                var updateDelegate = this.UpdateDelegate;
                var args = new StatusTextUpdateEventArgs() { tsplayer = tsplr, statusTextBuilder = this.plrSBs.AcquirePlrSB(tsplr) };
                updateDelegate(args);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"Exception occur while invoking delegate of '{this.AssemblyName}' in StatusTextUpdateHandler.Invoke, Ex: {ex}");
        }
        return false;
    }

    // 获取当前 Handler 对对应玩家的 Status Text
    public string GetPlrST(TSPlayer tsplr)
    {
        return this.plrSBs[tsplr.Index]?.ToString() ?? "";
    }
}
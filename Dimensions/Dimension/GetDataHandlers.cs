using MaxMind;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Net;
using System.Reflection;
using TShockAPI;

namespace Dimension;

public class GetDataHandlers
{
    private static Dictionary<PacketTypes, GetDataHandlerDelegate> _getDataHandlerDelegates;

    private readonly Dimensions Dimensions;

    public GetDataHandlers(Dimensions Dimensions)
    {
        this.Dimensions = Dimensions;
        _getDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate> {
        {
            PacketTypes.Placeholder,
            this.HandleDimensionsMessage
        } };
    }

    public bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
    {
        if (_getDataHandlerDelegates.TryGetValue(type, out var value))
        {
            try
            {
                return value(new GetDataHandlerArgs(player, data));
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.ToString());
            }
        }
        return false;
    }

    private bool HandleDimensionsMessage(GetDataHandlerArgs args)
    {
        if (args.Player == null)
        {
            return false;
        }
        _ = args.Player.Index;
        var num = args.Data.ReadInt16();
        var remoteAddress = args.Data.ReadString();
        var result = false;
        if (num == 0)
        {
            result = this.HandleIpInformation(remoteAddress, args.Player);
        }
        return result;
    }

    private bool HandleIpInformation(string remoteAddress, TSPlayer player)
    {
        typeof(TSPlayer).GetField("CacheIP", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(player, remoteAddress);
        if (this.Dimensions.Geo != null)
        {
            var text = this.Dimensions.Geo.TryGetCountryCode(IPAddress.Parse(remoteAddress));
            player.Country = (text == null) ? "N/A" : GeoIPCountry.GetCountryNameByCode(text);
            if (text == "A1" && TShock.Config.Settings.KickProxyUsers)
            {
                player.Kick("Proxies are not allowed.", force: true, silent: true);
                return false;
            }
        }
        return true;
    }
}
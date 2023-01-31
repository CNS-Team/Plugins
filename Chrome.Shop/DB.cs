using Microsoft.Xna.Framework;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;

namespace QwRPG;

public static class DB
{
    /// <summary>
    /// -货币
    /// </summary>
    public static bool DelCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = `点券` - @0 WHERE `玩家名` = @1", 点券, 玩家名);
        var plr = TSPlayer.FindByNameOrID(玩家名);
        if (是否头顶显示 && plr.Count != 0 && shop.ShopPlugin.Qw配置.是否头顶显示货币变化)
        {
            Utils.SendCombatMsg(string.Format(shop.ShopPlugin.Qw配置.货币减少时头顶显示, 点券), Color.AliceBlue, plr[0].TPlayer.position);
        }
        //QwRPG.Status(plr[0]);
        TShock.Log.Info($"{玩家名} 货币 - {点券}");
        return true;
        /*
        using var 表 = TShock.DB.QueryReader("SELECT * FROM QwRPG WHERE 玩家名=@0", 玩家名);
        if (!表.Read())
        {
            return false;
        }
        else
        {
            TShock.DB.Query("UPDATE QwRPG SET 点券 = 点券-@0 WHERE 玩家名 = @1", 点券, 玩家名);
            return true;
        }
        */
    }
    public static class Utils
    {
        // Token: 0x06000048 RID: 72 RVA: 0x00003D2C File Offset: 0x00001F2C
        public static void SendCombatMsg(string msg, Color color, Vector2 position)
        {
            Terraria.NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(msg), (int) color.PackedValue, position.X, position.Y, 0f, 0, 0, 0);
        }
    }
    /// <summary>
    /// 查货币
    /// </summary>
    public static long QueryCost(string 玩家名)
    {
        long 货币 = 0;
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名))
        {
            if (表.Read())
            {
                货币 = 表.Get<long>("点券");
            }
        }
        return 货币;
    }
}

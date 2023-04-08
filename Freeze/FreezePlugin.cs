using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Freeze;

[ApiVersion(2, 1)]
public class FreezePlugin : TerrariaPlugin
{
    private readonly HashSet<string> hash = new HashSet<string>();

    private bool freezeAll = false;

    public override string Name => "Freeze";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public override string Author => "棱镜";

    public override string Description => "临时冻结玩家";

    public FreezePlugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        //IL_002f: Unknown result type (might be due to invalid IL or missing references)
        //IL_0047: Expected O, but got Unknown
        //IL_0042: Unknown result type (might be due to invalid IL or missing references)
        //IL_0047: Unknown result type (might be due to invalid IL or missing references)
        //IL_0058: Expected O, but got Unknown
        //IL_006a: Unknown result type (might be due to invalid IL or missing references)
        //IL_0082: Expected O, but got Unknown
        //IL_007d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0087: Expected O, but got Unknown
        ServerApi.Hooks.GamePostUpdate.Register(this, this.PostGameUpdate);
        Commands.ChatCommands.Add(new Command("tshock.admin.kick", this.FreezeCmd, new string[1] { "freeze" })
        {
            HelpText = "用法:/freeze <玩家名>"
        });
        Commands.ChatCommands.Add(new Command("tshock.admin.kick", this.FreezeAllCmd, new string[1] { "freezeall" }));
    }

    private void FreezeAllCmd(CommandArgs args)
    {
        this.freezeAll = !this.freezeAll;
        args.Player.SendSuccessMessage("全服冻结已" + (this.freezeAll ? "开启" : "关闭"));
        TSPlayer.All.SendInfoMessage(args.Player.Name + (this.freezeAll ? "开启" : "关闭") + "了全服冻结");
        if (args.Parameters.Count > 0)
        {
            TSPlayer.All.SendInfoMessage(args.Parameters[0]);
        }
    }

    private void PostGameUpdate(EventArgs args)
    {
        //IL_0071: Unknown result type (might be due to invalid IL or missing references)
        var players = TShock.Players;
        var array = players;
        foreach (var val in array)
        {
            if (val != null && (this.hash.Contains(val.Name) || this.freezeAll) && !val.HasPermission("tshock.admin.kick"))
            {
                val.Disable("", 0);
                val.TPlayer.Bottom = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
                val.SendData((PacketTypes) 13, "", val.Index, 0f, 0f, 0f, 0);
            }
        }
    }

    private void FreezeCmd(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage("用法:/freeze <玩家名>");
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError((IEnumerable<object>) list);
            return;
        }
        if (list.Count == 0)
        {
            args.Player.SendErrorMessage("找不到玩家");
            return;
        }
        var val = list[0];
        if (this.hash.Add(val.Name))
        {
            val.Disable("被管理员冻结", 0);
            args.Player.SendSuccessMessage("已冻结" + val.Name);
        }
        else
        {
            args.Player.SendSuccessMessage("已解除冻结" + val.Name);
            this.hash.Remove(val.Name);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
        this.Dispose(disposing);
    }
}
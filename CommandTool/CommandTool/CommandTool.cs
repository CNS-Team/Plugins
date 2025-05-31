using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Xna.Framework;
using PlaceholderAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace CommandTool;

[ApiVersion(2, 1)]
public class CommandTool : TerrariaPlugin
{
    private Command AddCommand;

    private int[] PlayerRectangle = new int[256];

    private RectangleState[] PlayerRectangleState = new RectangleState[256];

    internal static Config MainConfig = Config.GetConfig();

    private static string switchParameters = "/开关 <添加/列表/删除/冷却/权限忽略/取消/重绑/完成>";

    public override string Name => this.GetType().Name;

    public override string Description => "命令工具";

    public override Version Version => this.GetType().Assembly.GetName().Version;

    public CommandTool(Main game)
        : base(game)
    {
        this.AddCommand = new Command("开关", SwitchCmd, "开关", "kg", "switch");
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(this.AddCommand);
        GeneralHooks.ReloadEvent += this.OnReload;
        ServerApi.Hooks.NetGetData.Register(this, this.OnNetGetData);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.Remove(this.AddCommand);
            GeneralHooks.ReloadEvent -= this.OnReload;
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnNetGetData);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
        }
    }

    private void OnReload(ReloadEventArgs e)
    {
        Array.Fill(this.PlayerRectangle, -1);
        Array.Fill(this.PlayerRectangleState, RectangleState.Unset);
        MainConfig = Config.GetConfig();
        var player = e.Player;
        var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
        defaultInterpolatedStringHandler.AppendLiteral("指令执行点加载成功,加载了");
        defaultInterpolatedStringHandler.AppendFormatted(MainConfig.Rectangles.Length);
        defaultInterpolatedStringHandler.AppendLiteral("个指令点");
        player.SendSuccessMessage(defaultInterpolatedStringHandler.ToStringAndClear());
    }

    private void OnServerJoin(JoinEventArgs args)
    {
        this.PlayerRectangle[args.Who] = -1;
        this.PlayerRectangleState[args.Who] = RectangleState.Unset;
    }

    private void OnNetGetData(GetDataEventArgs args)
    {
        if (args.MsgID == PacketTypes.HitSwitch)
        {
            var tSPlayer = TShock.Players[args.Msg.whoAmI];
            var span = args.Msg.readBuffer.AsSpan().Slice(args.Index, args.Length);
            var switchPos = new SwitchPos(BinaryPrimitives.ReadInt16LittleEndian(span), BinaryPrimitives.ReadInt16LittleEndian(span.Slice(2, span.Length - 2)));
            var tile = Main.tile[switchPos.X, switchPos.Y];
            if (tile.type == 132)
            {
                if (tile.frameX % 36 == 0)
                {
                    switchPos.X++;
                }

                if (tile.frameY == 0)
                {
                    switchPos.Y++;
                }
            }

            switch (tSPlayer.GetSiwtchState())
            {
                case SwitchState.SelectingSwitch:
                {
                    tSPlayer.SetSwitchPos(switchPos);
                    var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("成功绑定位于X：");
                    defaultInterpolatedStringHandler.AppendFormatted(switchPos.X);
                    defaultInterpolatedStringHandler.AppendLiteral("、Y：");
                    defaultInterpolatedStringHandler.AppendFormatted(switchPos.Y);
                    defaultInterpolatedStringHandler.AppendLiteral(" 的开关");
                    tSPlayer.SendSuccessMessage(defaultInterpolatedStringHandler.ToStringAndClear());
                    tSPlayer.SendSuccessMessage("输入/开关 ，可查看子命令列表");
                    tSPlayer.SetSwitchState(SwitchState.AddingCommands);
                    if (MainConfig.SwitchCommandList.ContainsKey(switchPos.ToString()))
                    {
                        tSPlayer.SetSwitchCommandInfo(MainConfig.SwitchCommandList[switchPos.ToString()]);
                    }

                    break;
                }
                case SwitchState.None:
                {
                    if (!MainConfig.SwitchCommandList.ContainsKey(switchPos.ToString()))
                    {
                        break;
                    }

                    var num = 999999.0;
                    Dictionary<string, DateTime> switchCooding = tSPlayer.GetSwitchCooding();
                    if (switchCooding != null && switchCooding.ContainsKey(switchPos.ToString()))
                    {
                        num = (DateTime.Now - switchCooding[switchPos.ToString()]).TotalSeconds;
                    }

                    if (num < (double) MainConfig.SwitchCommandList[switchPos.ToString()].cooldown)
                    {
                        break;
                    }

                    var switchCommandInfo = MainConfig.SwitchCommandList[switchPos.ToString()];
                    if (switchCommandInfo.ignorePerms)
                    {
                        var group = tSPlayer.Group;
                        tSPlayer.Group = new SuperAdminGroup();
                        try
                        {
                            foreach (var command in switchCommandInfo.commandList)
                            {
                                try
                                {
                                    Commands.HandleCommand(tSPlayer, global::PlaceholderAPI.PlaceholderAPI.Instance.placeholderManager.GetText(command.ReplaceTags(tSPlayer), tSPlayer));
                                }
                                catch
                                {
                                    Commands.HandleCommand(tSPlayer, command.ReplaceTags(tSPlayer));
                                }
                            }
                        }
                        finally
                        {
                            tSPlayer.Group = group;
                        }
                    }
                    else
                    {
                        foreach (var command2 in switchCommandInfo.commandList)
                        {
                            try
                            {
                                Commands.HandleCommand(tSPlayer, global::PlaceholderAPI.PlaceholderAPI.Instance.placeholderManager.GetText(command2.ReplaceTags(tSPlayer), tSPlayer));
                            }
                            catch
                            {
                                Commands.HandleCommand(tSPlayer, command2.ReplaceTags(tSPlayer));
                            }
                        }
                    }

                    if (switchCooding == null)
                    {
                        switchCooding = new Dictionary<string, DateTime> { { switchPos.ToString(), DateTime.Now } };
                        tSPlayer.SetSwitchCooding(switchCooding);
                    }
                    else
                    {
                        switchCooding[switchPos.ToString()] = DateTime.Now;
                    }

                    break;
                }
            }
        }
        else
        {
            if (args.MsgID != PacketTypes.PlayerUpdate || !MainConfig.Enable)
            {
                return;
            }

            var whoAmI = args.Msg.whoAmI;
            var tSPlayer2 = TShock.Players[whoAmI];
            if (this.PlayerRectangleState[whoAmI] == RectangleState.Completed)
            {
                if (!MainConfig.Rectangles[this.PlayerRectangle[whoAmI]].Contains(tSPlayer2.TileX, tSPlayer2.TileY))
                {
                    this.PlayerRectangle[whoAmI] = -1;
                    this.PlayerRectangleState[whoAmI] = RectangleState.Unset;
                }
            }
            else
            {
                if (this.PlayerRectangleState[whoAmI] != RectangleState.Unset)
                {
                    return;
                }

                var flag = true;
                for (var i = 0; i < MainConfig.Rectangles.Length; i++)
                {
                    var rectangleInfo = MainConfig.Rectangles[i];
                    if (rectangleInfo.Contains(tSPlayer2.TileX, tSPlayer2.TileY))
                    {
                        flag = false;
                        this.PlayerRectangle[whoAmI] = i;
                        this.PlayerRectangleState[whoAmI] = RectangleState.Doing;
                        if (rectangleInfo.Sign.Enable && !string.IsNullOrEmpty(rectangleInfo.Sign.OnEnter))
                        {
                            SendCombatText(tSPlayer2, rectangleInfo.Sign.Color, rectangleInfo.Sign.OnEnter);
                        }

                        ThreadPool.QueueUserWorkItem(this.ExcuteCommand, whoAmI);
                    }
                }

                if (flag)
                {
                    this.PlayerRectangle[whoAmI] = -1;
                }
            }
        }
    }

    private void ExcuteCommand(object? who)
    {
        var tSPlayer = TShock.Players[(int) who];
        var num = this.PlayerRectangle[tSPlayer.Index];
        if (num == -1)
        {
            this.PlayerRectangleState[tSPlayer.Index] = RectangleState.Unset;
            return;
        }

        var rectangleInfo = MainConfig.Rectangles[num];
        for (var i = 0; i < rectangleInfo.Time; i++)
        {
            Thread.Sleep(1000);
            if (tSPlayer.Active && rectangleInfo.Contains(tSPlayer.TileX, tSPlayer.TileY))
            {
                if (rectangleInfo.Sign.Enable && !string.IsNullOrEmpty(rectangleInfo.Sign.Loop))
                {
                    var color = rectangleInfo.Sign.Color;
                    var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("据");
                    defaultInterpolatedStringHandler.AppendFormatted(rectangleInfo.Sign.Loop);
                    defaultInterpolatedStringHandler.AppendLiteral("还有");
                    defaultInterpolatedStringHandler.AppendFormatted(rectangleInfo.Time - i);
                    defaultInterpolatedStringHandler.AppendLiteral("秒");
                    SendCombatText(tSPlayer, color, defaultInterpolatedStringHandler.ToStringAndClear());
                }

                continue;
            }

            this.PlayerRectangle[tSPlayer.Index] = -1;
            this.PlayerRectangleState[tSPlayer.Index] = RectangleState.Unset;
            return;
        }

        StandCommand[] commands = rectangleInfo.Commands;
        foreach (var standCommand in commands)
        {
            var group = tSPlayer.Group;
            var flag = string.IsNullOrEmpty(standCommand.Group);
            if (!flag)
            {
                if (group.Name != standCommand.Group)
                {
                    while (group.Parent != null)
                    {
                        if (group.ParentName == standCommand.Group)
                        {
                            flag = true;
                            break;
                        }

                        group = group.Parent;
                    }
                }
                else
                {
                    flag = true;
                }
            }

            if (!flag)
            {
                continue;
            }

            if (tSPlayer.HasPermission(standCommand.Permission))
            {
                if (!Commands.HandleCommand(tSPlayer, standCommand.Text))
                {
                    tSPlayer.SendErrorMessage("命令 " + standCommand.Text + " 执行失败");
                }

                continue;
            }

            tSPlayer.Group.AddPermission(standCommand.Permission);
            if (!Commands.HandleCommand(tSPlayer, standCommand.Text))
            {
                tSPlayer.SendErrorMessage("命令 " + standCommand.Text + " 执行失败");
            }

            tSPlayer.Group.RemovePermission(standCommand.Permission);
        }

        this.PlayerRectangleState[tSPlayer.Index] = RectangleState.Completed;
    }

    private static void SendCombatText(TSPlayer player, Color color, string text)
    {
        var memoryStream = new MemoryStream();
        var binaryWriter = new BinaryWriter(memoryStream);
        binaryWriter.Write((ushort) 0);
        binaryWriter.Write((byte) 119);
        binaryWriter.Write(player.X);
        binaryWriter.Write(player.Y);
        binaryWriter.Write(color.R);
        binaryWriter.Write(color.G);
        binaryWriter.Write(color.B);
        binaryWriter.Write((byte) 0);
        binaryWriter.Write(text);
        var array = memoryStream.ToArray();
        BinaryPrimitives.WriteUInt16LittleEndian(array, (ushort) array.Length);
        player.SendRawData(array);
    }

    private static void SwitchCmd(CommandArgs args)
    {
        var player = args.Player;
        switch (player.GetSiwtchState())
        {
            case SwitchState.None:
                player.SendSuccessMessage("激活一个开关以将其绑定,之后可输入/开关 ，查看子命令");
                player.SetSwitchState(SwitchState.SelectingSwitch);
                break;
            case SwitchState.AddingCommands:
            {
                if (args.Parameters.Count == 0)
                {
                    player.SendErrorMessage("正确指令：");
                    player.SendErrorMessage(switchParameters);
                    break;
                }

                var switchCommandInfo = player.GetSwitchCommandInfo();
                if (switchCommandInfo == null)
                {
                    switchCommandInfo = new SwitchCommandInfo();
                    player.SetSwitchCommandInfo(switchCommandInfo);
                }

                var text = args.Parameters[0].ToLower();
                var text2 = text;
                if (text2 != null)
                {
                    float result;
                    string text3;
                    bool result2;
                    SwitchPos switchPos;
                    int result3;
                    string value;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
                    switch (text2.Length)
                    {
                        case 3:
                        {
                            var c = text2[0];
                            if (c != 'a')
                            {
                                if (c != 'd' || !(text2 == "del"))
                                {
                                    break;
                                }

                                goto IL_0533;
                            }

                            if (!(text2 == "add"))
                            {
                                break;
                            }

                            goto IL_044f;
                        }
                        case 2:
                            switch (text2[0])
                            {
                                case '添':
                                    break;
                                case 't':
                                    goto IL_0286;
                                case '列':
                                    goto IL_029c;
                                case 'l':
                                    goto IL_02b2;
                                case '删':
                                    goto IL_02d9;
                                case 's':
                                    goto IL_02ef;
                                case '冷':
                                    goto IL_0305;
                                case '取':
                                    goto IL_031b;
                                case 'q':
                                    goto IL_0331;
                                case '重':
                                    goto IL_0347;
                                case 'z':
                                    goto IL_035d;
                                case '完':
                                    goto IL_0373;
                                case 'w':
                                    goto IL_0389;
                                default:
                                    goto end_IL_00b9;
                            }

                            if (!(text2 == "添加"))
                            {
                                break;
                            }

                            goto IL_044f;
                        case 4:
                        {
                            var c = text2[0];
                            if ((uint) c <= 108u)
                            {
                                if (c != 'd')
                                {
                                    if (c != 'l' || !(text2 == "list"))
                                    {
                                        break;
                                    }

                                    goto IL_04a4;
                                }

                                if (!(text2 == "done"))
                                {
                                    break;
                                }

                                goto IL_075c;
                            }

                            if (c != 'q')
                            {
                                if (c != '权' || !(text2 == "权限忽略"))
                                {
                                    break;
                                }
                            }
                            else if (!(text2 == "qxhl"))
                            {
                                break;
                            }

                            goto IL_0689;
                        }
                        case 6:
                        {
                            var c = text2[0];
                            if (c != 'c')
                            {
                                if (c != 'r' || !(text2 == "rebind"))
                                {
                                    break;
                                }

                                goto IL_0743;
                            }

                            if (!(text2 == "cancel"))
                            {
                                break;
                            }

                            goto IL_071e;
                        }
                        case 8:
                            if (!(text2 == "cooldown"))
                            {
                                break;
                            }

                            goto IL_05f4;
                        case 11:
                        {
                            if (!(text2 == "ignoreperms"))
                            {
                                break;
                            }

                            goto IL_0689;
                        }
                            IL_0331:
                            if (!(text2 == "qx"))
                            {
                                break;
                            }

                            goto IL_071e;
                            IL_071e:
                            player.SetSwitchState(SwitchState.None);
                            player.SetSwitchCommandInfo(new SwitchCommandInfo());
                            player.SendSuccessMessage("已取消添加要添加的命令");
                            return;
                            IL_031b:
                            if (!(text2 == "取消"))
                            {
                                break;
                            }

                            goto IL_071e;
                            IL_0305:
                            if (!(text2 == "冷却"))
                            {
                                break;
                            }

                            goto IL_05f4;
                            IL_02d9:
                            if (!(text2 == "删除"))
                            {
                                break;
                            }

                            goto IL_0533;
                            IL_0286:
                            if (!(text2 == "tj"))
                            {
                                break;
                            }

                            goto IL_044f;
                            IL_02b2:
                            if (text2 == "lb")
                            {
                                goto IL_04a4;
                            }

                            if (!(text2 == "lq"))
                            {
                                break;
                            }

                            goto IL_05f4;
                            IL_02ef:
                            if (!(text2 == "sc"))
                            {
                                break;
                            }

                            goto IL_0533;
                            IL_05f4:
                            if (args.Parameters.Count < 2 || !float.TryParse(args.Parameters[1], out result))
                            {
                                player.SendErrorMessage("语法错误：/开关 冷却 <秒>");
                                return;
                            }

                            switchCommandInfo.cooldown = result;
                            defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
                            defaultInterpolatedStringHandler.AppendLiteral("冷却时间已设置为 ");
                            defaultInterpolatedStringHandler.AppendFormatted(result);
                            defaultInterpolatedStringHandler.AppendLiteral(" 秒");
                            player.SendSuccessMessage(defaultInterpolatedStringHandler.ToStringAndClear());
                            MainConfig.Write();
                            goto IL_083a;
                            IL_083a:
                            player.SetSwitchCommandInfo(switchCommandInfo);
                            return;
                            IL_044f:
                            text3 = "/" + string.Join(" ", args.Parameters.Skip(1));
                            switchCommandInfo.commandList.Add(text3);
                            player.SendSuccessMessage("成功添加: " + text3);
                            MainConfig.Write();
                            goto IL_083a;
                            IL_0689:
                            if (args.Parameters.Count < 2 || !bool.TryParse(args.Parameters[1], out result2))
                            {
                                player.SendErrorMessage("语法错误：/开关 权限忽略 <true/false>");
                                return;
                            }

                            switchCommandInfo.ignorePerms = result2;
                            defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
                            defaultInterpolatedStringHandler.AppendLiteral("是否忽略玩家权限设置为: ");
                            defaultInterpolatedStringHandler.AppendFormatted(result2);
                            defaultInterpolatedStringHandler.AppendLiteral(".");
                            player.SendSuccessMessage(defaultInterpolatedStringHandler.ToStringAndClear());
                            MainConfig.Write();
                            goto IL_083a;
                            IL_029c:
                            if (!(text2 == "列表"))
                            {
                                break;
                            }

                            goto IL_04a4;
                            IL_04a4:
                            player.SendMessage("当前开关绑定的指令:", Color.Green);
                            for (var i = 0; i < switchCommandInfo.commandList.Count; i++)
                            {
                                defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
                                defaultInterpolatedStringHandler.AppendLiteral("(");
                                defaultInterpolatedStringHandler.AppendFormatted(i);
                                defaultInterpolatedStringHandler.AppendLiteral(") ");
                                defaultInterpolatedStringHandler.AppendFormatted(switchCommandInfo.commandList[i]);
                                player.SendMessage(defaultInterpolatedStringHandler.ToStringAndClear(), Color.Yellow);
                            }

                            goto IL_083a;
                            IL_0389:
                            if (!(text2 == "wc"))
                            {
                                break;
                            }

                            goto IL_075c;
                            IL_0373:
                            if (!(text2 == "完成"))
                            {
                                break;
                            }

                            goto IL_075c;
                            IL_075c:
                            switchPos = player.GetSwitchPos();
                            player.SendSuccessMessage("设置成功的开关位于 X： {0}， Y： {1}".SFormat(switchPos.X, switchPos.Y));
                            foreach (var command in switchCommandInfo.commandList)
                            {
                                player.SendMessage(command, Color.Yellow);
                            }

                            MainConfig.SwitchCommandList[switchPos.ToString()] = switchCommandInfo;
                            player.SetSwitchState(SwitchState.None);
                            player.SetSwitchPos(new SwitchPos());
                            player.SetSwitchCommandInfo(new SwitchCommandInfo());
                            MainConfig.Write();
                            return;
                            IL_0533:
                            if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out result3))
                            {
                                player.SendErrorMessage("语法错误：/开关 del <指令编号>");
                                return;
                            }

                            value = switchCommandInfo.commandList[result3];
                            switchCommandInfo.commandList.RemoveAt(result3);
                            defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 2);
                            defaultInterpolatedStringHandler.AppendLiteral("成功删除了第");
                            defaultInterpolatedStringHandler.AppendFormatted(result3);
                            defaultInterpolatedStringHandler.AppendLiteral("条指令：");
                            defaultInterpolatedStringHandler.AppendFormatted(value);
                            defaultInterpolatedStringHandler.AppendLiteral("。");
                            player.SendSuccessMessage(defaultInterpolatedStringHandler.ToStringAndClear());
                            MainConfig.Write();
                            goto IL_083a;
                            IL_035d:
                            if (!(text2 == "zb"))
                            {
                                break;
                            }

                            goto IL_0743;
                            IL_0347:
                            if (!(text2 == "重绑"))
                            {
                                break;
                            }

                            goto IL_0743;
                            IL_0743:
                            player.SendSuccessMessage("重新激活开关后可以重新绑定");
                            player.SetSwitchState(SwitchState.SelectingSwitch);
                            return;
                            end_IL_00b9:
                            break;
                    }
                }

                player.SendErrorMessage("语法无效. " + switchParameters);
                break;
            }
        }
    }
}
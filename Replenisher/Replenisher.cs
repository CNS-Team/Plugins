using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace Replenisher;

[ApiVersion(2, 1)]
public class Replenisher : TerrariaPlugin
{
    private static readonly int TIMEOUT = 100000;

    private Config config;

    private DateTime lastTime = DateTime.Now;

    public override Version Version => new Version("1.1.5");

    public override string Name => "Replenisher";

    public override string Author => "omni制作,nnt升级/汉化";

    public override string Description => "Replenish your world's resources!";

    public Replenisher(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("tshock.world.causeevents", this.Replen, "replen"));
        Commands.ChatCommands.Add(new Command("tshock.world.causeevents", this.ConfigReload, "replenreload"));
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        if (!this.ReadConfig())
        {
            TShock.Log.ConsoleError("错误的配置文件. 这很可能导致插件崩溃.");
        }
    }

    private void OnUpdate(EventArgs e)
    {
        if (DateTime.Now.Minute - this.lastTime.Minute <= this.config.AutoRefillTimerInMinutes)
        {
            return;
        }
        this.lastTime = DateTime.Now;
        if (this.config.ReplenChests)
        {
            TShock.Log.ConsoleInfo("自动生成宝箱...");
            this.PrivateReplenisher(GenType.chests, this.config.ChestAmount, 0);
        }
        if (this.config.ReplenLifeCrystals)
        {
            TShock.Log.ConsoleInfo("自动生成生命水晶...");
            this.PrivateReplenisher(GenType.lifecrystals, this.config.LifeCrystalAmount, 0);
        }
        if (this.config.ReplenOres)
        {
            TShock.Log.ConsoleInfo("自动生成矿石...");
            TileID tileID = new TileID();
            try
            {
                foreach (string item in this.config.OreToReplen)
                {
                    ushort oretype = (ushort) tileID.GetType().GetField(item.FirstCharToUpper()).GetValue(tileID);
                    this.PrivateReplenisher(GenType.ore, this.config.OreAmount, oretype);
                }
            }
            catch (ArgumentException)
            {
            }
        }
        if (this.config.ReplenPots)
        {
            TShock.Log.ConsoleInfo("自动生成罐子...");
            this.PrivateReplenisher(GenType.pots, this.config.PotsAmount, 0);
        }
        if (this.config.ReplenTrees)
        {
            TShock.Log.ConsoleInfo("自动生成树...");
            this.PrivateReplenisher(GenType.trees, this.config.TreesAmount, 0);
        }
    }

    private void CreateConfig()
    {
        string path = Path.Combine(TShock.SavePath, "ReplenisherConfig.json");
        try
        {
            using FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                this.config = new Config();
                string value = JsonConvert.SerializeObject(this.config, Formatting.Indented);
                streamWriter.Write(value);
            }
            fileStream.Close();
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.Message);
            this.config = new Config();
        }
    }

    private bool ReadConfig()
    {
        string path = Path.Combine(TShock.SavePath, "ReplenisherConfig.json");
        try
        {
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        string value = streamReader.ReadToEnd();
                        this.config = JsonConvert.DeserializeObject<Config>(value);
                    }
                    fileStream.Close();
                }
                return true;
            }
            TShock.Log.ConsoleError("Replenisher 配置文件未找到.正在自动生成...");
            this.CreateConfig();
            return true;
        }
        catch (JsonSerializationException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("错误的 Replenisher 配置文件. 请尝试让配置文件自动生成，并手动导入您的设置。如果这不起作用，请求助场外观众.");
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (Exception ex2)
        {
            TShock.Log.ConsoleError(ex2.Message);
        }
        return false;
    }

    private void ConfigReload(CommandArgs args)
    {
        if (this.ReadConfig())
        {
            args.Player.SendSuccessMessage("Replenisher 配置文件已重载.");
        }
        else
        {
            args.Player.SendErrorMessage("错误的配置文件.点击日志查看更多.");
        }
    }

    private bool PrivateReplenisher(GenType type, int amount, out int gend, ushort oretype = 0, CommandArgs args = null)
    {
        int num = gend = 0;
        for (int i = 0; i < TIMEOUT; i++)
        {
            bool flag = false;
            int num2 = WorldGen.genRand.Next(1, Main.maxTilesX);
            int num3 = 0;
            switch (type)
            {
                case GenType.ore:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    if (TShock.Regions.InAreaRegion(num2, num3).Any() && !this.config.GenerateInProtectedAreas)
                    {
                        flag = false;
                        break;
                    }
                    if (oretype != 58)
                    {
                        WorldGen.OreRunner(num2, num3, 2.0, amount, oretype);
                    }
                    else
                    {
                        WorldGen.OreRunner(num2, WorldGen.genRand.Next(Main.maxTilesY - 200, Main.maxTilesY), 2.0, amount, oretype);
                    }
                    flag = true;
                    break;
                case GenType.chests:
                    if (amount == 0)
                    {
                        int num4 = 0;
                        int num5 = 0;
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Main.chest[j] == null)
                            {
                                continue;
                            }
                            num4++;
                            bool flag2 = false;
                            Item[] item = Main.chest[j].item;
                            Item[] array = item;
                            Item[] array2 = array;
                            foreach (Item item2 in array2)
                            {
                                if (item2.netID != 0)
                                {
                                    flag2 = true;
                                }
                            }
                            if (!flag2)
                            {
                                num5++;
                                WorldGen.KillTile(Main.chest[j].x, Main.chest[j].y);
                                Main.chest[j] = null;
                            }
                        }
                        args.Player.SendSuccessMessage("Uprooted {0} empty out of {1} chests.", num5, num4);
                        return true;
                    }
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 200, Main.maxTilesY);
                    flag = (!TShock.Regions.InAreaRegion(num2, num3).Any() || this.config.GenerateInProtectedAreas) && WorldGen.AddBuriedChest(num2, num3, 0, notNearOtherChests: false, -1, trySlope: false, 0);
                    break;
                case GenType.pots:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    flag = (!TShock.Regions.InAreaRegion(num2, num3).Any() || this.config.GenerateInProtectedAreas) && WorldGen.PlacePot(num2, num3, 28);
                    break;
                case GenType.lifecrystals:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    flag = (!TShock.Regions.InAreaRegion(num2, num3).Any() || this.config.GenerateInProtectedAreas) && WorldGen.AddLifeCrystal(num2, num3);
                    break;
                case GenType.altars:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    if (TShock.Regions.InAreaRegion(num2, num3).Any() && !this.config.GenerateInProtectedAreas)
                    {
                        flag = false;
                        break;
                    }
                    WorldGen.Place3x2(num2, num3, 26);
                    flag = Main.tile[num2, num3].type == 26;
                    break;
                case GenType.trees:
                    WorldGen.AddTrees();
                    flag = true;
                    break;
                case GenType.floatingisland:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface + 175, (int) Main.worldSurface + 300);
                    if (TShock.Regions.InAreaRegion(num2, num3).Any() && !this.config.GenerateInProtectedAreas)
                    {
                        flag = false;
                        break;
                    }
                    WorldGen.FloatingIsland(num2, num3);
                    flag = true;
                    break;
            }
            if (flag)
            {
                num = gend = num + 1;
                if (num >= amount)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool PrivateReplenisher(GenType type, int amount, ushort oretype = 0, CommandArgs args = null)
    {
        int num = 0;
        for (int i = 0; i < TIMEOUT; i++)
        {
            bool flag = false;
            int num2 = WorldGen.genRand.Next(1, Main.maxTilesX);
            int num3 = 0;
            switch (type)
            {
                case GenType.ore:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    if (TShock.Regions.InAreaRegion(num2, num3).Any() && !this.config.GenerateInProtectedAreas)
                    {
                        flag = false;
                        break;
                    }
                    if (oretype != 58)
                    {
                        WorldGen.OreRunner(num2, num3, 2.0, amount, oretype);
                    }
                    else
                    {
                        WorldGen.OreRunner(num2, WorldGen.genRand.Next(Main.maxTilesY - 200, Main.maxTilesY), 2.0, amount, oretype);
                    }
                    flag = true;
                    break;
                case GenType.chests:
                    if (amount == 0)
                    {
                        int num4 = 0;
                        int num5 = 0;
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Main.chest[j] == null)
                            {
                                continue;
                            }
                            num4++;
                            bool flag2 = false;
                            Item[] item = Main.chest[j].item;
                            Item[] array = item;
                            Item[] array2 = array;
                            foreach (Item item2 in array2)
                            {
                                if (item2.netID != 0)
                                {
                                    flag2 = true;
                                }
                            }
                            if (!flag2)
                            {
                                num5++;
                                WorldGen.KillTile(Main.chest[j].x, Main.chest[j].y);
                                Main.chest[j] = null;
                            }
                        }
                        args.Player.SendSuccessMessage("Uprooted {0} empty out of {1} chests.", num5, num4);
                        return true;
                    }
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 200, Main.maxTilesY);
                    flag = (!TShock.Regions.InAreaRegion(num2, num3).Any() || this.config.GenerateInProtectedAreas) && WorldGen.AddBuriedChest(num2, num3, 0, notNearOtherChests: false, -1, trySlope: false, 0);
                    break;
                case GenType.pots:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    flag = (!TShock.Regions.InAreaRegion(num2, num3).Any() || this.config.GenerateInProtectedAreas) && WorldGen.PlacePot(num2, num3, 28);
                    break;
                case GenType.lifecrystals:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    flag = (!TShock.Regions.InAreaRegion(num2, num3).Any() || this.config.GenerateInProtectedAreas) && WorldGen.AddLifeCrystal(num2, num3);
                    break;
                case GenType.altars:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface - 12, Main.maxTilesY);
                    if (TShock.Regions.InAreaRegion(num2, num3).Any() && !this.config.GenerateInProtectedAreas)
                    {
                        flag = false;
                        break;
                    }
                    WorldGen.Place3x2(num2, num3, 26);
                    flag = Main.tile[num2, num3].type == 26;
                    break;
                case GenType.trees:
                    WorldGen.AddTrees();
                    flag = true;
                    break;
                case GenType.floatingisland:
                    num3 = WorldGen.genRand.Next((int) Main.worldSurface + 175, (int) Main.worldSurface + 300);
                    if (TShock.Regions.InAreaRegion(num2, num3).Any() && !this.config.GenerateInProtectedAreas)
                    {
                        flag = false;
                        break;
                    }
                    WorldGen.FloatingIsland(num2, num3);
                    flag = true;
                    break;
            }
            if (flag)
            {
                num++;
                if (num >= amount)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Replen(CommandArgs args)
    {
        GenType result = GenType.ore;
        int result2 = -1;
        ushort oretype = 0;
        int gend = 0;
        if (args.Parameters.Count >= 2 && Enum.TryParse<GenType>(args.Parameters[0], ignoreCase: true, out result) && int.TryParse(args.Parameters[1], out result2))
        {
            switch (result)
            {
                case GenType.ore:
                {
                    if (args.Parameters.Count < 3)
                    {
                        args.Player.SendErrorMessage("请输入正确的矿石类型.");
                        return;
                    }
                    TileID tileID = new TileID();
                    try
                    {
                        oretype = (ushort) tileID.GetType().GetField(args.Parameters[2].FirstCharToUpper()).GetValue(tileID);
                    }
                    catch (ArgumentException)
                    {
                        args.Player.SendErrorMessage("请输入正确的矿石类型.");
                    }
                    break;
                }
                case GenType.trees:
                    if (args.Parameters.Count >= 3)
                    {
                        args.Player.SendInfoMessage("注意:输入的数字不是树的总数。它指的是生成的一批的树木数量.");
                    }
                    break;
            }
            if (this.PrivateReplenisher(result, result2, out gend, oretype, args))
            {
                args.Player.SendInfoMessage(result.ToString().FirstCharToUpper() + " 已生成成功.");
                return;
            }
            args.Player.SendErrorMessage("无法全部生成 " + result.ToString() + ". 已生成 " + gend + " " + result.ToString() + ".");
        }
        else
        {
            args.Player.SendErrorMessage("错误的命令. 正确的用法: /replen <ore(矿石)|chests(宝箱)|pots(罐子)|lifecrystals(生命水晶)|altars(祭坛)|trees(树)|floatingisland(空岛)> <数量> (矿石类型)\r\n提示: 在生成树时，数量是以批为单位.");
        }
    }
}

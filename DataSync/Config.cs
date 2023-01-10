using Newtonsoft.Json;
using TShockAPI;

namespace DataSync
{
    public class Config
    {
        public static void GetConfig()
        {
            try
            {
                if (!File.Exists("tshock/DataSync.json"))
                {
                    FileTools.CreateIfNot(Path.Combine("tshock/DataSync.json"), JsonConvert.SerializeObject(Plugin.配置, Formatting.Indented));
                    Plugin.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/DataSync.json")));
                    File.WriteAllText("tshock/DataSync.json", JsonConvert.SerializeObject(Plugin.配置, Formatting.Indented));
                }
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[DataSync]配置文件读取错误！！！");
            }
        }
        public string 进度 = "史莱姆王,克苏鲁之眼,世界吞噬者,克苏鲁之脑,蜂王,骷髅王,独眼巨鹿,血肉墙,史莱姆皇后,双子魔眼,毁灭者,机械骷髅王,世纪之花,石巨人,猪龙鱼公爵,光之女皇,拜月教邪教徒,月亮领主,哥布林军队,海盗入侵,霜月,南瓜月,火星暴乱,日耀柱,星旋柱,星云柱,星尘柱";
        public bool 是否同步 = true;
        public bool 进度同步哥布林 = true;


        // Token: 0x04000007 RID: 7
        public bool 进度同步萌王 = true;

        // Token: 0x04000008 RID: 8
        public bool 进度同步鹿角怪 = true;

        // Token: 0x04000009 RID: 9
        public bool 进度同步克眼 = true;

        // Token: 0x0400000A RID: 10
        public bool 进度同步虫脑 = true;

        // Token: 0x0400000B RID: 11

        // Token: 0x0400000C RID: 12
        public bool 进度同步蜂后 = true;

        // Token: 0x0400000D RID: 13
        public bool 进度同步骷髅 = true;

        // Token: 0x0400000E RID: 14
        public bool 进度同步肉墙 = true;

        // Token: 0x0400000F RID: 15

        // Token: 0x04000010 RID: 16
        public bool 进度同步海盗 = true;

        // Token: 0x04000011 RID: 17

        // Token: 0x04000012 RID: 18
        public bool 进度同步萌后 = true;

        // Token: 0x04000013 RID: 19
        public bool 进度同步任一三王 = true;

        // Token: 0x04000014 RID: 20

        // Token: 0x04000015 RID: 21
        public bool 进度同步机械眼 = true;

        // Token: 0x04000016 RID: 22
        public bool 进度同步机械虫 = true;

        // Token: 0x04000017 RID: 23
        public bool 进度同步机械骷髅 = true;

        // Token: 0x04000018 RID: 24

        // Token: 0x04000019 RID: 25

        // Token: 0x0400001A RID: 26
        public bool 进度同步猪鲨前 = true;

        // Token: 0x0400001B RID: 27
        public bool 进度同步妖花前 = true;

        // Token: 0x0400001C RID: 28

        // Token: 0x0400001D RID: 29
        public bool 进度同步霜月树 = true;

        // Token: 0x0400001E RID: 30
        public bool 进度同步霜月坦 = true;

        // Token: 0x0400001F RID: 31
        public bool 进度同步霜月后 = true;

        // Token: 0x04000020 RID: 32
        public bool 进度同步南瓜树 = true;

        // Token: 0x04000021 RID: 33
        public bool 进度同步南瓜王 = true;

        // Token: 0x04000022 RID: 34
        public bool 进度同步光女前 = true;

        // Token: 0x04000023 RID: 35
        public bool 进度同步石巨人 = true;

        // Token: 0x04000024 RID: 36

        // Token: 0x04000025 RID: 37

        // Token: 0x04000026 RID: 38
        public bool 进度同步教徒前 = true;

        // Token: 0x04000027 RID: 39
        public bool 进度同步日耀前 = true;

        // Token: 0x04000028 RID: 40
        public bool 进度同步星旋前 = true;

        // Token: 0x04000029 RID: 41
        public bool 进度同步星尘前 = true;

        // Token: 0x0400002A RID: 42
        public bool 进度同步星云前 = true;

        // Token: 0x0400002B RID: 43

        // Token: 0x0400002C RID: 44
        public bool 进度同步月总 = true;

        /*
        public bool 哥布林一进行 = false;

        // Token: 0x0400002E RID: 46
        public bool 旧日军团一 = false;

        // Token: 0x0400002F RID: 47
        public bool 哥布林二进行 = false;

        // Token: 0x04000030 RID: 48
        public bool 海盗进行 = false;

        // Token: 0x04000031 RID: 49
        public bool 日蚀一进行 = false;

        // Token: 0x04000032 RID: 50
        public bool 旧日军团二 = false;

        // Token: 0x04000033 RID: 51
        public bool 日蚀二进行 = false;

        // Token: 0x04000034 RID: 52
        public bool 日蚀三进行 = false;

        // Token: 0x04000035 RID: 53
        public bool 旧日军团三 = false;

        // Token: 0x04000036 RID: 54
        public bool 火星进行 = false;
        */
    }
}
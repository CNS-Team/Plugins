using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace AntiItemCheating;

public class ConfigFile
{
	public bool 连接远程服务器判断进度 = false;

	public bool 封禁玩家 = false;

	public bool 广播超进度 = false;

	public bool 启动惩罚 = true;

	public List<int> 物品白名单 = new List<int>();

	public List<int> 物品黑名单 = new List<int>();

	public bool 是否启用远程进度判断哥布林 = false;

	public List<int> 哥布林一前禁物品 = new List<int>();

	public bool 是否启用远程进度判断萌王 = false;

	public List<int> 萌王前禁物品 = new List<int>();

	public bool 是否启用远程进度判断鹿角怪 = false;

	public List<int> 鹿角怪前禁物品 = new List<int>();

	public bool 是否启用远程进度判断克眼 = false;

	public List<int> 克眼前禁物品 = new List<int>();

	public bool 是否启用远程进度判断虫脑 = false;

	public List<int> 虫脑前禁物品 = new List<int>();

	public bool 是否启用远程进度判断旧日一 = false;

	public List<int> 旧日一前禁物品 = new List<int>();

	public bool 是否启用远程进度判断蜂后 = false;

	public List<int> 蜂后前禁物品 = new List<int>();

	public bool 是否启用远程进度判断骷髅 = false;

	public List<int> 骷髅前禁物品 = new List<int>();

	public bool 是否启用远程进度判断肉墙 = false;

	public List<int> 肉墙前禁物品 = new List<int>();

	public bool 是否启用远程进度判断哥布林二 = false;

	public List<int> 哥布林二前禁物品 = new List<int>();

	public bool 是否启用远程进度判断海盗 = false;

	public List<int> 海盗前禁物品 = new List<int>();

	public bool 是否启用远程进度判断日蚀一 = false;

	public List<int> 日蚀一前禁物品 = new List<int>();

	public bool 是否启用远程进度判断萌后 = false;

	public List<int> 萌后前禁物品 = new List<int>();

	public bool 是否启用远程进度判断任一三王 = false;

	public List<int> 任一三王前禁物品 = new List<int>();

	public bool 是否启用远程进度判断旧日二 = false;

	public List<int> 旧日二前禁物品 = new List<int>();

	public bool 是否启用远程进度判断机械眼 = false;

	public List<int> 机械眼前禁物品 = new List<int>();

	public bool 是否启用远程进度判断机械虫 = false;

	public List<int> 机械虫前禁物品 = new List<int>();

	public bool 是否启用远程进度判断机械骷髅 = false;

	public List<int> 机械骷髅前禁物品 = new List<int>();

	public bool 是否启用远程进度判断三王前 = false;

	public List<int> 三王前禁物品 = new List<int>();

	public bool 是否启用远程进度判断日蚀二 = false;

	public List<int> 日蚀二前禁物品 = new List<int>();

	public bool 是否启用远程进度判断猪鲨前 = false;

	public List<int> 猪鲨前禁物品 = new List<int>();

	public bool 是否启用远程进度判断妖花前 = false;

	public List<int> 妖花前禁物品 = new List<int>();

	public bool 是否启用远程进度判断日蚀三 = false;

	public List<int> 日蚀三前禁物品 = new List<int>();

	public bool 是否启用远程进度判断霜月树 = false;

	public List<int> 霜月树前禁物品 = new List<int>();

	public bool 是否启用远程进度判断霜月坦 = false;

	public List<int> 霜月坦前禁物品 = new List<int>();

	public bool 是否启用远程进度判断霜月后 = false;

	public List<int> 霜月后前禁物品 = new List<int>();

	public bool 是否启用远程进度判断南瓜树 = false;

	public List<int> 南瓜树前禁物品 = new List<int>();

	public bool 是否启用远程进度判断南瓜王 = false;

	public List<int> 南瓜王前禁物品 = new List<int>();

	public bool 是否启用远程进度判断光女前 = false;

	public List<int> 光女前禁物品 = new List<int>();

	public bool 是否启用远程进度判断石巨人 = false;

	public List<int> 石巨人前禁物品 = new List<int>();

	public bool 是否启用远程进度判断旧日三 = false;

	public List<int> 旧日三前禁物品 = new List<int>();

	public bool 是否启用远程进度判断外星人 = false;

	public List<int> 外星人前禁物品 = new List<int>();

	public bool 是否启用远程进度判断教徒前 = false;

	public List<int> 教徒前禁物品 = new List<int>();

	public bool 是否启用远程进度判断日耀前 = false;

	public List<int> 日耀前禁物品 = new List<int>();

	public bool 是否启用远程进度判断星旋前 = false;

	public List<int> 星旋前禁物品 = new List<int>();

	public bool 是否启用远程进度判断星尘前 = false;

	public List<int> 星尘前禁物品 = new List<int>();

	public bool 是否启用远程进度判断星云前 = false;

	public List<int> 星云前禁物品 = new List<int>();

	public bool 是否启用远程进度判断所有柱子 = false;

	public List<int> 所有柱子前禁物品 = new List<int>();

	public bool 是否启用远程进度判断月总 = false;

	public List<int> 月总前禁物品 = new List<int>();

	public bool 哥布林一进行 = false;

	public bool 旧日军团一 = false;

	public bool 哥布林二进行 = false;

	public bool 海盗进行 = false;

	public bool 日蚀一进行 = false;

	public bool 旧日军团二 = false;

	public bool 日蚀二进行 = false;

	public bool 日蚀三进行 = false;

	public bool 旧日军团三 = false;

	public bool 火星进行 = false;

	public static readonly string ConfigPath = Path.Combine(TShock.SavePath, "超进度物品限制.json");

	public static Action<ConfigFile> ConfigR;

	public static ConfigFile Read(string path)
	{
		if (!File.Exists(path))
		{
			return new ConfigFile
			{
				物品白名单 = new List<int>(),
				物品黑名单 = new List<int>(),
				哥布林一前禁物品 = new List<int>
				{
					486, 160, 398, 3251, 3990, 1861, 1163, 1250, 1164, 399,
					394, 3993, 1863, 1724, 3994, 3995, 3996, 1862, 3250, 1860,
					898, 3252, 128, 983, 3241, 405, 976, 1251, 1252, 395,
					3121, 3036, 2221, 1595, 4000, 555, 982, 407, 3061, 3721,
					5064
				},
				萌王前禁物品 = new List<int> { 2585, 256, 257, 258, 998, 3090, 2430, 3318 },
				鹿角怪前禁物品 = new List<int>
				{
					5108, 5110, 5111, 5100, 5113, 5109, 5101, 5095, 5117, 5118,
					5119, 5098
				},
				克眼前禁物品 = new List<int> { 3349, 3262, 3097, 3319 },
				虫脑前禁物品 = new List<int>
				{
					103, 798, 122, 204, 217, 104, 797, 4821, 198, 199,
					200, 201, 202, 203, 4258, 121, 801, 120, 197, 127,
					2365, 3829, 3818, 3832, 3824, 234, 3266, 3267, 3268, 123,
					124, 125, 102, 101, 100, 792, 793, 794, 231, 232,
					233, 116, 174, 117, 175, 4874, 908, 4038, 396, 907,
					5000, 3999, 193, 4004, 3224, 3223, 1354, 86, 1329, 3828,
					1362, 2104, 2111, 3060, 994, 4131, 4076, 3320, 3321
				},
				旧日一前禁物品 = new List<int> { 4796 },
				蜂后前禁物品 = new List<int>
				{
					1129, 1123, 1130, 2888, 1121, 2364, 2361, 2362, 2363, 1249,
					1247, 1132, 4007, 1578, 3333, 1158, 1358, 1359, 2502, 2431,
					2108, 842, 843, 844, 1170, 3322
				},
				骷髅前禁物品 = new List<int>
				{
					509, 850, 851, 3612, 510, 3611, 3620, 3625, 1273, 2296,
					3124, 4707, 932, 155, 3317, 3282, 274, 4818, 119, 163,
					220, 154, 3019, 164, 219, 113, 157, 112, 218, 1313,
					5074, 273, 151, 152, 153, 959, 3000, 2192, 2999, 3095,
					3122, 3123, 3619, 2799, 156, 397, 1307, 5010, 3245, 2739,
					538, 513, 3626, 3630, 3631, 3632, 3707, 583, 584, 585,
					4484, 4485, 581, 582, 530, 849, 3629, 3617, 3616, 420,
					3602, 3618, 3663, 1281, 4443, 329, 3205, 3323
				},
				肉墙前禁物品 = new List<int>
				{
					776, 385, 1188, 1189, 777, 386, 1195, 1196, 778, 388,
					1202, 1203, 991, 383, 1222, 1190, 992, 384, 1223, 1197,
					993, 387, 1224, 1204, 4317, 660, 367, 437, 3020, 3022,
					3023, 3623, 2295, 2422, 1326, 3031, 3032, 496, 659, 3351,
					3258, 483, 1185, 3764, 3765, 3766, 3767, 3768, 3769, 4259,
					1306, 484, 1192, 426, 676, 482, 723, 1199, 3013, 3211,
					3315, 3316, 3283, 3290, 3289, 537, 1186, 390, 1193, 406,
					1200, 2331, 3030, 4760, 1324, 2424, 1314, 4272, 3012, 389,
					4788, 661, 682, 725, 3029, 435, 1187, 436, 1194, 481,
					1201, 434, 2270, 534, 3788, 1265, 3210, 3007, 3008, 3787,
					3209, 1264, 726, 3051, 3006, 2750, 1308, 514, 4348, 519,
					1336, 518, 3014, 1244, 517, 3269, 3779, 4270, 4758, 2551,
					4269, 2366, 4912, 4911, 422, 515, 546, 1335, 1349, 1351,
					1352, 3104, 516, 545, 1334, 3103, 3009, 3010, 3011, 2673,
					736, 737, 738, 2370, 2371, 2372, 374, 375, 371, 372,
					373, 1208, 1209, 1207, 1205, 1206, 379, 380, 376, 377,
					378, 1213, 1214, 1212, 1210, 1211, 403, 404, 400, 401,
					402, 1218, 1219, 1217, 1215, 1216, 684, 685, 686, 3776,
					3777, 3778, 525, 1220, 524, 1221, 1430, 487, 996, 3064,
					502, 3795, 647, 364, 1104, 365, 1105, 366, 1106, 391,
					1198, 381, 382, 1191, 1184, 1162, 761, 2494, 822, 785,
					1515, 749, 821, 3991, 860, 4001, 535, 885, 1612, 1613,
					901, 886, 3992, 888, 485, 904, 554, 889, 3016, 1253,
					1321, 902, 890, 4002, 4003, 891, 3781, 897, 3015, 491,
					489, 4006, 532, 862, 2998, 903, 536, 893, 892, 490,
					3334, 3366, 493, 492, 748, 499, 2209, 500, 2345, 2352,
					2326, 1353, 1355, 1356, 3771, 2429, 4795, 3794, 507, 1611,
					522, 527, 1432, 3186, 1347, 1518, 3783, 2161, 1348, 508,
					1519, 1332, 528, 501, 531, 2607, 1521, 1328, 526, 575,
					520, 521, 4783, 4763, 4988, 544, 557, 556, 1315, 602,
					4428, 3335, 3614, 1365, 4930, 5004, 1312, 3984, 3979, 3980,
					3981, 3982, 3983, 3984, 3985, 3986, 3987, 4406, 4408, 5003,
					423, 3091, 3092, 3324
				},
				哥布林二前禁物品 = new List<int> { 3052, 3053, 3054 },
				海盗前禁物品 = new List<int>
				{
					672, 905, 2584, 928, 1337, 3034, 854, 3033, 3035, 855,
					3035, 4792, 3359, 4940, 1180, 4471
				},
				日蚀一前禁物品 = new List<int> { 497, 1165, 861, 900, 1520 },
				萌后前禁物品 = new List<int>
				{
					4980, 4982, 4983, 4984, 4987, 4981, 4958, 4950, 4960, 4986,
					4957
				},
				任一三王前禁物品 = new List<int>
				{
					787, 779, 368, 3284, 3286, 3287, 3288, 550, 756, 4790,
					578, 4060, 683, 4678, 3830, 3819, 3833, 3825, 1302, 780,
					781, 782, 783, 784, 3800, 3801, 3802, 3806, 3807, 3808,
					3803, 3804, 3805, 3797, 3798, 3799, 551, 552, 559, 553,
					558, 4873, 4900, 4901, 4896, 4897, 4898, 4899, 995, 4142,
					2193, 2203, 1225, 665, 1583, 1584, 1585, 1586, 3228, 3580,
					3582, 3588, 3592, 3924, 3928, 4730, 4746, 4750, 4754, 1263,
					4472, 1291
				},
				旧日二前禁物品 = new List<int> { 3852, 3854, 3823, 3835, 3836, 3809, 3810, 3811, 3812 },
				机械眼前禁物品 = new List<int> { 495, 494, 2535, 3354, 549, 4931, 3326 },
				机械虫前禁物品 = new List<int> { 561, 533, 3355, 548, 4932, 3325 },
				机械骷髅前禁物品 = new List<int> { 506, 3356, 547, 4933, 3327 },
				三王前禁物品 = new List<int>
				{
					1230, 1231, 990, 579, 1233, 1232, 1234, 1262, 1227, 1226,
					1228, 1229, 2188, 1179, 1235, 1004, 1005, 1001, 1002, 1003,
					1316, 1317, 1318, 947, 1006, 2220, 935, 1343, 936, 3353
				},
				日蚀二前禁物品 = new List<int> { 1327, 757, 674, 675 },
				猪鲨前禁物品 = new List<int>
				{
					2611, 2624, 2623, 2622, 2621, 2609, 3367, 2589, 4936, 2588,
					4808, 3330
				},
				妖花前禁物品 = new List<int>
				{
					1506, 2176, 1507, 1305, 1543, 1544, 1545, 3021, 671, 3018,
					3291, 1513, 1259, 1571, 4789, 1569, 2223, 1255, 679, 1254,
					758, 760, 759, 1156, 788, 1445, 1444, 1446, 5065, 1155,
					1178, 1260, 1266, 1157, 4607, 1572, 4679, 937, 1342, 1350,
					1341, 771, 772, 773, 774, 4445, 4446, 4459, 4447, 4448,
					4449, 4457, 4458, 1159, 1160, 1161, 1549, 1550, 1546, 1548,
					1547, 1504, 1505, 2189, 1503, 2195, 3261, 1552, 984, 977,
					1866, 786, 823, 963, 3997, 3998, 938, 1300, 1167, 4409,
					3336, 4013, 1357, 1340, 1146, 1149, 1148, 1147, 1517, 1508,
					1346, 2766, 1339, 1293, 1958, 4961, 2767, 1844, 4934, 2109,
					4806, 1182, 1141, 3328
				},
				日蚀三前禁物品 = new List<int> { 3098, 3249, 3105, 3107, 3106, 3292, 3108, 2770, 1570 },
				霜月树前禁物品 = new List<int> { 1916, 1928, 1930, 1871, 1962, 4944, 4813 },
				霜月坦前禁物品 = new List<int> { 1929, 1910, 4794, 1961, 4945 },
				霜月后前禁物品 = new List<int> { 1947, 1946, 1931, 1914, 1960, 4943, 1959, 4814 },
				南瓜树前禁物品 = new List<int>
				{
					1829, 1835, 1836, 1832, 1833, 1834, 1830, 1845, 1864, 4444,
					4793, 1831, 1855, 4941, 1837
				},
				南瓜王前禁物品 = new List<int>
				{
					1826, 1782, 1784, 1801, 1802, 4680, 1783, 1785, 1797, 1811,
					1856, 4942, 4812
				},
				光女前禁物品 = new List<int>
				{
					4923, 4953, 4952, 4715, 5005, 4914, 4823, 5075, 4989, 4783,
					4949, 4811, 4778, 4782
				},
				石巨人前禁物品 = new List<int>
				{
					1294, 1122, 1297, 1258, 3546, 1296, 1295, 3831, 3820, 3834,
					3826, 1261, 2199, 2202, 2200, 2201, 3871, 3872, 3873, 3880,
					3881, 3882, 3877, 3878, 3879, 3874, 3875, 3876, 1292, 2280,
					948, 1865, 3110, 1301, 1248, 4005, 1858, 899, 3337, 2218,
					1371, 4935, 4807, 4468, 3329
				},
				旧日三前禁物品 = new List<int> { 3827, 3858, 3859, 3870, 3883, 4817, 3860 },
				外星人前禁物品 = new List<int>
				{
					2798, 2800, 2880, 2797, 2796, 2795, 2882, 2749, 2771, 2769,
					3358, 4939
				},
				教徒前禁物品 = new List<int> { 3549, 3357, 4937, 3372, 4809, 3331 },
				日耀前禁物品 = new List<int> { 3543, 3473, 3539, 3458, 3526 },
				星旋前禁物品 = new List<int> { 3540, 3475, 3536, 3456, 3527 },
				星尘前禁物品 = new List<int> { 3474, 3531, 3538, 3459, 3528 },
				星云前禁物品 = new List<int> { 3476, 3542, 3537, 3457, 3529 },
				所有柱子前禁物品 = new List<int> { 3572, 3544, 3601 },
				月总前禁物品 = new List<int>
				{
					2776, 2781, 2786, 3466, 2774, 2779, 2784, 3464, 3523, 3524,
					3522, 3525, 3384, 3065, 3063, 3389, 4956, 1553, 3930, 3570,
					3541, 3569, 3571, 3567, 3568, 2763, 2764, 2765, 2757, 2758,
					2759, 2760, 2761, 2762, 3381, 3382, 3383, 3460, 3467, 3470,
					3469, 3468, 3471, 4954, 1131, 3664, 4318, 2768, 3595, 4938,
					3373, 3577, 3530, 3461, 4469, 3332
				}
			};
		}
		using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		return Read(stream);
	}

	public static ConfigFile Read(Stream stream)
	{
		using StreamReader streamReader = new StreamReader(stream);
		ConfigFile configFile = JsonConvert.DeserializeObject<ConfigFile>(streamReader.ReadToEnd());
		if (ConfigR != null)
		{
			ConfigR(configFile);
		}
		return configFile;
	}

	public void Write(string path)
	{
		using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
		Write(stream);
	}

	public void Write(Stream stream)
	{
		string value = JsonConvert.SerializeObject((object)this, (Formatting)1);
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(value);
	}

	public static void WriteConfig(ConfigFile config)
	{
		File.WriteAllText(ConfigPath, JsonConvert.SerializeObject((object)config));
	}
}

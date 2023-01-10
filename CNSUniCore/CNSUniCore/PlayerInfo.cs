using System;

namespace CNSUniCore
{
	// Token: 0x02000005 RID: 5
	public class PlayerInfo
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002F85 File Offset: 0x00001185
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002F8D File Offset: 0x0000118D
		public string Name { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002F96 File Offset: 0x00001196
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002F9E File Offset: 0x0000119E
		public string IP { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002FA7 File Offset: 0x000011A7
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002FAF File Offset: 0x000011AF
		public string UUID { get; set; }
		public string Reason { get; set; } = "";
		public string BanTime { get; set; } = "-1";

		public string AddTime { get; set; } = "2023/1/1 11:45:14";
    }
}

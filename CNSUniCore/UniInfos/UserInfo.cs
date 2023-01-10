using System;

namespace CNSUniCore.UniInfos
{
	// Token: 0x0200000A RID: 10
	public class UserInfo
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00003FB3 File Offset: 0x000021B3
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00003FBB File Offset: 0x000021BB
		public int ID { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00003FC4 File Offset: 0x000021C4
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00003FCC File Offset: 0x000021CC
		public string Name { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00003FD5 File Offset: 0x000021D5
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00003FDD File Offset: 0x000021DD
		public string Password { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00003FE6 File Offset: 0x000021E6
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00003FEE File Offset: 0x000021EE
		public string UUID { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00003FF7 File Offset: 0x000021F7
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00003FFF File Offset: 0x000021FF
		public string UserGroup { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00004008 File Offset: 0x00002208
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00004010 File Offset: 0x00002210
		public string Registered { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00004019 File Offset: 0x00002219
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00004021 File Offset: 0x00002221
		public string LastAccessed { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000067 RID: 103 RVA: 0x0000402A File Offset: 0x0000222A
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00004032 File Offset: 0x00002232
		public string KnownIPs { get; set; }
	}
}

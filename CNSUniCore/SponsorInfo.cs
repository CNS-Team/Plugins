using System;

namespace CNSUniCore
{
	// Token: 0x02000007 RID: 7
	public class SponsorInfo
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00003687 File Offset: 0x00001887
		public SponsorInfo(string name, string origin, string group, DateTime now, DateTime end)
		{
			this.name = name;
			this.originGroup = origin;
			this.group = group;
			this.startTime = now;
			this.endTime = end;
		}

		// Token: 0x04000015 RID: 21
		public string name;

		// Token: 0x04000016 RID: 22
		public string originGroup;

		// Token: 0x04000017 RID: 23
		public string group;

		// Token: 0x04000018 RID: 24
		public DateTime startTime;

		// Token: 0x04000019 RID: 25
		public DateTime endTime;
	}
}

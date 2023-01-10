using System;

namespace CNSUniCore
{	public class SponsorInfo
	{
		public SponsorInfo(string name, string origin, string group, DateTime now, DateTime end)
		{
			this.name = name;
			this.originGroup = origin;
			this.group = group;
			this.startTime = now;
			this.endTime = end;
		}

		public string name;

		public string originGroup;

		public string group;

		public DateTime startTime;

		public DateTime endTime;
	}
}

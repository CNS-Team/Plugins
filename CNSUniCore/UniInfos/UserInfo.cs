using System;

namespace CNSUniCore.UniInfos
{
	public class UserInfo
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public string Password { get; set; }
		public string UUID { get; set; }

		public string UserGroup { get; set; }
		public string Registered { get; set; }
		public string LastAccessed { get; set; }
		public string KnownIPs { get; set; }
	}
}

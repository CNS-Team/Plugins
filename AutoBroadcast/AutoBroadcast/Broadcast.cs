namespace AutoBroadcast
{
	public class Broadcast
	{
		public string Name = string.Empty;

		public bool Enabled = false;

		public string[] Messages = new string[0];

		public float[] ColorRGB = new float[3];

		public int Interval = 0;

		public int StartDelay = 0;

		public string[] TriggerRegions = new string[0];

		public string RegionTrigger = "none";

		public string[] Groups = new string[0];

		public string[] TriggerWords = new string[0];

		public bool TriggerToWholeGroup = false;
	}
}

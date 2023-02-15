using System;

namespace ProgressCommonSystem;

public class BossProgressAdvancedEventArgs : EventArgs
{
	public bool Handled { get; set; }

	public BossProgress BossProgress { get; internal set; }
}

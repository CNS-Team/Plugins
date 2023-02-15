using System;

namespace ProgressCommonSystem;

public class EventProgressAdvancedEventArgs : EventArgs
{
	public bool Handled { get; set; }

	public EventProgress EventProgress { get; internal set; }
}

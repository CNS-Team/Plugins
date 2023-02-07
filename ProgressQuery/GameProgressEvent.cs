namespace ProgressQuery;

public delegate void OnGameProgressHandler(OnGameProgressEventArgs e);
public class OnGameProgressEventArgs : EventArgs
{
    public string? Name { get; set; }

    public bool code { get; set; }
}

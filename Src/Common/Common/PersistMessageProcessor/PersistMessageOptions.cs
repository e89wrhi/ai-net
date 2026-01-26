namespace AI.Common.PersistMessageProcessor;

public class PersistMessageOptions
{
    public int? Interval { get; set; } = 30;
    public bool Enabled { get; set; } = true;
    public string? ConnectionString { get; set; }
}
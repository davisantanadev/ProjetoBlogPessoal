namespace BlogPessoal.Config;

public class AiOptions
{
    public string Provider { get; set; } = "Local";
    public string? Endpoint { get; set; }
    public string? ApiKey { get; set; }
    public string? Model { get; set; }
    public int MaxSummaryLength { get; set; } = 220;
}

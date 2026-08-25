namespace PragueMicroclimateProject.Options;

/// <summary>
/// Logstash configuration options.
/// </summary>
public class LogstashOptions
{
    /// <summary>
    /// Section name in configuration.
    /// </summary>
    public const string SectionName = "Logstash";

    /// <summary>
    /// Logstash HTTP input request URI.
    /// </summary>
    public string? RequestUri { get; set; }

    /// <summary>
    /// Maximum durable HTTP sink queue size in bytes.
    /// </summary>
    public long? QueueLimitBytes { get; set; }
}

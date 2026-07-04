namespace PragueMicroclimateProject.Options;

/// <summary>
/// Application configuration options.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// Section name in configuration.
    /// </summary>
    public const string SectionName = "Application";

    /// <summary>
    /// Enabled measurement types.
    /// </summary>
    public List<string> EnabledMeasurementTypes { get; set; } = [];
}

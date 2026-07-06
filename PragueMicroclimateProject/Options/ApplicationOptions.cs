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

    /// <summary>
    /// Maximum number of calendar months allowed in a single measurement request.
    /// </summary>
    public int MaxCalendarMonthsPerRequest { get; set; } = 3;
}

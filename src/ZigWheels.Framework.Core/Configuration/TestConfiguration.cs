namespace ZigWheels.Framework.Core.Configuration;

/// <summary>
/// Provides centralised runtime configuration for the automation framework.
///
/// Environment variables are used where available so that machine-specific
/// settings and sensitive values do not need to be hardcoded into tests.
/// </summary>
public static class TestConfiguration
{
    public static string BaseUrl =>
        GetEnvironmentVariable(
            "ZIGWHEELS_BASE_URL",
            "https://www.zigwheels.com/");

    public static string Browser =>
        GetEnvironmentVariable(
            "BROWSER",
            "chrome");

    public static bool Headless =>
        bool.TryParse(
            GetEnvironmentVariable("HEADLESS", "false"),
            out var headless) && headless;

    public static bool UseSeleniumGrid =>
        bool.TryParse(
            GetEnvironmentVariable("SELENIUM_USE_GRID", "false"),
            out var useGrid) && useGrid;

    public static string SeleniumGridUrl =>
        GetEnvironmentVariable(
            "SELENIUM_GRID_URL",
            "http://localhost:4444");

    public static string TestEmail =>
        Environment.GetEnvironmentVariable("ZIGWHEELS_TEST_EMAIL")
        ?? string.Empty;

    public static string TestPassword =>
        Environment.GetEnvironmentVariable("ZIGWHEELS_TEST_PASSWORD")
        ?? string.Empty;

    private static string GetEnvironmentVariable(
        string variableName,
        string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(variableName);

        return string.IsNullOrWhiteSpace(value)
            ? defaultValue
            : value;
    }
}
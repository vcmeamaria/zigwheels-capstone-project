namespace ZigWheels.Framework.Core.Utilities;

/// <summary>
/// Provides centralised paths for automation artefacts.
///
/// Artefacts are organised by:
/// artefact type → date → framework → scenario → timestamp.
///
/// Example:
/// artifacts/screenshots/2026-09-24/Selenium/GoogleLogin/
/// 14-15-22-418_failure.png
/// </summary>
public static class ArtifactPaths
{
    private const string ArtifactsFolderName = "artifacts";

    /// <summary>
    /// Creates a timestamp that can be shared by multiple artefacts
    /// generated from the same test execution.
    /// </summary>
    public static string CreateRunTimestamp()
    {
        return DateTime.Now.ToString(
            "HH-mm-ss-fff");
    }

    /// <summary>
    /// Creates and returns a path for a failure screenshot.
    /// </summary>
    public static string CreateScreenshotPath(
        string framework,
        string scenarioName,
        string? timestamp = null)
    {
        var directory = CreateScenarioDirectory(
            "screenshots",
            framework,
            scenarioName);

        var effectiveTimestamp =
            timestamp ?? CreateRunTimestamp();

        var fileName =
            $"{effectiveTimestamp}_failure.png";

        return Path.Combine(
            directory,
            fileName);
    }

    /// <summary>
    /// Creates and returns a path for a Playwright trace archive.
    /// </summary>
    public static string CreateTracePath(
        string framework,
        string scenarioName,
        string? timestamp = null)
    {
        var directory = CreateScenarioDirectory(
            "traces",
            framework,
            scenarioName);

        var effectiveTimestamp =
            timestamp ?? CreateRunTimestamp();

        var fileName =
            $"{effectiveTimestamp}_trace.zip";

        return Path.Combine(
            directory,
            fileName);
    }

    /// <summary>
    /// Creates and returns a path for a framework log file.
    /// </summary>
    public static string CreateLogPath(
        string framework,
        string scenarioName,
        string? timestamp = null)
    {
        var directory = CreateScenarioDirectory(
            "logs",
            framework,
            scenarioName);

        var effectiveTimestamp =
            timestamp ?? CreateRunTimestamp();

        var fileName =
            $"{effectiveTimestamp}.log";

        return Path.Combine(
            directory,
            fileName);
    }

    /// <summary>
    /// Returns the shared Allure results directory.
    ///
    /// Allure manages its own generated filenames, so the directory
    /// is intentionally not divided by date or framework.
    /// </summary>
    public static string GetAllureResultsDirectory()
    {
        var directory = Path.Combine(
            FindRepositoryRoot(),
            ArtifactsFolderName,
            "allure-results");

        Directory.CreateDirectory(directory);

        return directory;
    }

    /// <summary>
    /// Creates a scenario-specific artefact directory.
    /// </summary>
    private static string CreateScenarioDirectory(
        string artifactType,
        string framework,
        string scenarioName)
    {
        var date =
            DateTime.Now.ToString("yyyy-MM-dd");

        var safeFramework =
            SanitisePathSegment(framework);

        var safeScenario =
            SanitisePathSegment(scenarioName);

        var directory = Path.Combine(
            FindRepositoryRoot(),
            ArtifactsFolderName,
            artifactType,
            date,
            safeFramework,
            safeScenario);

        Directory.CreateDirectory(directory);

        return directory;
    }

    /// <summary>
    /// Removes characters that cannot safely be used in
    /// Windows directory and file names.
    /// </summary>
    private static string SanitisePathSegment(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Unknown";
        }

        var invalidCharacters =
            Path.GetInvalidFileNameChars();

        var cleanedCharacters =
            value.Trim()
                .Select(character =>
                    invalidCharacters.Contains(character)
                        ? '_'
                        : character)
                .ToArray();

        var cleanedValue =
            new string(cleanedCharacters);

        cleanedValue =
            cleanedValue.Replace(
                ' ',
                '_');

        return string.IsNullOrWhiteSpace(cleanedValue)
            ? "Unknown"
            : cleanedValue;
    }

    /// <summary>
    /// Locates the repository root by walking upwards from
    /// the test execution directory until the solution or
    /// Git repository is found.
    /// </summary>
    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            var solutionPath = Path.Combine(
                directory.FullName,
                "ZigWheels.Capstone.project.sln");

            var gitDirectory = Path.Combine(
                directory.FullName,
                ".git");

            if (File.Exists(solutionPath) ||
                Directory.Exists(gitDirectory))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "The ZigWheels Capstone repository root could not be located.");
    }
}
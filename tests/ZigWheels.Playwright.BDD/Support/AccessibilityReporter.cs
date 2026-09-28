using Deque.AxeCore.Commons;

namespace ZigWheels.Playwright.BDD.Support;

/// <summary>
/// Creates accessibility evidence files for axe-core scans.
///
/// Reports are stored outside generated test-output folders so that
/// accessibility evidence is easy to locate and review independently.
/// </summary>
public static class AccessibilityReporter
{
    /// <summary>
    /// Saves the complete axe accessibility result as JSON.
    /// </summary>
    /// <param name="axeResult">The result returned by axe-core.</param>
    /// <param name="scenarioName">The current BDD scenario name.</param>
    /// <returns>The full path of the generated report.</returns>
    public static async Task<string> SaveReportAsync(
        AxeResult axeResult,
        string scenarioName)
    {
        ArgumentNullException.ThrowIfNull(axeResult);

        if (string.IsNullOrWhiteSpace(scenarioName))
        {
            throw new ArgumentException(
                "A scenario name is required to create an accessibility report.",
                nameof(scenarioName));
        }

        var repositoryRoot = FindRepositoryRoot();

        var dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
        var scenarioFolder = SanitiseFileName(scenarioName);
        var timestamp = DateTime.Now.ToString("HH-mm-ss-fff");

        var reportDirectory = Path.Combine(
            repositoryRoot,
            "accessibility",
            "reports",
            dateFolder,
            scenarioFolder);

        Directory.CreateDirectory(reportDirectory);

        var reportPath = Path.Combine(
            reportDirectory,
            $"{timestamp}_accessibility.json");

        var json = axeResult.ToString();

        await File.WriteAllTextAsync(
            reportPath,
            json);

        return reportPath;
    }

    /// <summary>
    /// Prints a readable accessibility summary to the test output.
    /// </summary>
    public static void WriteSummary(AxeResult axeResult)
    {
        ArgumentNullException.ThrowIfNull(axeResult);

        var violations = axeResult.Violations;

        if (violations.Length == 0)
        {
            Console.WriteLine(
                "Accessibility scan completed with no axe violations.");

            return;
        }

        Console.WriteLine(
            $"Accessibility scan completed. Violations found: {violations.Length}");

        foreach (var violation in violations)
        {
            Console.WriteLine();

            Console.WriteLine(
                $"Rule: {violation.Id}");

            Console.WriteLine(
                $"Impact: {violation.Impact ?? "not specified"}");

            Console.WriteLine(
                $"Help: {violation.Help}");

            Console.WriteLine(
                $"Help URL: {violation.HelpUrl}");

            Console.WriteLine(
                $"Affected elements: {violation.Nodes.Length}");

            foreach (var node in violation.Nodes)
            {
                Console.WriteLine(
                    $"  Target: {node.Target}");

                Console.WriteLine(
                    $"  HTML: {node.Html ?? "not available"}");
            }
        }
    }

    /// <summary>
    /// Locates the root of the ZigWheels Capstone repository.
    /// </summary>
    private static string FindRepositoryRoot()
    {
        var currentDirectory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory is not null)
        {
            var gitDirectory = Path.Combine(
                currentDirectory.FullName,
                ".git");

            var accessibilityDirectory = Path.Combine(
                currentDirectory.FullName,
                "accessibility");

            if (Directory.Exists(gitDirectory) ||
                Directory.Exists(accessibilityDirectory))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new DirectoryNotFoundException(
            "The ZigWheels Capstone repository root could not be located.");
    }

    /// <summary>
    /// Converts a scenario name into a safe directory name.
    /// </summary>
    private static string SanitiseFileName(string value)
    {
        var invalidCharacters =
            Path.GetInvalidFileNameChars();

        var sanitisedCharacters = value
            .Select(character =>
                invalidCharacters.Contains(character) ||
                char.IsWhiteSpace(character)
                    ? '_'
                    : character)
            .ToArray();

        return new string(sanitisedCharacters);
    }
}
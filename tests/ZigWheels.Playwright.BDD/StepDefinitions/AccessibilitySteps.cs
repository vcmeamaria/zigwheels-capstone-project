using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using ZigWheels.Framework.Core.Configuration;
using ZigWheels.Playwright.BDD.Fixtures;
using ZigWheels.Playwright.BDD.Hooks;
using ZigWheels.Playwright.BDD.Support;

namespace ZigWheels.Playwright.BDD.StepDefinitions;

/// <summary>
/// BDD step definitions for automated accessibility scanning.
///
/// Axe-core is executed against key ZigWheels pages using Playwright.
/// Accessibility violations are documented as findings rather than
/// automatically treated as failures because ZigWheels is an external
/// production website that this project does not control.
/// </summary>
[Binding]
public class AccessibilitySteps
{
    private const string AxeResultKey =
        "AccessibilityAxeResult";

    private const string AccessibilityReportPathKey =
        "AccessibilityReportPath";

    private const string AccessibilityPageNameKey =
        "AccessibilityPageName";

    private readonly ScenarioContext _scenarioContext;

    public AccessibilitySteps(
        ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(
        @"I open the ""(.*)"" page for accessibility testing")]
    public async Task GivenIOpenThePageForAccessibilityTesting(
        string pageName)
    {
        var page = GetPage();

        var relativePath = GetRelativePath(pageName);

        var baseUri = new Uri(
            TestConfiguration.BaseUrl);

        var pageUri = new Uri(
            baseUri,
            relativePath);

        _scenarioContext[AccessibilityPageNameKey] =
            pageName;

        Console.WriteLine(
            $"Opening accessibility test page: {pageName}");

        Console.WriteLine(
            $"URL: {pageUri}");

        var response = await page.GotoAsync(
            pageUri.ToString(),
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

        Assert.That(
            response,
            Is.Not.Null,
            $"No HTTP response was received when opening {pageName}.");

        Assert.That(
            response!.Ok,
            Is.True,
            $"The {pageName} page did not load successfully. " +
            $"HTTP status: {response.Status}");

        await page.WaitForTimeoutAsync(1500);

        Console.WriteLine(
            $"{pageName} loaded successfully.");
    }

    [When("I run an axe accessibility scan")]
    public async Task WhenIRunAnAxeAccessibilityScan()
    {
        var page = GetPage();

        var pageName = GetAccessibilityPageName();

        Console.WriteLine(
            $"Starting axe-core accessibility scan for: {pageName}");

        AxeResult axeResult =
            await page.RunAxe();

        Assert.That(
            axeResult,
            Is.Not.Null,
            "axe-core did not return an accessibility result.");

        _scenarioContext[AxeResultKey] =
            axeResult;

        AccessibilityReporter.WriteSummary(
            axeResult);

        var reportPath =
            await AccessibilityReporter.SaveReportAsync(
                axeResult,
                pageName);

        _scenarioContext[AccessibilityReportPathKey] =
            reportPath;

        Console.WriteLine(
            $"Accessibility report saved: {reportPath}");
    }

    [Then(
        "the accessibility scan should complete successfully")]
    public void ThenTheAccessibilityScanShouldCompleteSuccessfully()
    {
        var axeResult = GetAxeResult();

        var pageName = GetAccessibilityPageName();

        Assert.That(
            axeResult,
            Is.Not.Null,
            "No axe accessibility result was stored for the scenario.");

        Console.WriteLine(
            $"Accessibility scan completed successfully for: {pageName}");

        Console.WriteLine(
            $"Violations recorded: {axeResult.Violations.Length}");

        /*
         * We intentionally do not assert that Violations is empty.
         *
         * ZigWheels is a third-party production website.
         * The purpose of this Capstone test is to identify and document
         * accessibility findings rather than claim ownership of fixing them.
         */
    }

    [Then(
        "the accessibility findings should be saved to a report")]
    public void ThenTheAccessibilityFindingsShouldBeSavedToAReport()
    {
        if (!_scenarioContext.TryGetValue(
                AccessibilityReportPathKey,
                out string? storedReportPath) ||
            string.IsNullOrWhiteSpace(storedReportPath))
        {
            throw new InvalidOperationException(
                "The accessibility report path was not stored.");
        }

        var reportPath = storedReportPath;

        Assert.That(
            File.Exists(reportPath),
            Is.True,
            $"The accessibility report was not created: {reportPath}");

        var fileInfo =
            new FileInfo(reportPath);

        Assert.That(
            fileInfo.Length,
            Is.GreaterThan(0),
            "The accessibility report was created but is empty.");

        Console.WriteLine(
            $"Accessibility evidence verified: {reportPath}");
    }

    /// <summary>
    /// Returns the Playwright page belonging to the current scenario.
    /// </summary>
    private IPage GetPage()
    {
        if (!_scenarioContext.TryGetValue(
                PlaywrightHooks.FixtureKey,
                out PlaywrightFixture? fixture) ||
            fixture?.Page is null)
        {
            throw new InvalidOperationException(
                "The Playwright browser page has not been created " +
                "for this scenario.");
        }

        return fixture.Page;
    }

    /// <summary>
    /// Returns the axe result produced during the current scenario.
    /// </summary>
    private AxeResult GetAxeResult()
    {
        if (!_scenarioContext.TryGetValue(
                AxeResultKey,
                out AxeResult? axeResult) ||
            axeResult is null)
        {
            throw new InvalidOperationException(
                "No axe accessibility result is available " +
                "for this scenario.");
        }

        return axeResult;
    }

    /// <summary>
    /// Returns the readable page name belonging to the current scenario.
    /// </summary>
    private string GetAccessibilityPageName()
    {
        if (!_scenarioContext.TryGetValue(
                AccessibilityPageNameKey,
                out string? pageName) ||
            string.IsNullOrWhiteSpace(pageName))
        {
            throw new InvalidOperationException(
                "The accessibility page name was not stored.");
        }

        return pageName;
    }

    /// <summary>
    /// Maps the readable BDD page name to its ZigWheels relative URL.
    /// </summary>
    private static string GetRelativePath(
        string pageName)
    {
        return pageName.Trim() switch
        {
            "Homepage" =>
                "/",

            "Upcoming Honda Bikes" =>
                "/upcoming-honda-bikes",

            "Used Cars Chennai" =>
                "/used-car/Chennai",

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(pageName),
                    pageName,
                    "The requested accessibility page is not configured.")
        };
    }
}
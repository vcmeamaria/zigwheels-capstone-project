using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using ZigWheels.Playwright.BDD.Fixtures;
using ZigWheels.Playwright.BDD.Hooks;
using ZigWheels.Playwright.BDD.Pages;

namespace ZigWheels.Playwright.BDD.StepDefinitions;

/// <summary>
/// BDD step definitions for handling embedded iframe content
/// within ZigWheels.
/// </summary>
[Binding]
public class FrameHandlingSteps
{
    private readonly ScenarioContext _scenarioContext;

    private FrameHandlingPage? _frameHandlingPage;

    public FrameHandlingSteps(
        ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(
        "I open a ZigWheels video review page")]
    public async Task GivenIOpenAZigWheelsVideoReviewPage()
    {
        var page =
            GetPage();

        _frameHandlingPage =
            new FrameHandlingPage(page);

        await _frameHandlingPage.OpenVideoReviewPageAsync();

        Console.WriteLine(
            "ZigWheels video review page is ready for iframe testing.");
    }

    [When(
        "I access the embedded video iframe")]
    public async Task WhenIAccessTheEmbeddedVideoIframe()
    {
        var framePage =
            GetFrameHandlingPage();

        await framePage.AccessEmbeddedVideoFrameAsync();

        Console.WriteLine(
            "Embedded video frame context accessed.");
    }

    [Then(
        "the embedded video frame should be available")]
    public async Task ThenTheEmbeddedVideoFrameShouldBeAvailable()
    {
        var framePage =
            GetFrameHandlingPage();

        var frameAvailable =
            await framePage.IsEmbeddedVideoFrameAvailableAsync();

        Assert.That(
            frameAvailable,
            Is.True,
            $"The embedded video iframe was not available. " +
            $"Current URL: {framePage.CurrentUrl}");

        Console.WriteLine(
            "Embedded video iframe verified successfully.");
    }

    [Then(
        "the ZigWheels page content should remain available outside the iframe")]
    public async Task ThenTheZigWheelsPageContentShouldRemainAvailableOutsideTheIframe()
    {
        var framePage =
            GetFrameHandlingPage();

        var mainPageAvailable =
            await framePage.IsMainPageContentAvailableAsync();

        Assert.That(
            mainPageAvailable,
            Is.True,
            $"The ZigWheels page content was not available after accessing " +
            $"the embedded iframe. Current URL: {framePage.CurrentUrl}");

        Console.WriteLine(
            "Main ZigWheels page context verified after iframe access.");
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
    /// Returns the frame-handling page object created
    /// for the current scenario.
    /// </summary>
    private FrameHandlingPage GetFrameHandlingPage()
    {
        if (_frameHandlingPage is null)
        {
            throw new InvalidOperationException(
                "FrameHandlingPage has not been initialised. " +
                "The Given step must execute first.");
        }

        return _frameHandlingPage;
    }
}
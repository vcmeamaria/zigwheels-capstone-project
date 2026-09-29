using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using ZigWheels.Playwright.BDD.Fixtures;
using ZigWheels.Playwright.BDD.Hooks;
using ZigWheels.Playwright.BDD.Pages;

namespace ZigWheels.Playwright.BDD.StepDefinitions;

/// <summary>
/// BDD step definitions for ZigWheels menu navigation
/// and browser-back navigation.
/// </summary>
[Binding]
public class NavigationAndBackSteps
{
    private readonly ScenarioContext _scenarioContext;

    private NavigationPage? _navigationPage;

    public NavigationAndBackSteps(
        ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(
        "I am on the ZigWheels homepage for navigation testing")]
    public async Task GivenIAmOnTheZigWheelsHomepageForNavigationTesting()
    {
        var page = GetPage();

        _navigationPage =
            new NavigationPage(page);

        await _navigationPage.OpenHomepageAsync();

        Assert.That(
            _navigationPage.IsHomepageDisplayed(),
            Is.True,
            $"The ZigWheels homepage was not displayed. " +
            $"Current URL: {_navigationPage.CurrentUrl}");

        Console.WriteLine(
            "Navigation scenario started on the ZigWheels homepage.");
    }

    [When(
        "I open the New Bikes menu")]
    public async Task WhenIOpenTheNewBikesMenu()
    {
        var navigationPage =
            GetNavigationPage();

        await navigationPage.OpenNewBikesMenuAsync();

        Console.WriteLine(
            "New Bikes menu is available.");
    }

    [When(
        "I select the Upcoming Bikes collection")]
    public async Task WhenISelectTheUpcomingBikesCollection()
    {
        var navigationPage =
            GetNavigationPage();

        await navigationPage.SelectUpcomingBikesAsync();

        Console.WriteLine(
            "Upcoming Bikes collection selected.");
    }

    [Then(
        "the Upcoming Bikes collection page should be displayed")]
    public async Task ThenTheUpcomingBikesCollectionPageShouldBeDisplayed()
    {
        var navigationPage =
            GetNavigationPage();

        var displayed =
            await navigationPage.IsUpcomingBikesPageDisplayedAsync();

        Assert.That(
            displayed,
            Is.True,
            $"The Upcoming Bikes collection page was not displayed. " +
            $"Current URL: {navigationPage.CurrentUrl}");

        Console.WriteLine(
            $"Upcoming Bikes collection verified: " +
            $"{navigationPage.CurrentUrl}");
    }

    [When(
        "I navigate back to the previous page")]
    public async Task WhenINavigateBackToThePreviousPage()
    {
        var navigationPage =
            GetNavigationPage();

        await navigationPage.NavigateBackAsync();
    }

    [Then(
        "I should return to the ZigWheels homepage")]
    public void ThenIShouldReturnToTheZigWheelsHomepage()
    {
        var navigationPage =
            GetNavigationPage();

        Assert.That(
            navigationPage.IsHomepageDisplayed(),
            Is.True,
            $"Browser back navigation did not return to the " +
            $"ZigWheels homepage. Current URL: " +
            $"{navigationPage.CurrentUrl}");

        Console.WriteLine(
            "Browser back navigation successfully returned " +
            "to the ZigWheels homepage.");
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
    /// Returns the navigation page object created for this scenario.
    /// </summary>
    private NavigationPage GetNavigationPage()
    {
        if (_navigationPage is null)
        {
            throw new InvalidOperationException(
                "The NavigationPage has not been initialised. " +
                "The homepage Given step must run first.");
        }

        return _navigationPage;
    }
}
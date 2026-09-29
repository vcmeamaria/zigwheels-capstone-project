using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using ZigWheels.Playwright.BDD.Fixtures;
using ZigWheels.Playwright.BDD.Hooks;
using ZigWheels.Playwright.BDD.Pages;

namespace ZigWheels.Playwright.BDD.StepDefinitions;

/// <summary>
/// BDD step definitions for ZigWheels form validation testing.
/// </summary>
[Binding]
public class FormValidationSteps
{
    private readonly ScenarioContext _scenarioContext;

    private FormValidationPage? _formValidationPage;

    public FormValidationSteps(
        ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(
        "I am on the ZigWheels on-road price form")]
    public async Task GivenIAmOnTheZigWheelsOnRoadPriceForm()
    {
        var page =
            GetPage();

        _formValidationPage =
            new FormValidationPage(page);

        await _formValidationPage.OpenAsync();

        Console.WriteLine(
            "ZigWheels On-Road Price form is ready for validation testing.");
    }

    [When(
        "I submit the on-road price form without selecting a vehicle make")]
    public async Task WhenISubmitTheOnRoadPriceFormWithoutSelectingAVehicleMake()
    {
        var formPage =
            GetFormValidationPage();

        await formPage.SubmitWithoutSelectingMakeAsync();
    }

    [Then(
        "the vehicle make validation warning should be displayed")]
    public async Task ThenTheVehicleMakeValidationWarningShouldBeDisplayed()
    {
        var formPage =
            GetFormValidationPage();

        var warningDisplayed =
            await formPage.IsMakeValidationWarningDisplayedAsync();

        Assert.That(
            warningDisplayed,
            Is.True,
            $"The expected vehicle make validation warning was not displayed. " +
            $"Current URL: {formPage.CurrentUrl}");

        var warningText =
            await formPage.GetMakeValidationWarningTextAsync();

        Assert.That(
            warningText,
            Is.EqualTo("Please select make"),
            "The vehicle make validation warning text was not as expected.");

        Console.WriteLine(
            $"Validation warning verified: {warningText}");
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
    /// Returns the form page object created for the current scenario.
    /// </summary>
    private FormValidationPage GetFormValidationPage()
    {
        if (_formValidationPage is null)
        {
            throw new InvalidOperationException(
                "FormValidationPage has not been initialised. " +
                "The Given step must execute first.");
        }

        return _formValidationPage;
    }
}
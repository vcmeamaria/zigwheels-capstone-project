using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using ZigWheels.Framework.Core.Models;
using ZigWheels.Playwright.BDD.Fixtures;
using ZigWheels.Playwright.BDD.Hooks;
using ZigWheels.Playwright.BDD.Pages;

namespace ZigWheels.Playwright.BDD.StepDefinitions;

/// <summary>
/// BDD step definitions for the Popular Used Cars in Chennai scenario.
///
/// Playwright-specific browser interaction remains inside the Page Object,
/// while this class coordinates business behaviour, test data and assertions.
/// </summary>
[Binding]
public class UsedCarsChennaiSteps
{
    private const string PopularModelsKey = "PopularUsedCarModels";

    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _outputHelper;

    public UsedCarsChennaiSteps(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        _scenarioContext = scenarioContext;
        _outputHelper = outputHelper;
    }

    [Given("I am on the ZigWheels used cars page for Chennai")]
    public async Task GivenIAmOnTheZigWheelsUsedCarsPageForChennai()
    {
        var page = new UsedCarsChennaiPage(GetPage());

        await page.OpenAsync();

        Assert.That(
            await page.IsLoadedAsync(),
            Is.True,
            "The ZigWheels Used Cars in Chennai page did not load successfully.");
    }

    [When("I collect the popular used-car models")]
    public async Task WhenICollectThePopularUsedCarModels()
    {
        var page = new UsedCarsChennaiPage(GetPage());

        var popularModels = await page.GetPopularModelsAsync();

        _scenarioContext[PopularModelsKey] =
            popularModels.ToList();

        _outputHelper.WriteLine(
            $"Popular used-car models extracted: {popularModels.Count}");
    }

    [Then("the popular used-car model collection should not be empty")]
    public void ThenThePopularUsedCarModelCollectionShouldNotBeEmpty()
    {
        var models = GetPopularModels();

        Assert.That(
            models,
            Is.Not.Empty,
            "No popular used-car models were extracted for Chennai.");
    }

    [Then("each collected model should have a name")]
    public void ThenEachCollectedModelShouldHaveAName()
    {
        var models = GetPopularModels();

        Assert.Multiple(() =>
        {
            foreach (var model in models)
            {
                Assert.That(
                    model.Name,
                    Is.Not.Null.And.Not.Empty,
                    "An extracted used-car model did not contain a name.");
            }
        });
    }

    [Then("the collected models should be displayed in the test output")]
    public void ThenTheCollectedModelsShouldBeDisplayedInTheTestOutput()
    {
        var models = GetPopularModels();

        _outputHelper.WriteLine("Popular Used Cars in Chennai:");

        foreach (var model in models)
        {
            _outputHelper.WriteLine(
                $"Model: {model.Name} | " +
                $"Inventory: {model.InventoryCount}");
        }

        Assert.That(
            models,
            Is.Not.Empty,
            "There were no collected models available to display.");
    }

    private IPage GetPage()
    {
        if (!_scenarioContext.TryGetValue(
                PlaywrightHooks.FixtureKey,
                out PlaywrightFixture? fixture) ||
            fixture?.Page is null)
        {
            throw new InvalidOperationException(
                "The Playwright page has not been created for this scenario.");
        }

        return fixture.Page;
    }

    private IReadOnlyList<UsedCarModel> GetPopularModels()
    {
        if (!_scenarioContext.TryGetValue(
                PopularModelsKey,
                out List<UsedCarModel>? models) ||
            models is null)
        {
            throw new InvalidOperationException(
                "The popular used-car model collection has not been created.");
        }

        return models;
    }
}
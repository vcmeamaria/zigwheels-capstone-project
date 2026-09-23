using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using ZigWheels.Framework.Core.Models;
using ZigWheels.Selenium.BDD.Hooks;
using ZigWheels.Selenium.BDD.Pages;

namespace ZigWheels.Selenium.BDD.StepDefinitions;

/// <summary>
/// BDD step definitions for the Upcoming Honda Bikes business scenario.
///
/// Browser interaction remains inside Page Objects while this class
/// coordinates business behaviour, collected test data and assertions.
/// </summary>
[Binding]
public class UpcomingHondaBikesSteps
{
    private const string FilteredBikesKey = "FilteredHondaBikes";

    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _outputHelper;

    public UpcomingHondaBikesSteps(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        _scenarioContext = scenarioContext;
        _outputHelper = outputHelper;
    }

    [Given("I am on the ZigWheels homepage")]
    public void GivenIAmOnTheZigWheelsHomepage()
    {
        var homePage = new HomePage(GetDriver());

        homePage.Open();

        Assert.That(
            homePage.IsLoaded(),
            Is.True,
            "The ZigWheels homepage did not load successfully.");
    }

    [When("I navigate to the upcoming Honda bikes page")]
    public void WhenINavigateToTheUpcomingHondaBikesPage()
    {
        var upcomingHondaBikesPage =
            new UpcomingHondaBikesPage(GetDriver());

        upcomingHondaBikesPage.Open();

        Assert.That(
            upcomingHondaBikesPage.IsLoaded(),
            Is.True,
            "The Upcoming Honda Bikes page did not load successfully.");
    }

    [When("I collect the upcoming Honda bikes priced below four lakh")]
    public void WhenICollectTheUpcomingHondaBikesPricedBelowFourLakh()
    {
        var upcomingHondaBikesPage =
            new UpcomingHondaBikesPage(GetDriver());

        var displayedBikes =
            upcomingHondaBikesPage.GetDisplayedBikes();

        Assert.That(
            displayedBikes,
            Is.Not.Empty,
            "No upcoming Honda bikes were extracted from ZigWheels.");

        var filteredBikes = displayedBikes
            .Where(bike =>
                bike.PriceInRupees > 0 &&
                bike.PriceInRupees < 400000)
            .ToList();

        _scenarioContext[FilteredBikesKey] = filteredBikes;

        _outputHelper.WriteLine(
            $"Total upcoming Honda bikes extracted: {displayedBikes.Count}");

        _outputHelper.WriteLine(
            $"Honda bikes below four lakh: {filteredBikes.Count}");

        _outputHelper.WriteLine(
            "Filtered bike details:");

        foreach (var bike in filteredBikes)
        {
            _outputHelper.WriteLine(
                $"Bike: {bike.Name} | " +
                $"Price: {bike.DisplayedPrice} | " +
                $"Launch: {bike.ExpectedLaunchDate}");
        }
    }

    [Then("the filtered bike results should not be empty")]
    public void ThenTheFilteredBikeResultsShouldNotBeEmpty()
    {
        var bikes = GetFilteredBikes();

        Assert.That(
            bikes,
            Is.Not.Empty,
            "No upcoming Honda bikes below four lakh were found.");
    }

    [Then("every filtered bike should be manufactured by Honda")]
    public void ThenEveryFilteredBikeShouldBeManufacturedByHonda()
    {
        var bikes = GetFilteredBikes();

        Assert.Multiple(() =>
        {
            foreach (var bike in bikes)
            {
                Assert.That(
                    bike.Name,
                    Does.StartWith("Honda ").IgnoreCase,
                    $"Bike '{bike.Name}' was not identified as a Honda model.");
            }
        });
    }

    [Then("every filtered bike should cost less than four lakh")]
    public void ThenEveryFilteredBikeShouldCostLessThanFourLakh()
    {
        var bikes = GetFilteredBikes();

        Assert.Multiple(() =>
        {
            foreach (var bike in bikes)
            {
                Assert.That(
                    bike.PriceInRupees,
                    Is.GreaterThan(0),
                    $"Bike '{bike.Name}' did not contain a valid numeric price.");

                Assert.That(
                    bike.PriceInRupees,
                    Is.LessThan(400000),
                    $"Bike '{bike.Name}' costs {bike.DisplayedPrice}, " +
                    "which is not below four lakh.");
            }
        });
    }

    [Then("every filtered bike should display its name")]
    public void ThenEveryFilteredBikeShouldDisplayItsName()
    {
        var bikes = GetFilteredBikes();

        Assert.Multiple(() =>
        {
            foreach (var bike in bikes)
            {
                Assert.That(
                    bike.Name,
                    Is.Not.Null.And.Not.Empty,
                    "An extracted bike did not contain a name.");
            }
        });
    }

    [Then("every filtered bike should display its price")]
    public void ThenEveryFilteredBikeShouldDisplayItsPrice()
    {
        var bikes = GetFilteredBikes();

        Assert.Multiple(() =>
        {
            foreach (var bike in bikes)
            {
                Assert.That(
                    bike.DisplayedPrice,
                    Is.Not.Null.And.Not.Empty,
                    $"Bike '{bike.Name}' did not display a price.");

                Assert.That(
                    bike.PriceInRupees,
                    Is.GreaterThan(0),
                    $"Bike '{bike.Name}' displayed a price that could not be parsed.");
            }
        });
    }

    [Then("every filtered bike should display its expected launch date")]
    public void ThenEveryFilteredBikeShouldDisplayItsExpectedLaunchDate()
    {
        var bikes = GetFilteredBikes();

        Assert.Multiple(() =>
        {
            foreach (var bike in bikes)
            {
                Assert.That(
                    bike.ExpectedLaunchDate,
                    Is.Not.Null.And.Not.Empty,
                    $"Bike '{bike.Name}' did not display an expected launch date.");
            }
        });
    }

    private IWebDriver GetDriver()
    {
        if (!_scenarioContext.TryGetValue(
                SeleniumHooks.DriverKey,
                out IWebDriver? driver) ||
            driver is null)
        {
            throw new InvalidOperationException(
                "The Selenium WebDriver has not been created for this scenario.");
        }

        return driver;
    }

    private IReadOnlyList<BikeDetails> GetFilteredBikes()
    {
        if (!_scenarioContext.TryGetValue(
                FilteredBikesKey,
                out List<BikeDetails>? bikes) ||
            bikes is null)
        {
            throw new InvalidOperationException(
                "The filtered Honda bike collection has not been created.");
        }

        return bikes;
    }
}
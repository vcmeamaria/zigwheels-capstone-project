using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ZigWheels.Framework.Core.Configuration;
using ZigWheels.Framework.Core.Models;
using ZigWheels.Framework.Core.Utilities;

namespace ZigWheels.Selenium.BDD.Pages;

/// <summary>
/// Represents the ZigWheels Upcoming Honda Bikes page.
///
/// The page object extracts the bike information displayed by the website
/// and converts it into reusable BikeDetails models for assertions and
/// filtering within the BDD scenario.
/// </summary>
public class UpcomingHondaBikesPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    private const string RelativeUrl = "upcoming-honda-bikes";

    public UpcomingHondaBikesPage(IWebDriver driver)
    {
        _driver = driver;

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(15));
    }

    /// <summary>
    /// Navigates directly to the Upcoming Honda Bikes page.
    /// </summary>
    public void Open()
    {
        var targetUrl = new Uri(
            new Uri(TestConfiguration.BaseUrl),
            RelativeUrl);

        _driver.Navigate().GoToUrl(targetUrl);
    }

    /// <summary>
    /// Confirms that the Upcoming Honda Bikes page has loaded.
    /// </summary>
    public bool IsLoaded()
    {
        try
        {
            return _wait.Until(driver =>
                driver.FindElements(
                        By.XPath(
                            "//h1[contains(normalize-space(.), " +
                            "'Upcoming Honda Bikes')]"))
                    .Count > 0);
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Extracts the upcoming Honda bikes currently displayed by ZigWheels.
    /// </summary>
    public IReadOnlyList<BikeDetails> GetDisplayedBikes()
    {
        WaitForBikeResults();

        var launchElements = _driver.FindElements(
            By.XPath(
                "//*[contains(normalize-space(.), 'Expected Launch') " +
                "and not(*)]"));

        var bikes = new List<BikeDetails>();

        foreach (var launchElement in launchElements)
        {
            var bike = TryExtractBike(launchElement);

            if (bike is not null &&
                bikes.All(existingBike =>
                    !existingBike.Name.Equals(
                        bike.Name,
                        StringComparison.OrdinalIgnoreCase)))
            {
                bikes.Add(bike);
            }
        }

        return bikes;
    }

    private void WaitForBikeResults()
    {
        _wait.Until(driver =>
            driver.FindElements(
                    By.XPath(
                        "//*[contains(normalize-space(.), " +
                        "'Expected Launch') and not(*)]"))
                .Count > 0);
    }

    private static BikeDetails? TryExtractBike(
        IWebElement launchElement)
    {
        IWebElement card;

        try
        {
            card = launchElement.FindElement(
                By.XPath(
                    "./ancestor::*[" +
                    "(self::div or self::li) " +
                    "and .//a[starts-with(normalize-space(.), 'Honda ')]" +
                    "][1]"));
        }
        catch (NoSuchElementException)
        {
            return null;
        }

        var lines = card.Text
            .Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        var name = lines.FirstOrDefault(line =>
            line.StartsWith(
                "Honda ",
                StringComparison.OrdinalIgnoreCase));

        var displayedPrice = lines.FirstOrDefault(line =>
            line.StartsWith(
                "Rs.",
                StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith(
                "₹",
                StringComparison.OrdinalIgnoreCase));

        var expectedLaunchDate = lines.FirstOrDefault(line =>
            line.Contains(
                "Expected Launch",
                StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(displayedPrice) ||
            string.IsNullOrWhiteSpace(expectedLaunchDate))
        {
            return null;
        }

        var cleanedLaunchDate = expectedLaunchDate;

        var separatorIndex = expectedLaunchDate.IndexOf(':');

        if (separatorIndex >= 0 &&
            separatorIndex < expectedLaunchDate.Length - 1)
        {
            cleanedLaunchDate = expectedLaunchDate[
                (separatorIndex + 1)..].Trim();
        }

        return new BikeDetails
        {
            Name = name,
            Manufacturer = "Honda",
            DisplayedPrice = displayedPrice,
            PriceInRupees =
                PriceParser.ParseToRupees(displayedPrice),
            ExpectedLaunchDate = cleanedLaunchDate
        };
    }
}
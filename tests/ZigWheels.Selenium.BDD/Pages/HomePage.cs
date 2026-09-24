using OpenQA.Selenium;
using ZigWheels.Framework.Core.Configuration;
using ZigWheels.Selenium.BDD.Components;

namespace ZigWheels.Selenium.BDD.Pages;

/// <summary>
/// Represents the ZigWheels homepage.
///
/// Page Objects contain browser interaction details so that BDD step
/// definitions remain focused on business behaviour rather than Selenium.
/// </summary>
public class HomePage
{
    private readonly IWebDriver _driver;

    public HomePage(IWebDriver driver)
    {
        _driver = driver;
    }

    /// <summary>
    /// Opens the configured ZigWheels homepage and handles the
    /// privacy/cookie consent dialog when it appears.
    /// </summary>
    public void Open()
    {
        _driver.Navigate().GoToUrl(
            TestConfiguration.BaseUrl);

        var cookieConsent =
            new CookieConsentComponent(_driver);

        cookieConsent.AcceptIfPresent();
    }

    /// <summary>
    /// Confirms that the browser is currently on the ZigWheels domain.
    /// </summary>
    public bool IsLoaded()
    {
        if (!Uri.TryCreate(
                _driver.Url,
                UriKind.Absolute,
                out var currentUri))
        {
            return false;
        }

        return currentUri.Host.Contains(
            "zigwheels.com",
            StringComparison.OrdinalIgnoreCase);
    }
}
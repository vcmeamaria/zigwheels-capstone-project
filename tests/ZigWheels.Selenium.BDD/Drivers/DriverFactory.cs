using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using ZigWheels.Framework.Core.Configuration;

namespace ZigWheels.Selenium.BDD.Drivers;

/// <summary>
/// Creates independent Selenium WebDriver instances for test scenarios.
///
/// The factory supports both local browser execution and Selenium Grid.
/// No shared static WebDriver instance is stored here, which helps keep
/// scenarios isolated and safe for parallel execution.
/// </summary>
public static class DriverFactory
{
    public static IWebDriver CreateDriver()
    {
        var browser = TestConfiguration.Browser
            .Trim()
            .ToLowerInvariant();

        var options = CreateBrowserOptions(browser);

        IWebDriver driver = TestConfiguration.UseSeleniumGrid
            ? CreateRemoteDriver(options)
            : CreateLocalDriver(browser, options);

        driver.Manage().Window.Maximize();

        return driver;
    }

    private static DriverOptions CreateBrowserOptions(string browser)
    {
        return browser switch
        {
            "chrome" => CreateChromeOptions(),
            "firefox" => CreateFirefoxOptions(),

            _ => throw new NotSupportedException(
                $"Browser '{browser}' is not supported. " +
                "Supported browsers are: chrome and firefox.")
        };
    }

    private static IWebDriver CreateLocalDriver(
        string browser,
        DriverOptions options)
    {
        return browser switch
        {
            "chrome" => new ChromeDriver((ChromeOptions)options),
            "firefox" => new FirefoxDriver((FirefoxOptions)options),

            _ => throw new NotSupportedException(
                $"Browser '{browser}' is not supported.")
        };
    }

    private static IWebDriver CreateRemoteDriver(DriverOptions options)
    {
        var gridUri = new Uri(TestConfiguration.SeleniumGridUrl);

        return new RemoteWebDriver(
            gridUri,
            options);
    }

    private static ChromeOptions CreateChromeOptions()
    {
        var options = new ChromeOptions();

        if (TestConfiguration.Headless)
        {
            options.AddArgument("--headless=new");
        }

        options.AddArgument("--disable-notifications");
        options.AddArgument("--start-maximized");

        return options;
    }

    private static FirefoxOptions CreateFirefoxOptions()
    {
        var options = new FirefoxOptions();

        if (TestConfiguration.Headless)
        {
            options.AddArgument("-headless");
        }

        return options;
    }
}
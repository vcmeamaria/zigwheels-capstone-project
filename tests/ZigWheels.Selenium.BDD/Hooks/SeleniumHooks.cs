using OpenQA.Selenium;
using Reqnroll;
using ZigWheels.Selenium.BDD.Drivers;

namespace ZigWheels.Selenium.BDD.Hooks;

/// <summary>
/// Manages the Selenium browser lifecycle for each BDD scenario.
///
/// A new WebDriver instance is created before every scenario and disposed
/// afterwards. This prevents scenarios from sharing browser state and
/// prepares the framework for safe parallel execution.
/// </summary>
[Binding]
public class SeleniumHooks
{
    public const string DriverKey = "WebDriver";

    private readonly ScenarioContext _scenarioContext;

    public SeleniumHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario(Order = 0)]
    public void StartBrowser()
    {
        var driver = DriverFactory.CreateDriver();

        _scenarioContext[DriverKey] = driver;
    }

    [AfterScenario(Order = 100)]
    public void CloseBrowser()
    {
        if (!_scenarioContext.TryGetValue(
                DriverKey,
                out IWebDriver? driver) ||
            driver is null)
        {
            return;
        }

        try
        {
            driver.Quit();
        }
        finally
        {
            driver.Dispose();
        }
    }
}
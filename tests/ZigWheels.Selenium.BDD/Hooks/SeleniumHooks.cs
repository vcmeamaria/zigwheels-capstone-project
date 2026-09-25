using Allure.Net.Commons;
using OpenQA.Selenium;
using Reqnroll;
using ZigWheels.Framework.Core.Utilities;
using ZigWheels.Selenium.BDD.Drivers;

namespace ZigWheels.Selenium.BDD.Hooks;

/// <summary>
/// Controls the Selenium browser lifecycle for each BDD scenario.
///
/// Each scenario receives:
/// - its own WebDriver
/// - its own log file
/// - a failure screenshot when required
///
/// Failure screenshots are also attached to Allure.
/// </summary>
[Binding]
public class SeleniumHooks
{
    public const string DriverKey =
        "SeleniumDriver";

    private const string LoggerKey =
        "SeleniumScenarioLogger";

    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _outputHelper;

    public SeleniumHooks(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        _scenarioContext =
            scenarioContext;

        _outputHelper =
            outputHelper;
    }

    /// <summary>
    /// Creates the scenario-specific Selenium log before
    /// any browser activity begins.
    /// </summary>
    [BeforeScenario(Order = -100)]
    public void StartScenarioLogging()
    {
        var scenarioName =
            _scenarioContext.ScenarioInfo.Title;

        var logger =
            new ScenarioLogger(
                "Selenium",
                scenarioName);

        _scenarioContext[LoggerKey] =
            logger;

        logger.Info(
            $"Scenario started: {scenarioName}");

        _outputHelper.WriteLine(
            $"Selenium scenario log: {logger.FilePath}");
    }

    /// <summary>
    /// Creates a fresh Selenium WebDriver before each scenario.
    /// </summary>
    [BeforeScenario(Order = 0)]
    public void StartBrowser()
    {
        var logger =
            GetLogger();

        logger.Info(
            "Creating Selenium WebDriver.");

        try
        {
            var driver =
                DriverFactory.CreateDriver();

            _scenarioContext[DriverKey] =
                driver;

            logger.Info(
                "Selenium browser started successfully.");
        }
        catch (Exception exception)
        {
            logger.Error(
                "Unable to start Selenium browser.",
                exception);

            throw;
        }
    }

    /// <summary>
    /// Captures a browser screenshot when a Selenium scenario fails
    /// and attaches the screenshot to the Allure test result.
    /// </summary>
    [AfterScenario(Order = 90)]
    public void CaptureFailureScreenshot()
    {
        if (_scenarioContext.TestError is null)
        {
            return;
        }

        var logger =
            GetLogger();

        logger.Warning(
            "Scenario failed. Attempting to capture a failure screenshot.");

        if (!_scenarioContext.TryGetValue(
                DriverKey,
                out IWebDriver? driver) ||
            driver is null)
        {
            logger.Warning(
                "Failure screenshot skipped because the WebDriver is unavailable.");

            return;
        }

        if (driver is not ITakesScreenshot screenshotDriver)
        {
            logger.Warning(
                "Failure screenshot skipped because the current WebDriver " +
                "does not support screenshots.");

            _outputHelper.WriteLine(
                "Screenshot skipped because the current " +
                "WebDriver does not support screenshots.");

            return;
        }

        try
        {
            var scenarioName =
                _scenarioContext.ScenarioInfo.Title;

            var screenshotPath =
                ArtifactPaths.CreateScreenshotPath(
                    "Selenium",
                    scenarioName);

            var screenshot =
                screenshotDriver.GetScreenshot();

            screenshot.SaveAsFile(
                screenshotPath);

            logger.Info(
                $"Failure screenshot saved: {screenshotPath}");

            _outputHelper.WriteLine(
                $"Failure screenshot saved: {screenshotPath}");

            try
            {
                AllureApi.AddAttachment(
                    "Selenium failure screenshot",
                    "image/png",
                    screenshotPath);

                logger.Info(
                    "Failure screenshot attached to Allure.");

                _outputHelper.WriteLine(
                    "Selenium failure screenshot attached to Allure.");
            }
            catch (Exception exception)
            {
                logger.Error(
                    "Unable to attach Selenium screenshot to Allure.",
                    exception);

                _outputHelper.WriteLine(
                    "Unable to attach Selenium screenshot to Allure.");

                _outputHelper.WriteLine(
                    $"Allure attachment error: {exception.Message}");
            }
        }
        catch (Exception exception)
        {
            /*
             * Evidence collection must never replace the original
             * scenario failure.
             */
            logger.Error(
                "Unable to capture Selenium failure screenshot.",
                exception);

            _outputHelper.WriteLine(
                "Unable to capture Selenium failure screenshot.");

            _outputHelper.WriteLine(
                $"Screenshot error: {exception.Message}");
        }
    }

    /// <summary>
    /// Closes and disposes the Selenium browser after each scenario.
    ///
    /// Order 100 ensures failure screenshots are collected first.
    /// </summary>
    [AfterScenario(Order = 100)]
    public void CloseBrowser()
    {
        var logger =
            GetLogger();

        if (!_scenarioContext.TryGetValue(
                DriverKey,
                out IWebDriver? driver) ||
            driver is null)
        {
            logger.Warning(
                "Selenium browser cleanup skipped because no WebDriver was available.");

            return;
        }

        try
        {
            logger.Info(
                "Closing Selenium browser.");

            driver.Quit();

            logger.Info(
                "Selenium browser closed successfully.");
        }
        catch (Exception exception)
        {
            logger.Error(
                "An error occurred while closing the Selenium browser.",
                exception);
        }
        finally
        {
            driver.Dispose();

            logger.Info(
                "Selenium WebDriver disposed.");
        }
    }

    /// <summary>
    /// Records the final scenario result and closes the log file.
    ///
    /// Order 110 ensures all screenshot and browser lifecycle
    /// events have already been written.
    /// </summary>
    [AfterScenario(Order = 110)]
    public void FinishScenarioLogging()
    {
        if (!_scenarioContext.TryGetValue(
                LoggerKey,
                out ScenarioLogger? logger) ||
            logger is null)
        {
            return;
        }

        try
        {
            if (_scenarioContext.TestError is null)
            {
                logger.Info(
                    "Scenario completed successfully.");
            }
            else
            {
                logger.Error(
                    "Scenario failed.",
                    _scenarioContext.TestError);
            }

            logger.Info(
                "Scenario execution finished.");
        }
        finally
        {
            logger.Dispose();
        }
    }

    /// <summary>
    /// Returns the logger created for the current Selenium scenario.
    /// </summary>
    private ScenarioLogger GetLogger()
    {
        if (!_scenarioContext.TryGetValue(
                LoggerKey,
                out ScenarioLogger? logger) ||
            logger is null)
        {
            throw new InvalidOperationException(
                "The Selenium scenario logger has not been created.");
        }

        return logger;
    }
}
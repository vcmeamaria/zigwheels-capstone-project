using Allure.Net.Commons;
using Reqnroll;
using ZigWheels.Framework.Core.Utilities;
using ZigWheels.Playwright.BDD.Fixtures;

namespace ZigWheels.Playwright.BDD.Hooks;

/// <summary>
/// Manages the Playwright browser lifecycle for each BDD scenario.
///
/// Every scenario receives:
/// - its own PlaywrightFixture
/// - its own BrowserContext and Page
/// - its own scenario log
/// - a failure screenshot and trace when required
///
/// Failure artefacts are also attached to Allure.
/// </summary>
[Binding]
public class PlaywrightHooks
{
    public const string FixtureKey =
        "PlaywrightFixture";

    private const string LoggerKey =
        "PlaywrightScenarioLogger";

    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _outputHelper;

    public PlaywrightHooks(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        _scenarioContext =
            scenarioContext;

        _outputHelper =
            outputHelper;
    }

    /// <summary>
    /// Creates the scenario-specific Playwright log before
    /// any browser activity begins.
    /// </summary>
    [BeforeScenario(Order = -100)]
    public void StartScenarioLogging()
    {
        var scenarioName =
            _scenarioContext.ScenarioInfo.Title;

        var logger =
            new ScenarioLogger(
                "Playwright",
                scenarioName);

        _scenarioContext[LoggerKey] =
            logger;

        logger.Info(
            $"Scenario started: {scenarioName}");

        _outputHelper.WriteLine(
            $"Playwright scenario log: {logger.FilePath}");
    }

    /// <summary>
    /// Starts an isolated Playwright browser session
    /// before each BDD scenario.
    /// </summary>
    [BeforeScenario(Order = 0)]
    public async Task StartPlaywrightAsync()
    {
        var logger =
            GetLogger();

        logger.Info(
            "Creating Playwright browser session.");

        try
        {
            var fixture =
                new PlaywrightFixture();

            await fixture.StartAsync();

            _scenarioContext[FixtureKey] =
                fixture;

            logger.Info(
                "Playwright browser session started successfully.");

            logger.Info(
                "Playwright tracing started.");
        }
        catch (Exception exception)
        {
            logger.Error(
                "Unable to start Playwright browser session.",
                exception);

            throw;
        }
    }

    /// <summary>
    /// Captures a screenshot and Playwright trace when
    /// the current scenario fails, then attaches both
    /// artefacts to the Allure result.
    ///
    /// Order 90 ensures evidence is collected while the
    /// page and browser context are still available.
    /// </summary>
    [AfterScenario(Order = 90)]
    public async Task CaptureFailureArtifactsAsync()
    {
        if (_scenarioContext.TestError is null)
        {
            return;
        }

        var logger =
            GetLogger();

        logger.Warning(
            "Scenario failed. Attempting to capture Playwright artefacts.");

        if (!_scenarioContext.TryGetValue(
                FixtureKey,
                out PlaywrightFixture? fixture) ||
            fixture is null)
        {
            logger.Warning(
                "Failure artefact capture skipped because " +
                "the Playwright fixture is unavailable.");

            return;
        }

        try
        {
            var scenarioName =
                _scenarioContext.ScenarioInfo.Title;

            var artifacts =
                await fixture
                    .CaptureFailureArtifactsAsync(
                        scenarioName);

            logger.Info(
                $"Failure screenshot saved: " +
                $"{artifacts.ScreenshotPath}");

            logger.Info(
                $"Playwright trace saved: " +
                $"{artifacts.TracePath}");

            _outputHelper.WriteLine(
                $"Playwright failure screenshot saved: " +
                $"{artifacts.ScreenshotPath}");

            _outputHelper.WriteLine(
                $"Playwright trace saved: " +
                $"{artifacts.TracePath}");

            try
            {
                AllureApi.AddAttachment(
                    "Playwright failure screenshot",
                    "image/png",
                    artifacts.ScreenshotPath);

                AllureApi.AddAttachment(
                    "Playwright trace",
                    "application/zip",
                    artifacts.TracePath);

                logger.Info(
                    "Playwright failure artefacts attached to Allure.");

                _outputHelper.WriteLine(
                    "Playwright failure artefacts attached to Allure.");
            }
            catch (Exception exception)
            {
                logger.Error(
                    "Unable to attach Playwright artefacts to Allure.",
                    exception);

                _outputHelper.WriteLine(
                    "Unable to attach Playwright artefacts to Allure.");

                _outputHelper.WriteLine(
                    $"Allure attachment error: {exception.Message}");
            }
        }
        catch (Exception exception)
        {
            /*
             * Artefact collection must never replace or hide
             * the original scenario failure.
             */
            logger.Error(
                "Unable to capture Playwright failure artefacts.",
                exception);

            _outputHelper.WriteLine(
                "Unable to capture Playwright failure artefacts.");

            _outputHelper.WriteLine(
                $"Artefact capture error: {exception.Message}");
        }
    }

    /// <summary>
    /// Stops Playwright after each scenario.
    ///
    /// Order 100 ensures failure artefacts are captured
    /// before the browser context is closed.
    /// </summary>
    [AfterScenario(Order = 100)]
    public async Task StopPlaywrightAsync()
    {
        var logger =
            GetLogger();

        if (!_scenarioContext.TryGetValue(
                FixtureKey,
                out PlaywrightFixture? fixture) ||
            fixture is null)
        {
            logger.Warning(
                "Playwright cleanup skipped because " +
                "the fixture is unavailable.");

            return;
        }

        try
        {
            logger.Info(
                "Closing Playwright browser session.");

            await fixture.StopAsync();

            logger.Info(
                "Playwright browser session closed successfully.");
        }
        catch (Exception exception)
        {
            logger.Error(
                "An error occurred while closing Playwright.",
                exception);
        }
    }

    /// <summary>
    /// Records the final scenario result and closes the log file.
    ///
    /// Order 110 ensures browser cleanup and failure artefact
    /// collection have already been logged.
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
    /// Returns the logger created for the current Playwright scenario.
    /// </summary>
    private ScenarioLogger GetLogger()
    {
        if (!_scenarioContext.TryGetValue(
                LoggerKey,
                out ScenarioLogger? logger) ||
            logger is null)
        {
            throw new InvalidOperationException(
                "The Playwright scenario logger has not been created.");
        }

        return logger;
    }
}
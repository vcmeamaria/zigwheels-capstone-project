using Microsoft.Playwright;
using ZigWheels.Framework.Core.Configuration;
using ZigWheels.Framework.Core.Utilities;

namespace ZigWheels.Playwright.BDD.Fixtures;

/// <summary>
/// Creates and manages an isolated Playwright browser session.
///
/// Each scenario receives its own BrowserContext and Page so that
/// cookies, storage and browser state are not shared between scenarios.
/// This keeps the framework suitable for parallel execution.
///
/// Playwright tracing is started for every scenario. Passing scenarios
/// discard their trace, while failed scenarios save both a screenshot
/// and trace archive.
/// </summary>
public class PlaywrightFixture
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    private bool _traceStarted;

    public IPage? Page { get; private set; }

    /// <summary>
    /// Starts Playwright, launches Chromium, creates an isolated
    /// browser context and begins trace recording.
    /// </summary>
    public async Task StartAsync()
    {
        _playwright =
            await Microsoft.Playwright.Playwright.CreateAsync();

        _browser =
            await _playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless =
                        TestConfiguration.Headless
                });

        _context =
            await _browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    ViewportSize =
                        new ViewportSize
                        {
                            Width = 1440,
                            Height = 900
                        }
                });

        await _context.Tracing.StartAsync(
            new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

        _traceStarted = true;

        Page =
            await _context.NewPageAsync();
    }

    /// <summary>
    /// Captures a full-page screenshot and saves the Playwright
    /// trace for a failed scenario.
    ///
    /// Both artefacts share the same timestamp so they can easily
    /// be matched during investigation or demonstration.
    /// </summary>
    public async Task<(string ScreenshotPath, string TracePath)>
        CaptureFailureArtifactsAsync(
            string scenarioName)
    {
        if (Page is null)
        {
            throw new InvalidOperationException(
                "The Playwright page is not available.");
        }

        if (_context is null)
        {
            throw new InvalidOperationException(
                "The Playwright browser context is not available.");
        }

        var timestamp =
            ArtifactPaths.CreateRunTimestamp();

        var screenshotPath =
            ArtifactPaths.CreateScreenshotPath(
                "Playwright",
                scenarioName,
                timestamp);

        var tracePath =
            ArtifactPaths.CreateTracePath(
                "Playwright",
                scenarioName,
                timestamp);

        await Page.ScreenshotAsync(
            new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });

        if (_traceStarted)
        {
            await _context.Tracing.StopAsync(
                new TracingStopOptions
                {
                    Path = tracePath
                });

            _traceStarted = false;
        }

        return (
            screenshotPath,
            tracePath);
    }

    /// <summary>
    /// Closes the page, browser context and browser after the scenario.
    ///
    /// If the scenario passed, the trace is stopped without being
    /// saved so successful executions do not clutter the artefact folder.
    /// </summary>
    public async Task StopAsync()
    {
        try
        {
            if (_context is not null &&
                _traceStarted)
            {
                await _context.Tracing.StopAsync();

                _traceStarted = false;
            }
        }
        finally
        {
            if (Page is not null)
            {
                await Page.CloseAsync();

                Page = null;
            }

            if (_context is not null)
            {
                await _context.CloseAsync();

                _context = null;
            }

            if (_browser is not null)
            {
                await _browser.CloseAsync();

                _browser = null;
            }

            _playwright?.Dispose();

            _playwright = null;
        }
    }
}
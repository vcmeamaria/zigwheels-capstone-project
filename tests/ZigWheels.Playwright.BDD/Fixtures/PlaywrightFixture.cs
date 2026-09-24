using Microsoft.Playwright;
using ZigWheels.Framework.Core.Configuration;

namespace ZigWheels.Playwright.BDD.Fixtures;

/// <summary>
/// Creates and manages an isolated Playwright browser session.
///
/// Each scenario receives its own BrowserContext and Page so that
/// cookies, storage and browser state are not shared between scenarios.
/// This keeps the framework suitable for parallel execution.
/// </summary>
public class PlaywrightFixture
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    public IPage? Page { get; private set; }

    /// <summary>
    /// Starts Playwright, launches Chromium and creates an isolated page.
    /// </summary>
    public async Task StartAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        _browser = await _playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = TestConfiguration.Headless
            });

        _context = await _browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = 1440,
                    Height = 900
                }
            });

        Page = await _context.NewPageAsync();
    }

    /// <summary>
    /// Closes the page, browser context and browser after the scenario.
    /// </summary>
    public async Task StopAsync()
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
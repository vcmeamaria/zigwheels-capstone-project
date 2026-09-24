using Reqnroll;
using ZigWheels.Playwright.BDD.Fixtures;

namespace ZigWheels.Playwright.BDD.Hooks;

/// <summary>
/// Manages the Playwright browser lifecycle for each BDD scenario.
///
/// Every scenario receives its own PlaywrightFixture, BrowserContext
/// and Page so that browser state remains isolated between tests.
/// </summary>
[Binding]
public class PlaywrightHooks
{
    public const string FixtureKey = "PlaywrightFixture";

    private readonly ScenarioContext _scenarioContext;

    public PlaywrightHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario(Order = 0)]
    public async Task StartPlaywrightAsync()
    {
        var fixture = new PlaywrightFixture();

        await fixture.StartAsync();

        _scenarioContext[FixtureKey] = fixture;
    }

    [AfterScenario(Order = 100)]
    public async Task StopPlaywrightAsync()
    {
        if (!_scenarioContext.TryGetValue(
                FixtureKey,
                out PlaywrightFixture? fixture) ||
            fixture is null)
        {
            return;
        }

        await fixture.StopAsync();
    }
}
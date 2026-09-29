using Microsoft.Playwright;
using ZigWheels.Framework.Core.Configuration;

namespace ZigWheels.Playwright.BDD.Pages;

/// <summary>
/// Represents the ZigWheels navigation behaviour used by the
/// menu-navigation and browser-back BDD scenario.
/// </summary>
public class NavigationPage
{
    private readonly IPage _page;

    private const string UpcomingBikesRelativeUrl =
        "/upcoming-bikes";

    public NavigationPage(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Opens the ZigWheels homepage.
    /// </summary>
    public async Task OpenHomepageAsync()
    {
        var response = await _page.GotoAsync(
            TestConfiguration.BaseUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

        if (response is null)
        {
            throw new InvalidOperationException(
                "No HTTP response was received when opening the ZigWheels homepage.");
        }

        if (!response.Ok)
        {
            throw new InvalidOperationException(
                $"The ZigWheels homepage did not load successfully. " +
                $"HTTP status: {response.Status}");
        }

        await _page.WaitForTimeoutAsync(1500);

        await HandleCookieConsentAsync();

        Console.WriteLine(
            $"ZigWheels homepage opened: {_page.Url}");
    }

    /// <summary>
    /// Opens the New Bikes navigation menu.
    /// </summary>
    public async Task OpenNewBikesMenuAsync()
    {
        await HandleCookieConsentAsync();

        var newBikesMenu = _page
            .GetByText(
                "NEW BIKES",
                new PageGetByTextOptions
                {
                    Exact = true
                })
            .First;

        await newBikesMenu.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

        await newBikesMenu.HoverAsync();

        await _page.WaitForTimeoutAsync(750);

        var upcomingBikesLink = GetUpcomingBikesLink();

        await upcomingBikesLink.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

        Console.WriteLine(
            "New Bikes menu opened successfully.");
    }

    /// <summary>
    /// Selects the Upcoming Bikes collection from the New Bikes menu.
    /// </summary>
    public async Task SelectUpcomingBikesAsync()
    {
        /*
         * Check once more in case the consent dialog appeared late.
         * If it did, dismissing it can close the menu, so we reopen
         * the menu afterwards before clicking Upcoming Bikes.
         */
        var consentWasHandled =
            await HandleCookieConsentAsync();

        if (consentWasHandled)
        {
            await OpenNewBikesMenuAsync();
        }

        var upcomingBikesLink = GetUpcomingBikesLink();

        if (!await upcomingBikesLink.IsVisibleAsync())
        {
            Console.WriteLine(
                "Upcoming Bikes link is not visible. Reopening New Bikes menu.");

            await OpenNewBikesMenuAsync();
        }

        await upcomingBikesLink.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

        await upcomingBikesLink.ClickAsync();

        await _page.WaitForURLAsync(
            "**/upcoming-bikes*",
            new PageWaitForURLOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

        await _page.WaitForTimeoutAsync(750);

        Console.WriteLine(
            $"Upcoming Bikes collection opened: {_page.Url}");
    }

    /// <summary>
    /// Confirms that the Upcoming Bikes collection page is displayed.
    /// </summary>
    public async Task<bool> IsUpcomingBikesPageDisplayedAsync()
    {
        if (!HasExpectedPath(
                UpcomingBikesRelativeUrl))
        {
            return false;
        }

        var pageHeading = _page
            .GetByText(
                "Upcoming Bikes in India",
                new PageGetByTextOptions
                {
                    Exact = false
                })
            .First;

        return await pageHeading.IsVisibleAsync();
    }

    /// <summary>
    /// Uses browser history to return to the previous page.
    /// </summary>
    public async Task NavigateBackAsync()
    {
        Console.WriteLine(
            $"Navigating back from: {_page.Url}");

        await _page.GoBackAsync(
            new PageGoBackOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

        await _page.WaitForFunctionAsync(
            "() => window.location.pathname === '/'",
            null,
            new PageWaitForFunctionOptions
            {
                Timeout = 30000
            });

        await _page.WaitForTimeoutAsync(750);

        await HandleCookieConsentAsync();

        Console.WriteLine(
            $"Returned to: {_page.Url}");
    }

    /// <summary>
    /// Confirms that the browser has returned to the ZigWheels homepage.
    /// </summary>
    public bool IsHomepageDisplayed()
    {
        try
        {
            var currentUri =
                new Uri(_page.Url);

            var configuredBaseUri =
                new Uri(TestConfiguration.BaseUrl);

            return string.Equals(
                       currentUri.Host,
                       configuredBaseUri.Host,
                       StringComparison.OrdinalIgnoreCase)
                   &&
                   currentUri.AbsolutePath == "/";
        }
        catch (UriFormatException)
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the current browser URL for diagnostics.
    /// </summary>
    public string CurrentUrl =>
        _page.Url;

    /// <summary>
    /// Handles the Google Funding Choices consent popup when present.
    /// </summary>
    /// <returns>
    /// True when a consent dialog was found and handled;
    /// otherwise false.
    /// </returns>
    private async Task<bool> HandleCookieConsentAsync()
    {
        /*
         * IMPORTANT:
         * Funding Choices uses class="fc-consent-root",
         * not id="fc-consent-root".
         */
        var consentRoot =
            _page.Locator(".fc-consent-root");

        try
        {
            await consentRoot.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 7000
                });
        }
        catch (TimeoutException)
        {
            Console.WriteLine(
                "Cookie consent popup was not displayed.");

            return false;
        }

        Console.WriteLine(
            "Cookie consent popup detected.");

        var consentButton = _page
            .GetByRole(
                AriaRole.Button,
                new PageGetByRoleOptions
                {
                    Name = "Consent",
                    Exact = true
                })
            .First;

        await consentButton.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });

        await consentButton.ClickAsync();

        Console.WriteLine(
            "Cookie consent accepted.");

        try
        {
            await consentRoot.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Hidden,
                    Timeout = 5000
                });
        }
        catch (TimeoutException)
        {
            Console.WriteLine(
                "Cookie consent root remained visible after clicking Consent.");
        }

        await _page.WaitForTimeoutAsync(500);

        return true;
    }

    /// <summary>
    /// Returns the Upcoming Bikes navigation link.
    /// </summary>
    private ILocator GetUpcomingBikesLink()
    {
        return _page
            .GetByRole(
                AriaRole.Link,
                new PageGetByRoleOptions
                {
                    Name = "Upcoming Bikes",
                    Exact = true
                })
            .First;
    }

    /// <summary>
    /// Determines whether the current URL matches the expected path.
    /// </summary>
    private bool HasExpectedPath(
        string expectedPath)
    {
        try
        {
            var currentUri =
                new Uri(_page.Url);

            return string.Equals(
                currentUri.AbsolutePath.TrimEnd('/'),
                expectedPath.TrimEnd('/'),
                StringComparison.OrdinalIgnoreCase);
        }
        catch (UriFormatException)
        {
            return false;
        }
    }
}
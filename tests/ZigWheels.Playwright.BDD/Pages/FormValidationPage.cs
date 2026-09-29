using Microsoft.Playwright;
using ZigWheels.Framework.Core.Configuration;

namespace ZigWheels.Playwright.BDD.Pages;

/// <summary>
/// Represents the ZigWheels On-Road Price form.
///
/// The page object is used to verify required-field validation
/// without submitting any genuine personal information.
/// </summary>
public class FormValidationPage
{
    private readonly IPage _page;

    private const string PriceQuoteRelativeUrl =
        "/price-quote";

    private const string MakeValidationMessage =
        "Please select make";

    public FormValidationPage(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Opens the ZigWheels On-Road Price form.
    /// </summary>
    public async Task OpenAsync()
    {
        var baseUri =
            new Uri(TestConfiguration.BaseUrl);

        var pageUri =
            new Uri(
                baseUri,
                PriceQuoteRelativeUrl);

        var response = await _page.GotoAsync(
            pageUri.ToString(),
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

        if (response is null)
        {
            throw new InvalidOperationException(
                "No HTTP response was received when opening the On-Road Price form.");
        }

        if (!response.Ok)
        {
            throw new InvalidOperationException(
                $"The On-Road Price form did not load successfully. " +
                $"HTTP status: {response.Status}");
        }

        await _page.WaitForTimeoutAsync(1500);

        await HandleCookieConsentAsync();

        var heading = _page
            .GetByText(
                "New Cars on Road Price",
                new PageGetByTextOptions
                {
                    Exact = true
                })
            .First;

        await heading.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

        Console.WriteLine(
            $"On-Road Price form opened: {_page.Url}");
    }

    /// <summary>
    /// Attempts to submit the form without selecting a vehicle make.
    ///
    /// No personal information is entered because this scenario only
    /// verifies required-field validation behaviour.
    /// </summary>
    public async Task SubmitWithoutSelectingMakeAsync()
    {
        await HandleCookieConsentAsync();

        var submitControl =
            GetSubmitControl();

        await submitControl.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

        await submitControl.ScrollIntoViewIfNeededAsync();

        await submitControl.ClickAsync();

        Console.WriteLine(
            "On-Road Price form submitted without selecting a vehicle make.");
    }

    /// <summary>
    /// Determines whether the expected make validation warning is displayed.
    /// </summary>
    public async Task<bool> IsMakeValidationWarningDisplayedAsync()
    {
        var warning = _page
            .GetByText(
                MakeValidationMessage,
                new PageGetByTextOptions
                {
                    Exact = true
                })
            .First;

        try
        {
            await warning.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 10000
                });

            return await warning.IsVisibleAsync();
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the validation warning text for diagnostics.
    /// </summary>
    public async Task<string> GetMakeValidationWarningTextAsync()
    {
        var warning = _page
            .GetByText(
                MakeValidationMessage,
                new PageGetByTextOptions
                {
                    Exact = true
                })
            .First;

        return (await warning.TextContentAsync())?.Trim()
               ?? string.Empty;
    }

    /// <summary>
    /// Returns the current browser URL.
    /// </summary>
    public string CurrentUrl =>
        _page.Url;

    /// <summary>
    /// Returns the visible control containing the
    /// "Show On Road Price" text.
    ///
    /// The ZigWheels control is visually presented as a button,
    /// but is not consistently exposed as a semantic HTML button.
    /// Therefore, this locator intentionally targets its visible text.
    /// </summary>
    private ILocator GetSubmitControl()
    {
        return _page
            .GetByText(
                "Show On Road Price",
                new PageGetByTextOptions
                {
                    Exact = true
                })
            .First;
    }

    /// <summary>
    /// Handles the Google Funding Choices consent popup when displayed.
    /// </summary>
    private async Task<bool> HandleCookieConsentAsync()
    {
        var consentRoot =
            _page.Locator(".fc-consent-root");

        try
        {
            await consentRoot.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
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
                "Cookie consent root remained visible after interaction.");
        }

        await _page.WaitForTimeoutAsync(500);

        return true;
    }
}
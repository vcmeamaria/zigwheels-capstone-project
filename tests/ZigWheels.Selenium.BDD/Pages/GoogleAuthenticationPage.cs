using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ZigWheels.Selenium.BDD.Pages;

/// <summary>
/// Represents the Google authentication popup opened from ZigWheels.
///
/// This page object manages the external Google authentication window,
/// including window switching, invalid account submission, failure capture
/// and returning control to the original ZigWheels window.
/// </summary>
public class GoogleAuthenticationPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    private readonly By _useAnotherAccount =
        By.XPath(
            "//*[normalize-space(.)='Use another account']");

    private readonly By _identifierInput =
        By.Id("identifierId");

    private readonly By _identifierNextButton =
        By.Id("identifierNext");

    public GoogleAuthenticationPage(IWebDriver driver)
    {
        _driver = driver;

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(15));
    }

    /// <summary>
    /// Waits for a second browser window and switches Selenium
    /// from ZigWheels to the Google authentication popup.
    /// </summary>
    public string WaitForGoogleWindowAndSwitch(
        string originalWindowHandle)
    {
        _wait.Until(driver =>
            driver.WindowHandles.Any(handle =>
                !handle.Equals(
                    originalWindowHandle,
                    StringComparison.Ordinal)));

        var googleWindowHandle =
            _driver.WindowHandles.First(handle =>
                !handle.Equals(
                    originalWindowHandle,
                    StringComparison.Ordinal));

        _driver.SwitchTo().Window(
            googleWindowHandle);

        _wait.Until(driver =>
        {
            if (!Uri.TryCreate(
                    driver.Url,
                    UriKind.Absolute,
                    out var currentUri))
            {
                return false;
            }

            return currentUri.Host.Contains(
                "accounts.google.com",
                StringComparison.OrdinalIgnoreCase);
        });

        return googleWindowHandle;
    }

    /// <summary>
    /// Confirms that Selenium is currently controlling
    /// the Google authentication window.
    /// </summary>
    public bool IsGoogleAuthenticationWindow()
    {
        if (!Uri.TryCreate(
                _driver.Url,
                UriKind.Absolute,
                out var currentUri))
        {
            return false;
        }

        return currentUri.Host.Contains(
            "accounts.google.com",
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Selects "Use another account" when Google displays an
    /// account chooser. If the email field is already displayed,
    /// no additional action is required.
    /// </summary>
    public void ChooseAnotherAccountIfRequired()
    {
        _wait.Until(driver =>
            driver.FindElements(_useAnotherAccount)
                .Any(element => element.Displayed) ||
            driver.FindElements(_identifierInput)
                .Any(element => element.Displayed));

        var useAnotherAccount =
            _driver.FindElements(_useAnotherAccount)
                .FirstOrDefault(element =>
                    element.Displayed &&
                    element.Enabled);

        if (useAnotherAccount is not null)
        {
            useAnotherAccount.Click();
        }

        _wait.Until(driver =>
            driver.FindElements(_identifierInput)
                .Any(element =>
                    element.Displayed &&
                    element.Enabled));
    }

    /// <summary>
    /// Submits a deliberately invalid Google account identifier.
    /// No password is entered by the automation.
    /// </summary>
    public void SubmitInvalidIdentifier(
        string invalidIdentifier)
    {
        var identifierInput =
            _wait.Until(driver =>
                driver.FindElements(_identifierInput)
                    .FirstOrDefault(element =>
                        element.Displayed &&
                        element.Enabled));

        if (identifierInput is null)
        {
            throw new WebDriverTimeoutException(
                "The Google account identifier field could not be found.");
        }

        identifierInput.Clear();

        identifierInput.SendKeys(
            invalidIdentifier);

        var nextButton =
            _wait.Until(driver =>
                driver.FindElements(_identifierNextButton)
                    .FirstOrDefault(element =>
                        element.Displayed &&
                        element.Enabled));

        if (nextButton is null)
        {
            throw new WebDriverTimeoutException(
                "The Google account Next button could not be found.");
        }

        nextButton.Click();
    }

    /// <summary>
    /// Captures the authentication failure displayed by Google.
    ///
    /// Google may reject the invalid identifier itself or reject the
    /// Selenium-controlled browser session. Either outcome demonstrates
    /// that authentication was unsuccessful.
    /// </summary>
    public string GetAuthenticationErrorMessage()
    {
        var errorMessage = _wait.Until(driver =>
        {
            var body =
                driver.FindElement(
                    By.TagName("body"));

            var bodyText =
                body.Text.Trim();

            /*
             * Normalise Google's curly apostrophe so:
             *
             * Couldn't
             * Couldn’t
             *
             * are treated the same way.
             */
            var normalisedText =
                bodyText.Replace('’', '\'');

            if (normalisedText.Contains(
                    "Couldn't find this account",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Couldn't find this account";
            }

            if (normalisedText.Contains(
                    "Couldn't sign you in",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (normalisedText.Contains(
                        "This browser or app may not be secure",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return
                        "Couldn't sign you in - " +
                        "This browser or app may not be secure.";
                }

                return "Couldn't sign you in";
            }

            return null;
        });

        if (string.IsNullOrWhiteSpace(
                errorMessage))
        {
            throw new WebDriverTimeoutException(
                "A Google authentication failure could not be captured.");
        }

        return errorMessage;
    }

    /// <summary>
    /// Closes the Google authentication popup and switches Selenium
    /// back to the original ZigWheels browser window.
    /// </summary>
    public void ReturnToZigWheels(
        string originalWindowHandle)
    {
        if (!_driver.CurrentWindowHandle.Equals(
                originalWindowHandle,
                StringComparison.Ordinal))
        {
            _driver.Close();
        }

        _driver.SwitchTo().Window(
            originalWindowHandle);

        _wait.Until(driver =>
        {
            if (!Uri.TryCreate(
                    driver.Url,
                    UriKind.Absolute,
                    out var currentUri))
            {
                return false;
            }

            return currentUri.Host.Contains(
                "zigwheels.com",
                StringComparison.OrdinalIgnoreCase);
        });
    }
}
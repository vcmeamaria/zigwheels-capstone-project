using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace ZigWheels.Selenium.BDD.Pages;

/// <summary>
/// Represents the ZigWheels login dialog.
///
/// This page object is responsible only for interactions on the
/// ZigWheels website. Google authentication behaviour is handled
/// separately after Selenium switches to the Google popup window.
/// </summary>
public class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    private readonly By _desktopLoginControl =
        By.Id("des_lIcon");

    private readonly By _avatarFallback =
        By.XPath(
            "//*[self::img or self::span or self::div]" +
            "[contains(" +
            "translate(@alt," +
            "'ABCDEFGHIJKLMNOPQRSTUVWXYZ'," +
            "'abcdefghijklmnopqrstuvwxyz')," +
            "'avatar') " +
            "or contains(" +
            "translate(@class," +
            "'ABCDEFGHIJKLMNOPQRSTUVWXYZ'," +
            "'abcdefghijklmnopqrstuvwxyz')," +
            "'login')]");

    private readonly By _loginDialogHeading =
        By.XPath(
            "//*[contains(normalize-space(.), " +
            "'Login/Register to ZigWheels')]");

    /*
     * Important:
     * We deliberately target an element whose DIRECT text is "Google".
     *
     * The previous locator was too broad and could match large parent
     * DIV elements that merely contained a Google element somewhere
     * inside them.
     */
    private readonly By _googleLoginText =
        By.XPath(
            "//*[normalize-space(text())='Google']");

    public LoginPage(IWebDriver driver)
    {
        _driver = driver;

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(15));
    }

    /// <summary>
    /// Opens the ZigWheels Login/Register dialog.
    /// </summary>
    public void OpenLoginDialog()
    {
        var loginControl = FindLoginControl();

        ScrollIntoView(loginControl);

        TryClick(loginControl);

        _wait.Until(driver =>
            driver.FindElements(_loginDialogHeading)
                .Any(element => element.Displayed));
    }

    /// <summary>
    /// Confirms that the ZigWheels Login/Register dialog is visible.
    /// </summary>
    public bool IsLoginDialogVisible()
    {
        return _driver.FindElements(_loginDialogHeading)
            .Any(element => element.Displayed);
    }

    /// <summary>
    /// Clicks the Google option inside the ZigWheels Login/Register dialog.
    ///
    /// The method first performs a physical Selenium Actions click against
    /// the visible Google text. If that does not launch the authentication
    /// popup, it searches upwards for the nearest clickable parent and
    /// performs a JavaScript click as a fallback.
    /// </summary>
    public void ClickGoogleLogin()
    {
        var windowsBeforeClick =
            _driver.WindowHandles.Count;

        var googleText = _wait.Until(driver =>
        {
            return driver.FindElements(_googleLoginText)
                .Where(element =>
                    element.Displayed &&
                    element.Enabled)
                .OrderBy(element =>
                    element.Size.Width *
                    element.Size.Height)
                .FirstOrDefault();
        });

        if (googleText is null)
        {
            throw new WebDriverTimeoutException(
                "The Google login option could not be found.");
        }

        ScrollIntoView(googleText);

        /*
         * A Selenium Actions click behaves more like a physical user click
         * than directly calling Click() on a broad container element.
         */
        var actions = new Actions(_driver);

        actions
            .MoveToElement(googleText)
            .Click()
            .Perform();

        if (AuthenticationNavigationStarted(
                windowsBeforeClick,
                TimeSpan.FromSeconds(3)))
        {
            return;
        }

        /*
         * If the text itself was not the interactive element,
         * locate its nearest clickable ancestor.
         */
        var clickableParent =
            FindClickableAncestor(googleText);

        if (clickableParent is not null)
        {
            JavaScriptClick(clickableParent);
        }
        else
        {
            /*
             * Final fallback: dispatch the click directly on the text
             * element. The event will bubble through its parent elements.
             */
            JavaScriptClick(googleText);
        }
    }

    private IWebElement FindLoginControl()
    {
        var loginControl = _wait.Until(driver =>
        {
            var desktopControls =
                driver.FindElements(_desktopLoginControl);

            var desktopControl =
                desktopControls.FirstOrDefault(element =>
                    element.Displayed &&
                    element.Enabled);

            if (desktopControl is not null)
            {
                return desktopControl;
            }

            return driver.FindElements(_avatarFallback)
                .FirstOrDefault(element =>
                    element.Displayed &&
                    element.Enabled);
        });

        if (loginControl is null)
        {
            throw new WebDriverTimeoutException(
                "The ZigWheels login/profile control could not be found.");
        }

        return loginControl;
    }

    private IWebElement? FindClickableAncestor(
        IWebElement startingElement)
    {
        var currentElement = startingElement;

        for (var level = 0; level < 5; level++)
        {
            try
            {
                currentElement =
                    currentElement.FindElement(
                        By.XPath("./parent::*"));

                var tagName =
                    currentElement.TagName;

                var role =
                    currentElement.GetAttribute("role");

                var cursor =
                    GetCursorStyle(currentElement);

                if (tagName.Equals(
                        "button",
                        StringComparison.OrdinalIgnoreCase) ||
                    tagName.Equals(
                        "a",
                        StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(
                        role,
                        "button",
                        StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(
                        cursor,
                        "pointer",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return currentElement;
                }
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        }

        return null;
    }

    private bool AuthenticationNavigationStarted(
        int originalWindowCount,
        TimeSpan timeout)
    {
        try
        {
            var shortWait =
                new WebDriverWait(
                    _driver,
                    timeout);

            return shortWait.Until(driver =>
            {
                if (driver.WindowHandles.Count >
                    originalWindowCount)
                {
                    return true;
                }

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
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    private string GetCursorStyle(
        IWebElement element)
    {
        if (_driver is not IJavaScriptExecutor javaScript)
        {
            return string.Empty;
        }

        var result =
            javaScript.ExecuteScript(
                "return window.getComputedStyle(arguments[0]).cursor;",
                element);

        return result?.ToString() ??
               string.Empty;
    }

    private void TryClick(
        IWebElement element)
    {
        try
        {
            element.Click();
        }
        catch (ElementClickInterceptedException)
        {
            JavaScriptClick(element);
        }
        catch (ElementNotInteractableException)
        {
            JavaScriptClick(element);
        }
    }

    private void JavaScriptClick(
        IWebElement element)
    {
        if (_driver is not IJavaScriptExecutor javaScript)
        {
            throw new InvalidOperationException(
                "The WebDriver does not support JavaScript execution.");
        }

        javaScript.ExecuteScript(
            "arguments[0].click();",
            element);
    }

    private void ScrollIntoView(
        IWebElement element)
    {
        if (_driver is not IJavaScriptExecutor javaScript)
        {
            return;
        }

        javaScript.ExecuteScript(
            "arguments[0].scrollIntoView({block: 'center'});",
            element);
    }
}
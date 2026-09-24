using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ZigWheels.Selenium.BDD.Components;

/// <summary>
/// Handles the ZigWheels cookie/privacy consent dialog.
///
/// ZigWheels can display the consent message shortly after the main page
/// has loaded. The component therefore waits briefly for the dialog and
/// supports controls located either in the main page or inside an iframe.
/// </summary>
public class CookieConsentComponent
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    private readonly By _consentButton =
        By.XPath(
            "//*[self::button or self::a or @role='button']" +
            "[normalize-space(.)='Consent' " +
            "or .//*[normalize-space(.)='Consent']]");

    public CookieConsentComponent(IWebDriver driver)
    {
        _driver = driver;

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(8));
    }

    /// <summary>
    /// Waits briefly for the ZigWheels consent dialog and accepts it
    /// when present. If no dialog appears, the test continues normally.
    /// </summary>
    public void AcceptIfPresent()
    {
        try
        {
            _wait.Until(_ =>
            {
                return TryAcceptConsent();
            });
        }
        catch (WebDriverTimeoutException)
        {
            // Consent is not always displayed.
            // If it does not appear within the wait period,
            // continue with the scenario.
        }
        finally
        {
            _driver.SwitchTo().DefaultContent();
        }
    }

    private bool TryAcceptConsent()
    {
        _driver.SwitchTo().DefaultContent();

        if (TryClickConsentButton())
        {
            WaitForConsentToDisappear();

            return true;
        }

        var frames = _driver.FindElements(
            By.TagName("iframe"));

        foreach (var frame in frames)
        {
            try
            {
                if (!frame.Displayed)
                {
                    continue;
                }

                _driver.SwitchTo().DefaultContent();
                _driver.SwitchTo().Frame(frame);

                if (TryClickConsentButton())
                {
                    _driver.SwitchTo().DefaultContent();

                    WaitForConsentToDisappear();

                    return true;
                }
            }
            catch (StaleElementReferenceException)
            {
                // The iframe changed while the consent UI was loading.
            }
            catch (NoSuchFrameException)
            {
                // The iframe disappeared before Selenium could switch.
            }
            finally
            {
                _driver.SwitchTo().DefaultContent();
            }
        }

        return false;
    }

    private bool TryClickConsentButton()
    {
        var consentButton =
            _driver.FindElements(_consentButton)
                .FirstOrDefault(element =>
                    element.Displayed &&
                    element.Enabled);

        if (consentButton is null)
        {
            return false;
        }

        consentButton.Click();

        return true;
    }

    private void WaitForConsentToDisappear()
    {
        try
        {
            var shortWait = new WebDriverWait(
                _driver,
                TimeSpan.FromSeconds(5));

            shortWait.Until(driver =>
            {
                driver.SwitchTo().DefaultContent();

                var visibleConsentButtons =
                    driver.FindElements(_consentButton)
                        .Any(element => element.Displayed);

                if (visibleConsentButtons)
                {
                    return false;
                }

                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            // The click has already occurred.
            // Allow the scenario to continue if the CMP takes
            // slightly longer to remove its UI.
        }
    }
}
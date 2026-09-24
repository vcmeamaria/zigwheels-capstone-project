using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using ZigWheels.Selenium.BDD.Hooks;
using ZigWheels.Selenium.BDD.Pages;

namespace ZigWheels.Selenium.BDD.StepDefinitions;

/// <summary>
/// BDD step definitions for the Google authentication error scenario.
///
/// The flow demonstrates Selenium browser-window handling by moving
/// between the ZigWheels window and the Google authentication popup.
/// </summary>
[Binding]
public class GoogleLoginSteps
{
    private const string OriginalWindowKey =
        "OriginalZigWheelsWindowHandle";

    private const string GoogleWindowKey =
        "GoogleAuthenticationWindowHandle";

    private const string InvalidGoogleIdentifier =
        "zigwheels.capstone.invalid@example.invalid";

    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _outputHelper;

    public GoogleLoginSteps(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        _scenarioContext = scenarioContext;
        _outputHelper = outputHelper;
    }

    [When("I open the ZigWheels login dialog")]
    public void WhenIOpenTheZigWheelsLoginDialog()
    {
        var loginPage =
            new LoginPage(GetDriver());

        loginPage.OpenLoginDialog();

        Assert.That(
            loginPage.IsLoginDialogVisible(),
            Is.True,
            "The ZigWheels Login/Register dialog was not displayed.");
    }

    [When("I choose Google login")]
    public void WhenIChooseGoogleLogin()
    {
        var driver = GetDriver();

        _scenarioContext[OriginalWindowKey] =
            driver.CurrentWindowHandle;

        var loginPage =
            new LoginPage(driver);

        loginPage.ClickGoogleLogin();
    }

    [Then("a Google authentication window should open")]
    public void ThenAGoogleAuthenticationWindowShouldOpen()
    {
        var driver = GetDriver();

        var originalWindowHandle =
            GetOriginalWindowHandle();

        var googleAuthenticationPage =
            new GoogleAuthenticationPage(driver);

        var googleWindowHandle =
            googleAuthenticationPage
                .WaitForGoogleWindowAndSwitch(
                    originalWindowHandle);

        _scenarioContext[GoogleWindowKey] =
            googleWindowHandle;

        Assert.That(
            driver.WindowHandles.Count,
            Is.GreaterThanOrEqualTo(2),
            "A second browser window was not opened.");

        Assert.That(
            googleAuthenticationPage
                .IsGoogleAuthenticationWindow(),
            Is.True,
            "Selenium did not switch to the Google authentication window.");

        _outputHelper.WriteLine(
            "Google authentication popup opened successfully.");

        _outputHelper.WriteLine(
            $"Browser window count: {driver.WindowHandles.Count}");
    }

    [When("I choose to use another Google account")]
    public void WhenIChooseToUseAnotherGoogleAccount()
    {
        var googleAuthenticationPage =
            new GoogleAuthenticationPage(
                GetDriver());

        googleAuthenticationPage
            .ChooseAnotherAccountIfRequired();

        _outputHelper.WriteLine(
            "Google account identifier field is ready.");
    }

    [When("I submit an invalid Google account identifier")]
    public void WhenISubmitAnInvalidGoogleAccountIdentifier()
    {
        var googleAuthenticationPage =
            new GoogleAuthenticationPage(
                GetDriver());

        googleAuthenticationPage
            .SubmitInvalidIdentifier(
                InvalidGoogleIdentifier);

        _outputHelper.WriteLine(
            $"Submitted non-production identifier: " +
            $"{InvalidGoogleIdentifier}");
    }

    [Then("a Google authentication error should be displayed")]
    public void ThenAGoogleAuthenticationErrorShouldBeDisplayed()
    {
        var googleAuthenticationPage =
            new GoogleAuthenticationPage(
                GetDriver());

        var actualError =
            googleAuthenticationPage
                .GetAuthenticationErrorMessage();

        _outputHelper.WriteLine(
            $"Google authentication error captured: {actualError}");

        Assert.That(
            actualError,
            Is.Not.Null.And.Not.Empty,
            "Google did not display an authentication error.");
    }

    [Then("I should be able to return to the ZigWheels window")]
    public void ThenIShouldBeAbleToReturnToTheZigWheelsWindow()
    {
        var driver = GetDriver();

        var originalWindowHandle =
            GetOriginalWindowHandle();

        var googleAuthenticationPage =
            new GoogleAuthenticationPage(driver);

        googleAuthenticationPage.ReturnToZigWheels(
            originalWindowHandle);

        Assert.That(
            driver.CurrentWindowHandle,
            Is.EqualTo(originalWindowHandle),
            "Selenium did not return to the original ZigWheels window.");

        var homePage =
            new HomePage(driver);

        Assert.That(
            homePage.IsLoaded(),
            Is.True,
            "The browser did not return to the ZigWheels website.");

        _outputHelper.WriteLine(
            "Returned successfully to the original ZigWheels window.");
    }

    private IWebDriver GetDriver()
    {
        if (!_scenarioContext.TryGetValue(
                SeleniumHooks.DriverKey,
                out IWebDriver? driver) ||
            driver is null)
        {
            throw new InvalidOperationException(
                "The Selenium WebDriver has not been created " +
                "for this scenario.");
        }

        return driver;
    }

    private string GetOriginalWindowHandle()
    {
        if (!_scenarioContext.TryGetValue(
                OriginalWindowKey,
                out string? originalWindowHandle) ||
            string.IsNullOrWhiteSpace(
                originalWindowHandle))
        {
            throw new InvalidOperationException(
                "The original ZigWheels window handle " +
                "has not been stored.");
        }

        return originalWindowHandle;
    }
}
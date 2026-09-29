using Microsoft.Playwright;
using ZigWheels.Framework.Core.Configuration;

namespace ZigWheels.Playwright.BDD.Pages;

/// <summary>
/// Represents a ZigWheels video review page containing
/// an embedded YouTube video iframe.
/// </summary>
public class FrameHandlingPage
{
    private readonly IPage _page;

    private const string VideoReviewRelativeUrl =
        "/gallery/reviews/top-music-kawasaki-z800-powerdrift/31104/1";

    private const string ExpectedHeading =
        "Top Music : Kawasaki Z800 : PowerDrift";

    private IFrame? _videoFrame;

    public FrameHandlingPage(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Opens the ZigWheels video review page.
    /// </summary>
    public async Task OpenVideoReviewPageAsync()
    {
        var baseUri =
            new Uri(TestConfiguration.BaseUrl);

        var pageUri =
            new Uri(
                baseUri,
                VideoReviewRelativeUrl);

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
                "No HTTP response was received when opening the ZigWheels video review page.");
        }

        if (!response.Ok)
        {
            throw new InvalidOperationException(
                $"The ZigWheels video review page did not load successfully. " +
                $"HTTP status: {response.Status}");
        }

        await _page.WaitForTimeoutAsync(1500);

        await HandleCookieConsentAsync();

        var heading =
            GetPageHeading();

        await heading.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

        Console.WriteLine(
            $"ZigWheels video review page opened: {_page.Url}");
    }

    /// <summary>
    /// Activates the video preview and accesses the resulting
    /// YouTube iframe.
    /// </summary>
    public async Task AccessEmbeddedVideoFrameAsync()
    {
        await HandleCookieConsentAsync();

        /*
         * First check whether YouTube has already been loaded.
         * If not, activate the large video preview.
         */
        _videoFrame =
            FindYouTubeFrame();

        if (_videoFrame is null)
        {
            Console.WriteLine(
                "YouTube iframe is not loaded yet. Activating video preview.");

            await ActivateVideoPreviewAsync();

            _videoFrame =
                await WaitForYouTubeFrameAsync();
        }

        if (_videoFrame is null)
        {
            await WriteIframeDiagnosticsAsync();

            throw new InvalidOperationException(
                "The YouTube iframe did not become available after activating the video preview.");
        }

        Console.WriteLine(
            $"YouTube frame detected: {_videoFrame.Url}");

        /*
         * Access content inside the child frame.
         * This demonstrates genuine iframe/frame-context handling.
         */
        var frameBody =
            _videoFrame.Locator("body");

        await frameBody.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = 15000
            });

        Console.WriteLine(
            "Embedded YouTube iframe accessed successfully.");
    }

    /// <summary>
    /// Verifies that Playwright can access the document
    /// inside the embedded video frame.
    /// </summary>
    public async Task<bool> IsEmbeddedVideoFrameAvailableAsync()
    {
        if (_videoFrame is null)
        {
            return false;
        }

        try
        {
            var frameBody =
                _videoFrame.Locator("body");

            await frameBody.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Attached,
                    Timeout = 10000
                });

            return await frameBody.CountAsync() > 0;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Verifies that the original ZigWheels page remains
    /// available outside the child iframe.
    /// </summary>
    public async Task<bool> IsMainPageContentAvailableAsync()
    {
        var heading =
            GetPageHeading();

        try
        {
            await heading.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 10000
                });

            return await heading.IsVisibleAsync();
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the current ZigWheels browser URL.
    /// </summary>
    public string CurrentUrl =>
        _page.Url;

    /// <summary>
    /// Activates the large ZigWheels video preview.
    ///
    /// ZigWheels displays a large thumbnail with a Play overlay
    /// before loading the actual YouTube iframe.
    /// </summary>
    private async Task ActivateVideoPreviewAsync()
    {
        var heading =
            GetPageHeading();

        /*
         * The main video thumbnail is the first large image
         * following the article heading.
         *
         * We identify a genuinely large visible image instead of
         * relying on a fragile CSS class used by the live website.
         */
        var imagesAfterHeading =
            heading.Locator("xpath=following::img");

        var imageCount =
            await imagesAfterHeading.CountAsync();

        var maximumImagesToCheck =
            Math.Min(imageCount, 15);

        for (var index = 0;
             index < maximumImagesToCheck;
             index++)
        {
            var image =
                imagesAfterHeading.Nth(index);

            if (!await image.IsVisibleAsync())
            {
                continue;
            }

            var box =
                await image.BoundingBoxAsync();

            if (box is null)
            {
                continue;
            }

            /*
             * Ignore small icons, logos and recommended-video
             * thumbnails. The main review preview is substantially
             * larger than these elements.
             */
            if (box.Width < 500 ||
                box.Height < 250)
            {
                continue;
            }

            await image.ScrollIntoViewIfNeededAsync();

            box =
                await image.BoundingBoxAsync();

            if (box is null)
            {
                continue;
            }

            var centreX =
                box.X + (box.Width / 2);

            var centreY =
                box.Y + (box.Height / 2);

            Console.WriteLine(
                $"Large video preview located. Size: " +
                $"{box.Width:0}x{box.Height:0}");

            /*
             * Clicking the visual centre hits the Play overlay
             * shown on top of the thumbnail.
             */
            await _page.Mouse.ClickAsync(
                centreX,
                centreY);

            Console.WriteLine(
                "Video preview Play control activated.");

            await _page.WaitForTimeoutAsync(1000);

            return;
        }

        throw new InvalidOperationException(
            "The large ZigWheels video preview could not be located.");
    }

    /// <summary>
    /// Waits for a YouTube child frame to be created after
    /// the video preview has been activated.
    /// </summary>
    private async Task<IFrame?> WaitForYouTubeFrameAsync()
    {
        const int maximumAttempts = 30;
        const int delayMilliseconds = 500;

        for (var attempt = 1;
             attempt <= maximumAttempts;
             attempt++)
        {
            var frame =
                FindYouTubeFrame();

            if (frame is not null)
            {
                Console.WriteLine(
                    $"YouTube iframe loaded after attempt {attempt}.");

                return frame;
            }

            await _page.WaitForTimeoutAsync(
                delayMilliseconds);
        }

        return null;
    }

    /// <summary>
    /// Searches all Playwright page frames for the YouTube
    /// video frame.
    ///
    /// This avoids depending on the exact iframe HTML attributes
    /// used by the live ZigWheels website.
    /// </summary>
    private IFrame? FindYouTubeFrame()
    {
        return _page.Frames
            .FirstOrDefault(
                frame =>
                    frame.Url.Contains(
                        "youtube.com",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    frame.Url.Contains(
                        "youtube-nocookie.com",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    frame.Url.Contains(
                        "youtu.be",
                        StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Prints iframe information when the expected YouTube
    /// frame cannot be located.
    /// </summary>
    private async Task WriteIframeDiagnosticsAsync()
    {
        Console.WriteLine(
            "----- IFRAME DIAGNOSTICS -----");

        Console.WriteLine(
            $"Playwright frames detected: {_page.Frames.Count}");

        foreach (var frame in _page.Frames)
        {
            Console.WriteLine(
                $"Frame URL: {frame.Url}");
        }

        var iframeElements =
            _page.Locator("iframe");

        var iframeCount =
            await iframeElements.CountAsync();

        Console.WriteLine(
            $"iframe elements detected in DOM: {iframeCount}");

        for (var index = 0;
             index < iframeCount;
             index++)
        {
            var iframe =
                iframeElements.Nth(index);

            var source =
                await iframe.GetAttributeAsync("src");

            var dataSource =
                await iframe.GetAttributeAsync("data-src");

            var title =
                await iframe.GetAttributeAsync("title");

            Console.WriteLine(
                $"Iframe {index + 1}:");

            Console.WriteLine(
                $"  src: {source ?? "not set"}");

            Console.WriteLine(
                $"  data-src: {dataSource ?? "not set"}");

            Console.WriteLine(
                $"  title: {title ?? "not set"}");
        }

        Console.WriteLine(
            "------------------------------");
    }

    /// <summary>
    /// Returns the main video-review page heading.
    /// </summary>
    private ILocator GetPageHeading()
    {
        return _page
            .GetByText(
                ExpectedHeading,
                new PageGetByTextOptions
                {
                    Exact = true
                })
            .First;
    }

    /// <summary>
    /// Handles the Google Funding Choices consent popup
    /// when it appears.
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

        var consentButton =
            _page
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
using Microsoft.Playwright;
using ZigWheels.Framework.Core.Configuration;
using ZigWheels.Framework.Core.Models;

namespace ZigWheels.Playwright.BDD.Pages;

/// <summary>
/// Represents the ZigWheels Used Cars in Chennai page.
///
/// The page object extracts the popular used-car model information
/// displayed by the website and returns it as structured test data.
/// </summary>
public class UsedCarsChennaiPage
{
    private readonly IPage _page;

    private const string RelativeUrl = "used-car/Chennai";

    public UsedCarsChennaiPage(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Opens the ZigWheels Used Cars page for Chennai.
    /// </summary>
    public async Task OpenAsync()
    {
        var targetUrl = new Uri(
            new Uri(TestConfiguration.BaseUrl),
            RelativeUrl);

        await _page.GotoAsync(
            targetUrl.ToString(),
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
    }

    /// <summary>
    /// Confirms that the Chennai used-car page has loaded.
    /// </summary>
    public async Task<bool> IsLoadedAsync()
    {
        var heading = _page.GetByText(
            "Top 10 Used Cars In Chennai",
            new PageGetByTextOptions
            {
                Exact = false
            });

        return await heading.CountAsync() > 0;
    }

    /// <summary>
    /// Extracts the popular used-car models and their inventory counts.
    /// </summary>
    public async Task<IReadOnlyList<UsedCarModel>> GetPopularModelsAsync()
    {
        var sectionHeading = _page.GetByText(
            "Top 10 Used Cars In Chennai",
            new PageGetByTextOptions
            {
                Exact = false
            });

        await sectionHeading.First.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });

        var rows = _page.Locator(
            "table tr");

        var rowCount = await rows.CountAsync();

        var models = new List<UsedCarModel>();

        for (var index = 0; index < rowCount; index++)
        {
            var row = rows.Nth(index);

            var cells = row.Locator("td");

            var cellCount = await cells.CountAsync();

            if (cellCount < 2)
            {
                continue;
            }

            var modelName =
                (await cells.Nth(0).InnerTextAsync()).Trim();

            var inventoryCount =
                (await cells.Nth(1).InnerTextAsync()).Trim();

            if (string.IsNullOrWhiteSpace(modelName))
            {
                continue;
            }

            if (modelName.Equals(
                    "Model Name",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            models.Add(
                new UsedCarModel
                {
                    Name = modelName,
                    InventoryCount = inventoryCount
                });
        }

        return models;
    }
}
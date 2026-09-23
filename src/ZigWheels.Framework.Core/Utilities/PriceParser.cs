using System.Globalization;
using System.Text.RegularExpressions;

namespace ZigWheels.Framework.Core.Utilities;

/// <summary>
/// Converts price text displayed by ZigWheels into a numeric rupee value
/// that can be used by automated assertions and filtering logic.
/// </summary>
public static class PriceParser
{
    public static decimal ParseToRupees(string displayedPrice)
    {
        if (string.IsNullOrWhiteSpace(displayedPrice))
        {
            return 0;
        }

        var normalizedPrice = displayedPrice
            .Replace("₹", string.Empty)
            .Replace("Rs.", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Rs", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(",", string.Empty)
            .Trim();

        var match = Regex.Match(
            normalizedPrice,
            @"(\d+(?:\.\d+)?)",
            RegexOptions.IgnoreCase);

        if (!match.Success ||
            !decimal.TryParse(
                match.Groups[1].Value,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var numericValue))
        {
            return 0;
        }

        if (normalizedPrice.Contains("Lakh", StringComparison.OrdinalIgnoreCase))
        {
            return numericValue * 100000;
        }

        if (normalizedPrice.Contains("Crore", StringComparison.OrdinalIgnoreCase))
        {
            return numericValue * 10000000;
        }

        return numericValue;
    }
}
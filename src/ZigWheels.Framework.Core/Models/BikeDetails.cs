namespace ZigWheels.Framework.Core.Models;

/// <summary>
/// Represents an upcoming bike extracted from the ZigWheels website.
/// The model stores both the displayed values and the parsed numeric
/// price used by automated assertions.
/// </summary>
public class BikeDetails
{
    public string Name { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string DisplayedPrice { get; set; } = string.Empty;

    public decimal PriceInRupees { get; set; }

    public string ExpectedLaunchDate { get; set; } = string.Empty;
}
namespace TrueLayer.PaymentsProviders.Model
{
    /// <summary>
    /// Represents a payments provider
    /// </summary>
    public record PaymentsProvider(
        string Id,
        Capabilities Capabilities,
        string? DisplayName,
        string? IconUri,
        string? LogoUri,
        string? BgColor,
        string? CountryCode
    );
}

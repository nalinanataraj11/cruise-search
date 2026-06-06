namespace CruiseSearch.Api.Models;

public class BookingConfirmation
{
    public required string BookingReference { get; set; }
    public required string Status { get; set; }
    public required string ProviderId { get; set; }
    public required string ProviderName { get; set; }
    public required string LeadPassengerName { get; set; }
    public required DateTime SailingDate { get; set; }
    public required CabinClass CabinClass { get; set; }
    public required int PassengerCount { get; set; }
    public required decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public required CancellationPolicy CancellationPolicy { get; set; }
    public required DateTime BookedAtUtc { get; set; }
}

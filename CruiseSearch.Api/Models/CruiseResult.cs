namespace CruiseSearch.Api.Models;

public class CruiseResult
{
    public required string ProviderId { get; set; }
    public required string ProviderName { get; set; }
    public string? ShipName { get; set; }
    public required CabinClass CabinClass { get; set; }
    public required decimal PricePerPerson { get; set; }
    public decimal? PortFees { get; set; }
    public required decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public List<string> Inclusions { get; set; } = new();
    public required CancellationPolicy CancellationPolicy { get; set; }
    public required string DeparturePort { get; set; }
    public required DateTime SailingDate { get; set; }
    public required int PassengerCount { get; set; }
}

namespace CruiseSearch.Api.Models;

public class BookingRequest
{
    public required string ProviderId { get; set; }
    public required DateTime SailingDate { get; set; }
    public required CabinClass CabinClass { get; set; }
    public required string DeparturePort { get; set; }
    public required string LeadPassengerName { get; set; }
    public required string PassportNumber { get; set; }
    public string? NationalIdNumber { get; set; }
    public required int PassengerCount { get; set; }
    public required decimal PricePerPerson { get; set; }
    public required decimal TotalPrice { get; set; }
}

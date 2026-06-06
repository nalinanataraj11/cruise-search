namespace CruiseSearch.Api.Providers;

using CruiseSearch.Api.Models;

public class OceanLuxProvider : ICruiseProvider
{
    private static readonly Dictionary<string, List<CruiseResult>> StubData = new()
    {
        {
            "Miami-2026-07-15",
            new()
            {
                new CruiseResult
                {
                    ProviderId = "oceanLux",
                    ProviderName = "OceanLux",
                    ShipName = "Symphony of the Seas",
                    CabinClass = CabinClass.Interior,
                    PricePerPerson = 899m,
                    PortFees = 150m,
                    TotalPrice = 1898m,
                    Inclusions = new() { "All meals", "Entertainment", "Beverages" },
                    CancellationPolicy = CancellationPolicy.FullRefund,
                    DeparturePort = "Miami",
                    SailingDate = new DateTime(2026, 7, 15),
                    PassengerCount = 2
                },
                new CruiseResult
                {
                    ProviderId = "oceanLux",
                    ProviderName = "OceanLux",
                    ShipName = "Symphony of the Seas",
                    CabinClass = CabinClass.OceanView,
                    PricePerPerson = 1200m,
                    PortFees = 150m,
                    TotalPrice = 2700m,
                    Inclusions = new() { "All meals", "Entertainment", "Beverages" },
                    CancellationPolicy = CancellationPolicy.FullRefund,
                    DeparturePort = "Miami",
                    SailingDate = new DateTime(2026, 7, 15),
                    PassengerCount = 2
                },
                new CruiseResult
                {
                    ProviderId = "oceanLux",
                    ProviderName = "OceanLux",
                    ShipName = "Symphony of the Seas",
                    CabinClass = CabinClass.Balcony,
                    PricePerPerson = 1599m,
                    PortFees = 150m,
                    TotalPrice = 3498m,
                    Inclusions = new() { "All meals", "Entertainment", "Beverages", "Balcony access" },
                    CancellationPolicy = CancellationPolicy.PartialRefund,
                    DeparturePort = "Miami",
                    SailingDate = new DateTime(2026, 7, 15),
                    PassengerCount = 2
                },
                new CruiseResult
                {
                    ProviderId = "oceanLux",
                    ProviderName = "OceanLux",
                    ShipName = "Symphony of the Seas",
                    CabinClass = CabinClass.Suite,
                    PricePerPerson = 2499m,
                    PortFees = 150m,
                    TotalPrice = 5298m,
                    Inclusions = new() { "All meals", "Entertainment", "Beverages", "Priority service", "Suite lounge access" },
                    CancellationPolicy = CancellationPolicy.NonRefundable,
                    DeparturePort = "Miami",
                    SailingDate = new DateTime(2026, 7, 15),
                    PassengerCount = 2
                }
            }
        }
    };

    private static readonly Dictionary<string, BookingConfirmation> BookingRecords = new();

    public Task<IEnumerable<CruiseResult>> SearchAsync(
        string departurePort,
        DateTime sailingDate,
        CabinClass? cabinClass,
        int passengerCount)
    {
        var key = $"{departurePort}-{sailingDate:yyyy-MM-dd}";

        if (!StubData.TryGetValue(key, out var cruises))
        {
            return Task.FromResult(Enumerable.Empty<CruiseResult>());
        }

        var results = cruises
            .Where(c => cabinClass == null || c.CabinClass == cabinClass)
            .Select(c => new CruiseResult
            {
                ProviderId = c.ProviderId,
                ProviderName = c.ProviderName,
                ShipName = c.ShipName,
                CabinClass = c.CabinClass,
                PricePerPerson = c.PricePerPerson,
                PortFees = c.PortFees,
                TotalPrice = (c.PricePerPerson * passengerCount) + (c.PortFees ?? 0),
                Inclusions = c.Inclusions,
                CancellationPolicy = c.CancellationPolicy,
                DeparturePort = c.DeparturePort,
                SailingDate = c.SailingDate,
                PassengerCount = passengerCount
            })
            .ToList();

        return Task.FromResult((IEnumerable<CruiseResult>)results);
    }

    public Task<BookingConfirmation> BookAsync(BookingRequest request)
    {
        var reference = $"BK-OceanLux-{DateTime.UtcNow:yyyyMMdd}-{BookingRecords.Count + 1:D3}";
        var confirmation = new BookingConfirmation
        {
            BookingReference = reference,
            Status = "confirmed",
            ProviderId = request.ProviderId,
            ProviderName = "OceanLux",
            LeadPassengerName = request.LeadPassengerName,
            SailingDate = request.SailingDate,
            CabinClass = request.CabinClass,
            PassengerCount = request.PassengerCount,
            TotalPrice = request.TotalPrice,
            CancellationPolicy = CancellationPolicy.FullRefund,
            BookedAtUtc = DateTime.UtcNow
        };

        BookingRecords[reference] = confirmation;
        return Task.FromResult(confirmation);
    }

    public Task<BookingConfirmation?> GetBookingStatusAsync(string bookingReference)
    {
        BookingRecords.TryGetValue(bookingReference, out var confirmation);
        return Task.FromResult(confirmation);
    }
}

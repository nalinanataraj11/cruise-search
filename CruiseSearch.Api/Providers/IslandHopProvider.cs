namespace CruiseSearch.Api.Providers;

using CruiseSearch.Api.Models;

public class IslandHopProvider : ICruiseProvider
{
    private static readonly Dictionary<string, List<(CabinClass Cabin, decimal Price, int Availability)>> StubData = new()
    {
        {
            "Miami-2026-07-15",
            new()
            {
                (CabinClass.Interior, 599m, 5),
                (CabinClass.OceanView, 899m, 3),
                (CabinClass.Balcony, 1199m, 2),
                (CabinClass.Suite, 1899m, 1)
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

        if (!StubData.TryGetValue(key, out var cabins))
        {
            return Task.FromResult(Enumerable.Empty<CruiseResult>());
        }

        var results = cabins
            .Where(c => c.Availability > 0) // Filter zero-availability cabins
            .Where(c => cabinClass == null || c.Cabin == cabinClass)
            .Select(c => new CruiseResult
            {
                ProviderId = "islandHop",
                ProviderName = "IslandHop",
                ShipName = null, // IslandHop doesn't provide ship name
                CabinClass = c.Cabin,
                PricePerPerson = c.Price,
                PortFees = null, // IslandHop doesn't charge port fees
                TotalPrice = c.Price * passengerCount,
                Inclusions = new(),
                CancellationPolicy = CancellationPolicy.NonRefundable,
                DeparturePort = departurePort,
                SailingDate = sailingDate,
                PassengerCount = passengerCount
            })
            .ToList();

        return Task.FromResult((IEnumerable<CruiseResult>)results);
    }

    public Task<BookingConfirmation> BookAsync(BookingRequest request)
    {
        var reference = $"BK-IslandHop-{DateTime.UtcNow:yyyyMMdd}-{BookingRecords.Count + 1:D3}";
        var confirmation = new BookingConfirmation
        {
            BookingReference = reference,
            Status = "confirmed",
            ProviderId = request.ProviderId,
            ProviderName = "IslandHop",
            LeadPassengerName = request.LeadPassengerName,
            SailingDate = request.SailingDate,
            CabinClass = request.CabinClass,
            PassengerCount = request.PassengerCount,
            TotalPrice = request.TotalPrice,
            CancellationPolicy = CancellationPolicy.NonRefundable,
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

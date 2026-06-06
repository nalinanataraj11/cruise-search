namespace CruiseSearch.Api.Providers;

using CruiseSearch.Api.Models;

public class RiverVoyageProvider : ICruiseProvider
{
    private static readonly Dictionary<string, List<(CabinClass Cabin, decimal FlatPrice, bool Available)>> StubData = new()
    {
        {
            "Amsterdam-2026-08-10",
            new()
            {
                (CabinClass.Interior, 1800m, true),
                (CabinClass.OceanView, 2400m, true)
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
            .Where(c => c.Available) // Filter unavailable cabins
            .Where(c => cabinClass == null || c.Cabin == cabinClass)
            .Where(c => c.Cabin == CabinClass.Interior || c.Cabin == CabinClass.OceanView) // RiverVoyage supports only these
            .Select(c => new CruiseResult
            {
                ProviderId = "riverVoyage",
                ProviderName = "RiverVoyage",
                ShipName = "Danube Dream",
                CabinClass = c.Cabin,
                PricePerPerson = c.FlatPrice / passengerCount, // Convert flat cabin price to per-person
                PortFees = null,
                TotalPrice = c.FlatPrice,
                Inclusions = new() { "All meals", "Guided tours", "River navigation" },
                CancellationPolicy = DateTime.UtcNow.AddDays(45) <= sailingDate
                    ? CancellationPolicy.FullRefund
                    : CancellationPolicy.NonRefundable,
                DeparturePort = departurePort,
                SailingDate = sailingDate,
                PassengerCount = passengerCount
            })
            .ToList();

        return Task.FromResult((IEnumerable<CruiseResult>)results);
    }

    public Task<BookingConfirmation> BookAsync(BookingRequest request)
    {
        var reference = $"BK-RiverVoyage-{DateTime.UtcNow:yyyyMMdd}-{BookingRecords.Count + 1:D3}";
        var confirmation = new BookingConfirmation
        {
            BookingReference = reference,
            Status = "confirmed",
            ProviderId = request.ProviderId,
            ProviderName = "RiverVoyage",
            LeadPassengerName = request.LeadPassengerName,
            SailingDate = request.SailingDate,
            CabinClass = request.CabinClass,
            PassengerCount = request.PassengerCount,
            TotalPrice = request.TotalPrice,
            CancellationPolicy = DateTime.UtcNow.AddDays(45) <= request.SailingDate
                ? CancellationPolicy.FullRefund
                : CancellationPolicy.NonRefundable,
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

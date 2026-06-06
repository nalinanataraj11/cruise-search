namespace CruiseSearch.Api.Services;

using CruiseSearch.Api.Models;
using CruiseSearch.Api.Providers;

public class CruiseAggregatorService
{
    private readonly OceanLuxProvider _oceanLuxProvider;
    private readonly IslandHopProvider _islandHopProvider;
    private readonly RiverVoyageProvider _riverVoyageProvider;

    public CruiseAggregatorService(
        OceanLuxProvider oceanLuxProvider,
        IslandHopProvider islandHopProvider,
        RiverVoyageProvider riverVoyageProvider)
    {
        _oceanLuxProvider = oceanLuxProvider;
        _islandHopProvider = islandHopProvider;
        _riverVoyageProvider = riverVoyageProvider;
    }

    public async Task<SearchResponse> SearchAsync(
        string departurePort,
        DateTime sailingDate,
        CabinClass? cabinClass,
        int passengerCount)
    {
        var tasks = new List<Task<IEnumerable<CruiseResult>>>
        {
            _oceanLuxProvider.SearchAsync(departurePort, sailingDate, cabinClass, passengerCount),
            _islandHopProvider.SearchAsync(departurePort, sailingDate, cabinClass, passengerCount),
            _riverVoyageProvider.SearchAsync(departurePort, sailingDate, cabinClass, passengerCount)
        };

        var results = await Task.WhenAll(tasks);

        var aggregated = results
            .SelectMany(r => r)
            .OrderBy(c => c.TotalPrice)
            .ToList();

        return new SearchResponse
        {
            Results = aggregated,
            SearchTimestampUtc = DateTime.UtcNow
        };
    }

    public async Task<BookingConfirmation> BookAsync(BookingRequest request)
    {
        var provider = request.ProviderId switch
        {
            "oceanLux" => (ICruiseProvider)_oceanLuxProvider,
            "islandHop" => (ICruiseProvider)_islandHopProvider,
            "riverVoyage" => (ICruiseProvider)_riverVoyageProvider,
            _ => throw new InvalidOperationException($"Unknown provider: {request.ProviderId}")
        };

        return await provider.BookAsync(request);
    }

    public async Task<BookingConfirmation?> GetBookingStatusAsync(string bookingReference)
    {
        // Try each provider
        var oceanLuxResult = await _oceanLuxProvider.GetBookingStatusAsync(bookingReference);
        if (oceanLuxResult != null) return oceanLuxResult;

        var islandHopResult = await _islandHopProvider.GetBookingStatusAsync(bookingReference);
        if (islandHopResult != null) return islandHopResult;

        return await _riverVoyageProvider.GetBookingStatusAsync(bookingReference);
    }
}

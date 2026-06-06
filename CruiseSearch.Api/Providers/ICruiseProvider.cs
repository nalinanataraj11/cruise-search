namespace CruiseSearch.Api.Providers;

using CruiseSearch.Api.Models;

public interface ICruiseProvider
{
    Task<IEnumerable<CruiseResult>> SearchAsync(
        string departurePort,
        DateTime sailingDate,
        CabinClass? cabinClass,
        int passengerCount);

    Task<BookingConfirmation> BookAsync(BookingRequest request);

    Task<BookingConfirmation?> GetBookingStatusAsync(string bookingReference);
}

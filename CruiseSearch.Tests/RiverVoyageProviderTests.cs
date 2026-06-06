namespace CruiseSearch.Tests;

using CruiseSearch.Api.Models;
using CruiseSearch.Api.Providers;
using Xunit;

public class RiverVoyageProviderTests
{
    private readonly RiverVoyageProvider _provider = new();

    [Fact]
    public async Task SearchAsync_ReturnsSupportedCabinClassesOnly()
    {
        var departurePort = "Amsterdam";
        var sailingDate = new DateTime(2026, 8, 10);
        var passengerCount = 2;

        var results = await _provider.SearchAsync(departurePort, sailingDate, null, passengerCount);
        var resultList = results.ToList();

        Assert.NotEmpty(resultList);
        Assert.All(resultList, cruise =>
        {
            Assert.True(
                cruise.CabinClass == CabinClass.Interior || cruise.CabinClass == CabinClass.OceanView
            );
        });
    }

    [Fact]
    public async Task SearchAsync_ConvertsFlatCabinToPricePerPerson()
    {
        var departurePort = "Amsterdam";
        var sailingDate = new DateTime(2026, 8, 10);
        var passengerCount = 2;

        var results = await _provider.SearchAsync(departurePort, sailingDate, null, passengerCount);
        var resultList = results.ToList();

        Assert.All(resultList, cruise =>
        {
            var expectedPerPerson = cruise.TotalPrice / passengerCount;
            Assert.Equal(expectedPerPerson, cruise.PricePerPerson);
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task SearchAsync_CorrectlyCalculatesPriceForDifferentPassengerCounts(int passengerCount)
    {
        var departurePort = "Amsterdam";
        var sailingDate = new DateTime(2026, 8, 10);

        var results = await _provider.SearchAsync(departurePort, sailingDate, null, passengerCount);
        var resultList = results.ToList();

        Assert.All(resultList, cruise =>
        {
            var expectedPerPerson = cruise.TotalPrice / passengerCount;
            Assert.Equal(expectedPerPerson, cruise.PricePerPerson);
            Assert.Equal(passengerCount, cruise.PassengerCount);
        });
    }

    [Fact]
    public async Task SearchAsync_NoPortFees()
    {
        var departurePort = "Amsterdam";
        var sailingDate = new DateTime(2026, 8, 10);

        var results = await _provider.SearchAsync(departurePort, sailingDate, null, 2);
        var resultList = results.ToList();

        Assert.All(resultList, cruise => Assert.Null(cruise.PortFees));
    }

    [Fact]
    public async Task BookAsync_GeneratesBookingReference()
    {
        var request = new BookingRequest
        {
            ProviderId = "riverVoyage",
            SailingDate = new DateTime(2026, 8, 10),
            CabinClass = CabinClass.Interior,
            DeparturePort = "Amsterdam",
            LeadPassengerName = "Alice Smith",
            PassportNumber = "EF456789",
            PassengerCount = 2,
            PricePerPerson = 900m,
            TotalPrice = 1800m
        };

        var confirmation = await _provider.BookAsync(request);

        Assert.NotNull(confirmation);
        Assert.StartsWith("BK-RiverVoyage-", confirmation.BookingReference);
        Assert.Equal("confirmed", confirmation.Status);
    }
}

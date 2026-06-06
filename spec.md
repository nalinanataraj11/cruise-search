# CruiseSearch Specification

## Functional Requirements

### Search Endpoint

**Request**
```
GET /cruises/search?departurePort={port}&date={date}&cabinClass={class}&passengers={n}
```

**Query Parameters**
- `departurePort` (required): Departure port code
- `date` (required): Sailing date (ISO 8601 format)
- `cabinClass` (optional): Cabin class filter (Interior, OceanView, Balcony, Suite)
- `passengers` (optional, default: 2): Passenger count (1–4)

**Validation**
- Return 400 if `departurePort` or `date` missing
- Return 400 if `passengers` not in range 1–4
- Return 400 if `cabinClass` provided but invalid

**Response** (200 OK)
```json
{
  "results": [
    {
      "providerId": "oceanLux",
      "providerName": "OceanLux",
      "shipName": "Symphony of the Seas",
      "cabinClass": "OceanView",
      "pricePerPerson": 1200.00,
      "portFees": 150.00,
      "totalPrice": 2700.00,
      "currencyCode": "USD",
      "inclusions": ["All meals", "Entertainment", "Beverages"],
      "cancellationPolicy": "FullRefund",
      "departurePort": "Miami",
      "sailingDate": "2026-07-15",
      "passengerCount": 2
    }
  ],
  "searchTimestampUtc": "2026-06-06T10:30:00Z"
}
```

**Rules**
- Zero-availability IslandHop cabins filtered before returning
- Results sorted by total price (ascending)
- Port fees displayed separately for OceanLux
- Total price = (pricePerPerson × passengerCount) + (portFees if present)

---

### Booking Endpoint

**Request**
```
POST /cruises/book
```

**Payload**
```json
{
  "providerId": "oceanLux",
  "sailingDate": "2026-07-15",
  "cabinClass": "OceanView",
  "departurePort": "Miami",
  "leadPassengerName": "John Doe",
  "passportNumber": "AB123456",
  "nationalIdNumber": null,
  "passengerCount": 2,
  "pricePerPerson": 1200.00,
  "totalPrice": 2700.00
}
```

**Document Validation**
- ✅ Passport number required for all bookings
- ❌ National ID number must be null (reject with 422 if provided)

**Response** (200 OK)
```json
{
  "bookingReference": "BK-OceanLux-20260606-001",
  "status": "confirmed",
  "providerId": "oceanLux",
  "providerName": "OceanLux",
  "leadPassengerName": "John Doe",
  "sailingDate": "2026-07-15",
  "cabinClass": "OceanView",
  "passengerCount": 2,
  "totalPrice": 2700.00,
  "currencyCode": "USD",
  "cancellationPolicy": "FullRefund",
  "bookedAtUtc": "2026-06-06T10:35:00Z"
}
```

---

### Booking Status Endpoint

**Request**
```
GET /cruises/booking/{reference}
```

**Response** (200 OK)
```json
{
  "bookingReference": "BK-OceanLux-20260606-001",
  "status": "confirmed",
  "providerId": "oceanLux",
  "providerName": "OceanLux",
  "leadPassengerName": "John Doe",
  "sailingDate": "2026-07-15",
  "cabinClass": "OceanView",
  "passengerCount": 2,
  "totalPrice": 2700.00,
  "currencyCode": "USD",
  "cancellationPolicy": "FullRefund",
  "bookedAtUtc": "2026-06-06T10:35:00Z"
}
```

---

## Data Models

### Unified CruiseResult

```csharp
public class CruiseResult
{
    public string ProviderId { get; set; }
    public string ProviderName { get; set; }
    public string? ShipName { get; set; }
    public CabinClass CabinClass { get; set; }
    public decimal PricePerPerson { get; set; }
    public decimal? PortFees { get; set; }  // Nullable for non-OceanLux providers
    public decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public List<string> Inclusions { get; set; } = new();
    public CancellationPolicy CancellationPolicy { get; set; }
    public string DeparturePort { get; set; }
    public DateTime SailingDate { get; set; }
    public int PassengerCount { get; set; }
}
```

---

## Provider Abstraction

### ICruiseProvider Interface

```csharp
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
```

---

## Live Tweak: RiverVoyage Provider

### New Requirements

**Specialization**: River cruises (different departure ports from OceanLux/IslandHop)  
**Pricing Model**: All-inclusive flat price per cabin (NOT per person)  
**Cabin Support**: Interior and Ocean View only  
**Cancellation**: FullRefund (45+ days), NonRefundable (<45 days)  
**Port Fees**: None  
**Availability**: Boolean per cabin class

### Implementation Scope

- ✅ New `RiverVoyageProvider` implementation
- ✅ DI registration
- ❌ No changes to `ICruiseProvider` interface
- ❌ No changes to aggregation/booking logic
- ❌ No changes to existing provider implementations

---

## Definition of Done

- [ ] OceanLux port fees surfaced and displayed
- [ ] IslandHop zero-availability cabins filtered
- [ ] Pricing calculations correct (1–4 passengers)
- [ ] Document validation enforces Passport only
- [ ] Booking flow completes with reference number
- [ ] Tests cover all critical paths
- [ ] README has complete setup instructions
- [ ] No secrets committed
- [ ] Code professionally formatted and documented
- [ ] Live tweak (RiverVoyage) implemented without modifying core logic

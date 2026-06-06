# CruiseSearch: Cruise Cabin Search & Booking Platform

A multi-provider cruise cabin search and booking platform built for the SkyRoute Travel Platform.

## Overview

CruiseSearch aggregates cruise offerings from multiple providers (OceanLux, IslandHop, and extensible for more), normalizes the results, and presents a unified booking experience. Travelers can search by departure port, sailing date, and cabin class, then book with document validation.

## Features

- **Multi-Provider Aggregation**: Query OceanLux and IslandHop simultaneously with provider-specific pricing and policy models
- **Unified Cabin Search**: Standardized cabin classes (Interior, Ocean View, Balcony, Suite) across all providers
- **Flexible Pricing**: Per-person pricing with configurable passenger count (1–4), automatic total calculations
- **Port Fees Transparency**: OceanLux port fees displayed separately and included in all-in totals
- **Document Validation**: Passport requirement enforced for international sailings (client-side and server-side)
- **Booking Confirmation**: Reference number generation and booking status tracking
- **Extensible Architecture**: Provider abstraction pattern enables seamless addition of new cruise lines

## Architecture

### Technology Stack

- **Backend**: .NET 8+ (Minimal API)
- **Frontend**: Angular (TBD - framework choice)
- **Testing**: xUnit (backend), Jasmine/Karma (frontend)
- **Data Access**: In-memory stubs (deterministic hardcoded responses)

### Design Patterns

1. **Provider Abstraction Pattern**: `ICruiseProvider` interface with pluggable implementations
   - Decouples provider-specific logic from aggregation and booking orchestration
   - Enables new providers without modifying core logic
   - Each provider handles its own normalization

2. **Normalization Layer**: Unified `CruiseResult` model carries provider-specific fields (e.g., nullable PortFees)
   - Preserves fidelity while maintaining contract consistency
   - Allows frontend to make rendering decisions based on data availability

3. **Dependency Injection**: Factory pattern for provider instantiation
   - Supports multiple provider configurations
   - Facilitates testing and provider selection logic

### Functional Scope

#### Backend (.NET Minimal API)

- `GET /cruises/search` - Search across all providers with filtering
- `POST /cruises/book` - Validate documents and confirm booking
- `GET /cruises/booking/{reference}` - Retrieve booking status

#### Frontend

- Search form with port dropdown, date picker, optional cabin class filter, passenger count
- Results list with per-provider badges, sortable by total price
- Booking form with lead passenger details and document validation
- Confirmation screen with reference number and policy details

## Project Structure

```
cruise-search/
├── README.md
├── spec.md
├── CruiseSearch.Api/               # .NET Minimal API
│   ├── Program.cs
│   ├── Models/
│   ├── Services/
│   ├── Providers/
│   └── Validators/
├── CruiseSearch.Tests/             # Backend tests
│   ├── ProviderTests/
│   ├── NormalizationTests/
│   └── BookingTests/
├── cruisesearch-ui/                # Angular frontend
│   ├── src/
│   │   ├── app/
│   │   └── assets/
│   └── angular.json
├── prompts.md                      # Copilot prompts used
├── reflection.md                   # Copilot usage & decisions
└── .gitignore
```

## Providers

### OceanLux

- **Pricing**: Base price per person + mandatory port fees
- **Data**: Full detail (ship name, inclusions, cancellation policy)
- **Availability**: Always available
- **Format**: PascalCase JSON
- **Cancellation**: FullRefund (60+ days), PartialRefund (30–59 days, 50%), NonRefundable (<30 days)

### IslandHop

- **Pricing**: Lower-cost per-person pricing
- **Data**: Minimal (cabin class, price, date, policy only)
- **Availability**: May return 0 cabins (filter before returning)
- **Format**: camelCase JSON
- **Cancellation**: Flexible (30+ days) or NonRefundable

## Setup Instructions

### Prerequisites

- .NET 8+ SDK
- Node.js 18+ and npm
- Angular CLI

### Backend Setup

```bash
cd CruiseSearch.Api
dotnet restore
dotnet run
# API runs on http://localhost:5000
```

### Frontend Setup

```bash
cd cruisesearch-ui
npm install
ng serve
# UI runs on http://localhost:4200
```

### Testing

```bash
cd CruiseSearch.Tests
dotnet test
```

## Evaluation Criteria

- ✅ Code quality and organization
- ✅ Correctness and completeness of solution
- ✅ Effective and ethical use of Copilot
- ✅ Documentation and architectural decisions
- ✅ Professionalism and clarity of submission

## Copilot Usage

See `prompts.md` for a detailed list of Copilot prompts used throughout development.

See `reflection.md` for reflection on Copilot's impact on design and implementation decisions.

## Submission Status

- [ ] Backend implementation complete
- [ ] Frontend implementation complete
- [ ] All tests passing
- [ ] Documentation complete
- [ ] Live tweak scenario (RiverVoyage) tested

---

**Author**: nalinanataraj11  
**Last Updated**: June 2026

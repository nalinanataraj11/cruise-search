# Frontend Setup Guide

## Quick Start

### Prerequisites
- Node.js 18+ and npm 9+
- Angular CLI 17

### Installation

```bash
cd cruisesearch-ui
npm install
```

### Development Server

```bash
ng serve
# or
npm start
```

Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

### Build for Production

```bash
ng build --configuration production
```

The build artifacts will be stored in the `dist/` directory.

### Running Tests

```bash
ng test
```

## Architecture

### Components

#### SearchComponent (`/search`)
- Departure port selection (Miami, Amsterdam, Barcelona, Port Canaveral)
- Sailing date picker (no past dates allowed)
- Optional cabin class filter (Interior, Ocean View, Balcony, Suite)
- Passenger count selector (1–4)
- Form validation with error messages
- Loading state during search

#### ResultsComponent (`/results`)
- Displays aggregated cruise results from all providers
- Provider badges with distinct colors:
  - OceanLux (Blue)
  - IslandHop (Purple)
  - RiverVoyage (Teal)
- Sortable by:
  - Price: Low to High (default)
  - Price: High to Low
  - Provider name
- Shows:
  - Ship name (null for IslandHop)
  - Cabin class
  - Per-person price
  - Port fees (if available)
  - Total price for all passengers
  - Inclusions
  - Cancellation policy
- "Book Now" button to proceed to booking

#### BookingComponent (`/booking`)
- Cruise summary display (provider, ship, dates, passengers, total price)
- Lead passenger name input
- Passport number input (required for international sailings)
- National ID field (disabled, not accepted for international bookings)
- Form validation with error messages
- Cancel or confirm booking

#### ConfirmationComponent (`/confirmation`)
- Booking reference number display (prominently formatted)
- Booking confirmation details
- Passenger information summary
- Total price summary
- Cancellation policy reminder
- Next steps guidance (bring passport, arrive early, etc.)
- "Search Another Cruise" button for new searches

### Services

#### CruiseService
- **search()**: GET /cruises/search with query parameters
- **book()**: POST /cruises/book with booking request
- **getBookingStatus()**: GET /cruises/booking/{reference}
- Base URL: `https://localhost:5001/cruises`
- Handles HTTP requests and error responses

### Models

#### CruiseResult
- Provider info (ID, name)
- Ship details (name, cabin class)
- Pricing (per-person, port fees, total)
- Inclusions list
- Cancellation policy
- Departure port and sailing date
- Passenger count

#### BookingRequest
- Provider ID
- Sailing details (date, cabin class, port)
- Lead passenger name
- Passport number (required)
- National ID (must be null)
- Pricing information

#### BookingConfirmation
- Booking reference
- Status (confirmed)
- Full booking details
- Total price
- Cancellation policy
- Booked timestamp

## Routing

```
/                    → redirects to /search
/search              → SearchComponent (search form)
/results             → ResultsComponent (results list)
/booking             → BookingComponent (booking form)
/confirmation        → ConfirmationComponent (confirmation screen)
```

## State Management

State is passed between components using Angular Router's `navigation.extras.state`:

1. **Search → Results**: Pass search params and results array
2. **Results → Booking**: Pass selected cruise and search params
3. **Booking → Confirmation**: Pass booking confirmation
4. **Confirmation**: Offers new search (returns to /search)

## API Integration

### CORS Configuration
The Angular app runs on `http://localhost:4200` and communicates with the .NET API on `https://localhost:5001`. The backend has CORS enabled for all origins.

### Error Handling
All HTTP requests include error handling:
- Network errors display user-friendly messages
- Server validation errors (400, 422) show backend error messages
- Loading states prevent double-submission

### Document Validation
- Passport is required for all bookings (validated on form)
- National ID field is disabled and cannot be filled
- Server-side validation rejects bookings with National ID (422 status)

## Styling

### Global Styles
- Font: System fonts with fallbacks
- Color scheme: Blue primary (#0066cc), neutral grays
- Responsive layout with max-widths
- Gradient header with footer

### Component Styles
Each component has scoped SCSS styles:
- Forms with input validation styling
- Cards with shadow effects
- Buttons with hover states
- Provider badges with distinct colors
- Error messages in red with background
- Loading states disabled button appearance

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Development Notes

### Standalone Components
All components are standalone (no NgModules), following Angular 17+ best practices.

### Reactive Forms
All forms use reactive forms with FormBuilder for strong typing and validation control.

### HttpClient
API calls use Angular's HttpClient with typed responses.

### Router State
Navigation state is preserved across route changes for seamless user experience.

## Deployment

### To Production
1. Build the project: `ng build --configuration production`
2. Deploy `dist/cruisesearch-ui/` to a web server
3. Configure backend API URL in environment files if using different domain
4. Ensure CORS is configured on backend for your domain

### Environment-Specific Configuration
For production, update the API URL in `cruise.service.ts`:
```typescript
private apiUrl = 'https://api.yourdomain.com/cruises';
```

## Troubleshooting

### CORS Errors
Ensure the .NET backend is running with CORS enabled and the API URL is correct.

### Blank Results
Check that the backend is returning data for the selected search criteria.

### Booking Fails with 422
Likely caused by:
- Empty passport number
- National ID provided
- Invalid lead passenger name

Check the error message returned from the API.

### Date Validation
The date picker prevents selecting past dates. Use today's date or later.

## Contributing

- Keep components small and focused
- Use typed models for API responses
- Maintain consistent styling
- Add error handling for all API calls
- Test routing state transitions


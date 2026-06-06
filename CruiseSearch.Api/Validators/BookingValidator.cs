namespace CruiseSearch.Api.Validators;

using CruiseSearch.Api.Models;

public class BookingValidator
{
    public (bool IsValid, string? ErrorMessage) ValidateBooking(BookingRequest request)
    {
        // Passport is required for international sailings
        if (string.IsNullOrWhiteSpace(request.PassportNumber))
        {
            return (false, "Passport number is required for international sailings.");
        }

        // National ID is not accepted for international sailings
        if (!string.IsNullOrWhiteSpace(request.NationalIdNumber))
        {
            return (false, "National ID is not accepted for international sailings. Passport required.");
        }

        // Lead passenger name is required
        if (string.IsNullOrWhiteSpace(request.LeadPassengerName))
        {
            return (false, "Lead passenger name is required.");
        }

        return (true, null);
    }
}

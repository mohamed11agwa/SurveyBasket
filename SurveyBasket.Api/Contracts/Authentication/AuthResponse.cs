namespace SurveyBasket.Api.Contracts.Authentication
{
    public record AuthResponse(
        string Id,
        string? Email,
        String FirstName,
        String LastName,
        String Token,
        int ExpiresIn
        );
}

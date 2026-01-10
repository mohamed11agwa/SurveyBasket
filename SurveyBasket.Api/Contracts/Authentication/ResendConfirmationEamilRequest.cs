using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication
{
    public record ResendConfirmationEamilRequest(
        string Email
    );
}

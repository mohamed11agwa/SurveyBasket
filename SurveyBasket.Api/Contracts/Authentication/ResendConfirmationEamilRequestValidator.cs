using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication
{
    public class ResendConfirmationEamilRequestValidator: AbstractValidator<ResendConfirmationEamilRequest>
    {
        public ResendConfirmationEamilRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}

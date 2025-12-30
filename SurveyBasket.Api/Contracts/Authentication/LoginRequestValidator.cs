using FluentValidation;
namespace SurveyBasket.Api.Contracts.Authentication
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(X => X.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(X => X.Password)
                .NotEmpty();
        }
    }
}
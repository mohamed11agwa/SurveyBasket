using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(X => X.Token).NotEmpty();
            RuleFor(X => X.RefreshToken).NotEmpty();

        }
    }
}

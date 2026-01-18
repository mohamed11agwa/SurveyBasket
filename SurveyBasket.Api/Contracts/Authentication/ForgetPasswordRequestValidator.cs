using FluentValidation;
using SurveyBasket.Abstractions.Consts;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Contracts.Authentication
{
    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}

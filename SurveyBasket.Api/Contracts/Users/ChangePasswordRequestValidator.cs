using FluentValidation;
using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password Should be at least 8 digits and should contain lowerCase, UpperCase and NonAlphanumeric")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New Password Can't Be Same as the current password");

        }
    }
}


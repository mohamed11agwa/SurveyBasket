using FluentValidation;
using SurveyBasket.Api.Contracts.Requests;

namespace SurveyBasket.Api.Contracts.Validations
{
    public class PollRequestValidator:AbstractValidator<PollRequest>
    {
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.Summary)
                .NotEmpty().
                Length(3, 1500);

            RuleFor(x => x.StartsAt)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

            RuleFor(x => x.EndsAt)
                .NotEmpty();

            RuleFor(x => x)
                .Must(HasValidDates)
                .WithName(nameof(PollRequest.EndsAt))
                .WithMessage("{PropertyName} must be greater than or equal to StartsAt.");
        }

        private bool HasValidDates(PollRequest poll)
        {
            return poll.EndsAt >= poll.StartsAt;
        }
    }
}

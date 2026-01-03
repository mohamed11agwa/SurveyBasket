using FluentValidation;

namespace SurveyBasket.Api.Contracts.Votes
{
    public class VoteRequestValidator: AbstractValidator<VoteRequest>
    {
        public VoteRequestValidator()
        {
            RuleFor(x => x.Answers)
                .NotEmpty();

            //valide children
            RuleForEach(x => x.Answers)
                .SetInheritanceValidator(
                        v => v.Add (new VoteAnswerRequestValidator())
                
                );
        }



    }
}

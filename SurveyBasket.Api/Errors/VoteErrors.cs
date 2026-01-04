using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors
{
    public static class VoteErrors
    {
        public static readonly Error InvalidAnswers =
            new("Vote.InvalidAnswers", "the provided answers are invalid for this poll.", StatusCodes.Status400BadRequest);
        public static readonly Error DuplicatedVote =
            new("Vote.DuplicatedVote", "this user is already voted before for this poll.", StatusCodes.Status409Conflict);

    }
}

using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors
{
    public static class PollErrors
    {
        public static readonly Error PollNotFound =
            new("Poll.NotFound", "No Poll Was Found With The Given ID.", StatusCodes.Status404NotFound);

        public static readonly Error DuplicatedPollTitle =
            new("Poll.DuplicatedTitle", "Another poll with the same title is already exists", StatusCodes.Status409Conflict);

    }
}

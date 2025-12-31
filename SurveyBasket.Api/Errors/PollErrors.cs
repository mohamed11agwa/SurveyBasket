using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors
{
    public static class PollErrors
    {
        public static readonly Error PollNotFound =
            new("Poll.NotFound", "No Poll Was Found With The Given ID.");
    }
}

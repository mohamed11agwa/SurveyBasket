using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services
{
    public interface IVoteService
    {
        Task<Result> AddAsync(int pollId, string userId, VoteRequest vote, CancellationToken cancellationToken = default);
    }
}

using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Services
{
    public interface IPollService
    {
        Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        //Task<Poll> AddAsync (Poll poll, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default);
        //Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        //Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);

    }
}

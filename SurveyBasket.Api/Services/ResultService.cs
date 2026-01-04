using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Results;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services
{
    public class ResultService(ApplicationDbContext context) : IResultService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
        {
            
            var pollVotes = await _context.Polls
                .Where(p => p.Id == pollId)
                .Select(p => new PollVotesResponse(
                    p.Title,
                    p.Votes.Select(v => new VoteResponse(
                                $"{v.User.FirstName}  {v.User.LastName}",
                                v.SubmittedOn,
                                v.VoteAnswers.Select(va => new QuestionAnswerResponse(
                                    va.Question.Content,
                                    va.Answer.Content
                                ))
                    ))
                )).SingleOrDefaultAsync(cancellationToken);

            return pollVotes is null
            ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
            : Result.Success(pollVotes);
        }

        public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
            if (!pollExists)
                return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);
            
            var votesPerDay = await _context.Votes
                .Where(v => v.PollId == pollId)
                .GroupBy(v => new {Date = DateOnly.FromDateTime(v.SubmittedOn)})
                .Select(g => new VotesPerDayResponse(
                    g.Key.Date,
                    g.Count()
                )).ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);

        }


        public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
            if (!pollExists)
                return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

            var votesPerQuestion = await _context.VoteAnswers
                .Where(va => va.Vote.PollId == pollId)
                .Select(va => new VotesPerQuestionResponse (
                    
                        va.Question.Content,
                        va.Question.VoteAnswers.GroupBy(va => new {AnswerId =  va.Answer.Id,AnswerContent =  va.Answer.Content })
                        .Select(g => new VotesPerAnswerResponse(
                            g.Key.AnswerContent,
                            g.Count()
                        ))


                )).ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);


        }


    }
}

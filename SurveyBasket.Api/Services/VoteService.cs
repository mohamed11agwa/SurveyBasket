using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
        {
            var hasVote = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
            if (hasVote)
                return Result.Failure(VoteErrors.DuplicatedVote);

            var PollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId
                    && p.IsPublished
                    && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)
                    , cancellationToken);
            if (!PollIsExists)
                return Result.Failure(PollErrors.PollNotFound);
            //
            var AvailableQuestions = await _context.Questions.Where(q => q.PollId == pollId && q.IsActive)
                .Select(q => q.Id)
                .ToListAsync(cancellationToken);
            if(!request.Answers.Select(a =>a.QuestionId).SequenceEqual(AvailableQuestions))
                return Result.Failure(VoteErrors.InvalidAnswers);
            
            var vote = new Vote()
            {
                PollId = pollId,
                UserId = userId,
                SubmittedOn = DateTime.UtcNow,
                VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
                //VoteAnswers = request.Answers.Select(a => new VoteAnswer
                //{
                //    QuestionId = a.QuestionId,
                //    AnswerId = a.AnswerId,
                //    VoteId = vote.Id
                //}).ToList()
            };
            await _context.Votes.AddAsync(vote, cancellationToken);
            await  _context.SaveChangesAsync(cancellationToken);
            return Result.Success();



        }
    }
}

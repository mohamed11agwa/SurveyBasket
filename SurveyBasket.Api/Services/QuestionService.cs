using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Persistence;
using System.Threading;

namespace SurveyBasket.Api.Services
{
    public class QuestionService(ApplicationDbContext context) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken = default)
        {
            var PollIsExists = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);
            if (!PollIsExists)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
            var questions = await _context.Questions
                .Where(q => q.PollId == PollId)
                .Include(q => q.Answers)
                //.Select(q => new QuestionResponse(
                //    q.Id,
                //    q.Content,
                //    q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
                //))
                .ProjectToType<QuestionResponse>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }


        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int PollId, string UserId, CancellationToken cancellationToken = default)
        {
            var hasVote = await _context.Votes.AnyAsync(v => v.PollId == PollId && v.UserId == UserId, cancellationToken);
            if(hasVote)
                return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);
            var PollIsExists = await _context.Polls.AnyAsync(p => p.Id == PollId 
                    && p.IsPublished
                    && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)
                    , cancellationToken);
            if (!PollIsExists)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
            // 
            var questions = await _context.Questions
                .Where(q => q.PollId == PollId && q.IsActive)
                .Include(q => q.Answers)
                .Select(a => new QuestionResponse
                (
                    a.Id,
                    a.Content,
                    a.Answers.Where(ans => ans.IsActive).Select(ans => new AnswerResponse(ans.Id, ans.Content))

                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }


        public async Task<Result<QuestionResponse>> GetByIdAsync(int PollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions
                .Where(q => q.PollId == PollId && q.Id == id)
                .Include(q => q.Answers)
                .ProjectToType<QuestionResponse>()
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (question == null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            return Result.Success(question);

        }



        public async Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var PollIsExists = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);
            if(!PollIsExists)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var QuestionIsExists = await _context.Questions.AnyAsync(q => q.PollId == PollId && q.Content == request.Content, cancellationToken);

            if(QuestionIsExists)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

            var question = request.Adapt<Question>();
            question.PollId = PollId;

            //request.Answers.ForEach(answer =>
            //    question.Answers.Add(new Answer { Content = answer})
            //);

            await _context.Questions.AddAsync(question, cancellationToken);
            await  _context.SaveChangesAsync(cancellationToken);

            return Result.Success(question.Adapt<QuestionResponse>());
        }



        public async Task<Result> UpdateAsync(int PollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var questionIsExists = await _context.Questions.AnyAsync(q => q.PollId == PollId
            && q.Id != id
            && q.Content == request.Content,
            cancellationToken);
            if (questionIsExists)
                return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

            var question = await _context.Questions.Include(q => q.Answers)
                .SingleOrDefaultAsync(q => q.PollId == PollId && q.Id == id, cancellationToken);
            if (question == null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.Content = request.Content;

            //current answers
            var currentAnswers = question.Answers.Select(a => a.Content).ToList();

            //add new answers
            var newAnswers = request.Answers.Except(currentAnswers).ToList();

            newAnswers.ForEach(answer =>
                question.Answers.Add(new Answer { Content = answer })
            );
            question.Answers.ToList().ForEach(answer => {
                answer.IsActive = request.Answers.Contains(answer.Content);
            });

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }


        public async Task<Result> ToggleStatusAsync(int PollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions
                .SingleOrDefaultAsync(q => q.PollId == PollId && q.Id == id, cancellationToken);

            if (question == null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.IsActive = !question.IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }

        
    }
}

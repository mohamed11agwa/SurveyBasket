
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Helpers;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services
{
    public class NotificationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager
        ,IHttpContextAccessor httpContextAccessor, IEmailSender emailSender, ILogger<NotificationService> logger ) : INotificationService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<NotificationService> _logger = logger;

        public async Task SendNewPollsNotifications(int? pollId)
        {
            IEnumerable<Poll> polls = [];

            if (pollId.HasValue)
            {
                var poll = await _context.Polls.SingleOrDefaultAsync(x => x.Id == pollId && x.IsPublished);

                polls = [poll!];
            }
            else
            {
                polls = await _context.Polls
                    .Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                    .AsNoTracking()
                    .ToListAsync();
            }
            
            _logger.LogInformation("Polls Count = {PollsCount}", polls.Count());

            //TODO: Select members only
            var users = await _userManager.Users.ToListAsync();

            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            foreach (var poll in polls)
            {
                foreach (var user in users)
                {
                    var placeholders = new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName },
                    { "{{pollTill}}", poll.Title },
                    { "{{endDate}}", poll.EndsAt.ToString() },
                    { "{{url}}", $"{origin}/polls/start/{poll.Id}" }
                };

                    var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholders);

                    await _emailSender.SendEmailAsync(user.Email!, $"Survey Basket: New Poll - {poll.Title}", body);
                }
            }

        }
    }
}

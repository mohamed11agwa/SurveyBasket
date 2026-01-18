namespace SurveyBasket.Api.Services
{
    public interface INotificationService
    {
        Task SendNewPollsNotifications(int? pollId);
    }
}

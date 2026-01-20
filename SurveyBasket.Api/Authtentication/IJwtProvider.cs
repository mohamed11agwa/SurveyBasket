using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Authtentication
{
    public interface IJwtProvider
    {
        (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles,
            IEnumerable<string> permissions);
        string? ValidateToken(string token);
    }
}

using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors
{
    public static class UserErrors
    {
        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Invalid email or password.", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedEmail =
            new("User.DuplicatedEmail", "Another User with the same email is already exists", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed =
            new("User.EmailNotConfirmed", "Email is Not Confirmed", StatusCodes.Status409Conflict);

        public static readonly Error InvalidCode =
            new("User.InvalidCode", "Invalid Code", StatusCodes.Status409Conflict);

        public static readonly Error DuplicatedEmailConfirmation =
            new("User.DuplicatedEmailConfirmation", "email Already Confirmed", StatusCodes.Status400BadRequest);


    }
}

using Microsoft.AspNetCore.Identity;
using OneOf;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authtentication;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly int _refreshTokenExpirationDays = 14;

        public async Task<OneOf<AuthResponse, Error>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            //Check USER
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                //return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
                return UserErrors.InvalidCredentials;

            //Check PASSWORD
            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!isValidPassword)
                //return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
                return UserErrors.InvalidCredentials;

            //Generate jwt TOKEN
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

            //Generate REFRESH TOKEN
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration,
            }); 
            await _userManager.UpdateAsync(user);

            //Generate Refresh token and return it with jwt token during login
            var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
            //return Result.Success(response);
            return response;

        }


        public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return null;
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;
            var existingRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

            if (existingRefreshToken is null)
                return null;
            existingRefreshToken.RevokedOn = DateTime.UtcNow;


            //Generate new jwt TOKEN
            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiration,
            });
            await _userManager.UpdateAsync(user);
            
            return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        }



        public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return false;
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return false;
            var existingRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

            if (existingRefreshToken is null)
                return false;
            existingRefreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            
            return true;
        }


        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}

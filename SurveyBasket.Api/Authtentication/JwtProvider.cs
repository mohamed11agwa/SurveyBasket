using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authtentication
{
    public class JwtProvider : IJwtProvider
    {
        public (string token, int expiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [

                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
           ];
            var symmertricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("WYlOvfY6fLwGSShPVnlQveCRfBho5CRp"));
            var signingCredentials = new SigningCredentials(symmertricSecurityKey, SecurityAlgorithms.HmacSha256);

            var expiresIn = 30; // Token expiration time in minutes

            var expirationDate = DateTime.UtcNow.AddMinutes(expiresIn);

            var token = new JwtSecurityToken(
                issuer: "SurveyBasketApp",
                audience: "SurveyBasketApp Users",
                claims: claims,
                expires: expirationDate,
                signingCredentials: signingCredentials
                );
            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn);

        }
    }
}

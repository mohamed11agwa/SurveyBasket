using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authtentication;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        [HttpPost("")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request, CancellationToken cancellationToken)
        {
            
            var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            //if (authResult.IsSuccess)
            //    return Ok(authResult.Value);
            //return BadRequest(authResult.Error);

            //return authResult.Match(
            //    AuthResponse => Ok(AuthResponse),
            //    error => Problem(statusCode: StatusCodes.Status400BadRequest, title: error.Code, detail: error.Description)
            //);

            return authResult.IsSuccess
                ? Ok(authResult.Value)
                : authResult.ToProblem();
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            //if (authResult is null)
            //    return BadRequest("Invalid Token.");
            //return Ok(authResult);
            return authResult.IsSuccess
                ? Ok(authResult.Value)
                //: Problem(statusCode: StatusCodes.Status400BadRequest, title: authResult.Error.Code, detail: authResult.Error.Description);
                : authResult.ToProblem();

        }



        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            //if (isRevoked)
            //    return Ok();
            // else
            //    return BadRequest("Operation Failed.");
            return result.IsSuccess
                ? Ok()
                //: Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);
                : result.ToProblem();
        }


    }
}

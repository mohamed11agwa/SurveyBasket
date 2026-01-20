using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Abstractions.Consts;
using System.Net;

namespace SurveyBasket.Api.Authtentication.Filters
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = context.User.Identity;
            if (user is null || !user.IsAuthenticated)
                return;
            var hasPermission = context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type);
            if (!hasPermission)
                return;

            context.Succeed(requirement);
            return;

        }
    }
}

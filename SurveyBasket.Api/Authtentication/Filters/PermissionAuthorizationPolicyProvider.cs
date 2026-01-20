using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SurveyBasket.Api.Authtentication.Filters
{
    public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
                    : DefaultAuthorizationPolicyProvider(options)
    {
        private readonly AuthorizationOptions _authorizationoptions = options.Value;

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if(policy is not null)
                return policy;
            var PermissionPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName)).Build();

            _authorizationoptions.AddPolicy(policyName, PermissionPolicy);
            return PermissionPolicy;

        }
    }
}

using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Api.Authtentication.Filters
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {

    }
}

using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Abstractions.Consts;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authtentication.Filters;
using SurveyBasket.Api.Contracts.Roles;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        [HttpGet("")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAllRoles([FromQuery] bool includeDisabled, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetAllRolesAsync(includeDisabled, cancellationToken);
            return Ok(roles);
        }


        [HttpGet("{id}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _roleService.GetAsync(id);
            return result.IsSuccess? Ok(result.Value) : result.ToProblem();
        }



        [HttpPost("")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> AddRole([FromBody] RoleRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleService.AddRoleAsync(request,cancellationToken );
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new {result.Value.Id}, result.Value) : result.ToProblem();
        }


        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> UpdateRole([FromRoute] string id, [FromBody] RoleRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleService.UpdateRoleAsync(id ,request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

    }
}

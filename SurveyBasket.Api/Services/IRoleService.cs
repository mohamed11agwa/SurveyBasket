using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);

        Task<Result<RoleDetailResponse>> GetAsync(string id);

        Task<Result<RoleDetailResponse>> AddRoleAsync(RoleRequest request, CancellationToken cancellationToken = default);

        Task<Result> UpdateRoleAsync(string id, RoleRequest request, CancellationToken cancellationToken);
    }
}

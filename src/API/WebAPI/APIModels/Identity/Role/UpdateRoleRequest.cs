namespace WebAPI.APIModels.Identity.Role
{
    public record UpdateRoleRequest(string Name, List<Guid>? ActionIds);
}

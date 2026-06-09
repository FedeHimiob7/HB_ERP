namespace WebAPI.APIModels.Identity.Authentication.User
{
    public class AssignRoleRequest
    {
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}

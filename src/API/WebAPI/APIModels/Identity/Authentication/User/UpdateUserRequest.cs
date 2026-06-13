namespace WebAPI.APIModels.Identity.Authentication.User
{
    public record UpdateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        List<Guid>? RoleIds,
        List<Guid>? PslIds
    );
}

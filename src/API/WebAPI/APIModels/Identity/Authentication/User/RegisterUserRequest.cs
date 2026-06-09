namespace WebAPI.APIModels.Identity.Authentication.User
{
    public record RegisterUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password
    );
}

namespace WebAPI.APIModels.Identity.Authentication.User.Request
{
    public record RegisterUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password
    );
}

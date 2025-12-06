namespace WebAPI.DTOs.User.Request
{
    public record RegisterUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password
    );
}

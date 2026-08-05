namespace Identity.Domain.Enums
{
    public enum EventLogType
    {
        LoginSucceeded = 1,
        LoginFailed = 2,
        UserRegistered = 3,
        UserUpdated = 4,
        UserDeactivated = 5,
        UserPasswordReset = 6,
        RoleCreated = 7,
        RoleUpdated = 8,
        RoleDeactivated = 9,
        ActionAssignedToRole = 10,
        SystemActionCreated = 11,
        SystemActionUpdated = 12,
        SystemActionDeactivated = 13,
    }
}

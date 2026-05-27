namespace WebAPI.APIModels.MasterData.State
{
    public record UpdateStateRequest(Guid CountryId, string Code, string Name);
}

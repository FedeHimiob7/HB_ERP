namespace WebAPI.APIModels.MasterData.State
{
    public record CreateStateRequest(Guid CountryId, string Code, string Name);
}

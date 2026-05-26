namespace WebAPI.APIModels.MasterData.ProductServiceLine
{    
        public record CreateProductServiceLineRequest(string Name, string Description);

        public record UpdateProductServiceLineRequest(string Name, string Description);
}

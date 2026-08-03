using ErrorOr;
using Inventory.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductTests
    {
        private static readonly ProductServiceLineId SamplePslId = ProductServiceLineId.New();
        private static readonly CurrencyId SampleCurrencyId = CurrencyId.New();

        private static ErrorOr<Product> CreateProduct(
            string code = "20260731-1-1",
            string name = "Producto de prueba",
            decimal? cost = 100m,
            decimal? price = 150m,
            ProductServiceLineId? pslId = null)
        {
            return Product.Create(
                code,
                itemNumberByDay: 1,
                name,
                pslId ?? SamplePslId,
                cost,
                SampleCurrencyId,
                costExchangeRate: 150.25m,
                price,
                SampleCurrencyId,
                priceExchangeRate: 150.25m,
                isSalable: true,
                isPurchasable: true,
                isStored: true);
        }

        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = CreateProduct();

            Assert.False(result.IsError);
            Assert.Equal("20260731-1-1", result.Value.Code);
            Assert.Equal("Producto de prueba", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankCode_Fails(string code)
        {
            var result = CreateProduct(code: code);

            Assert.True(result.IsError);
            Assert.Equal("Product.CodeIsRequired", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = CreateProduct(name: name);

            Assert.True(result.IsError);
            Assert.Equal("Product.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void Create_WithEmptyProductServiceLineId_Fails()
        {
            var result = CreateProduct(pslId: new ProductServiceLineId(Guid.Empty));

            Assert.True(result.IsError);
            Assert.Equal("Product.InvalidProductServiceLine", result.FirstError.Code);
        }

        [Fact]
        public void Create_WithNegativeCost_Fails()
        {
            var result = CreateProduct(cost: -1m);

            Assert.True(result.IsError);
            Assert.Equal("Product.NegativeCost", result.FirstError.Code);
        }

        [Fact]
        public void Create_WithNegativePrice_Fails()
        {
            var result = CreateProduct(price: -1m);

            Assert.True(result.IsError);
            Assert.Equal("Product.NegativePrice", result.FirstError.Code);
        }

        [Fact]
        public void Create_TrimsCodeAndName()
        {
            var result = CreateProduct(code: "  20260731-1-1  ", name: "  Producto de prueba  ");

            Assert.False(result.IsError);
            Assert.Equal("20260731-1-1", result.Value.Code);
            Assert.Equal("Producto de prueba", result.Value.Name);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var product = CreateProduct().Value;

            var result = product.UpdateDetails(
                name: "Nuevo nombre",
                description: "Nueva descripción",
                model: "Modelo X",
                barcode: "123456789",
                supplierCode: "SUP-1",
                productTypeId: null,
                productCategoryId: null,
                productSubCategoryId: null,
                productBrandId: null,
                isSalable: false,
                isPurchasable: false,
                isStored: false,
                purchaseUnitId: null,
                saleUnitId: null,
                unitConversionFactor: null,
                weight: 10m,
                volume: 5m,
                contentCapacity: 2m,
                tags: "tag1,tag2",
                imageUrl: "http://example.com/image.png",
                profitMargin: 20m);

            Assert.False(result.IsError);
            Assert.Equal("Nuevo nombre", product.Name);
            Assert.Equal("Nueva descripción", product.Description);
            Assert.Equal("Modelo X", product.Model);
            Assert.Equal("123456789", product.Barcode);
            Assert.False(product.IsSalable);
            Assert.False(product.IsPurchasable);
            Assert.False(product.IsStored);
            Assert.Equal(10m, product.Weight);
            Assert.Equal(20m, product.ProfitMargin);
        }

        [Fact]
        public void UpdateDetails_WithBlankName_Fails()
        {
            var product = CreateProduct().Value;

            var result = product.UpdateDetails(
                name: "",
                description: null,
                model: null,
                barcode: null,
                supplierCode: null,
                productTypeId: null,
                productCategoryId: null,
                productSubCategoryId: null,
                productBrandId: null,
                isSalable: true,
                isPurchasable: true,
                isStored: true,
                purchaseUnitId: null,
                saleUnitId: null,
                unitConversionFactor: null,
                weight: null,
                volume: null,
                contentCapacity: null,
                tags: null,
                imageUrl: null,
                profitMargin: null);

            Assert.True(result.IsError);
            Assert.Equal("Product.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateCode_WithValidCode_UpdatesCode()
        {
            var product = CreateProduct().Value;

            product.UpdateCode("NUEVO-CODIGO");

            Assert.Equal("NUEVO-CODIGO", product.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateCode_WithBlankCode_DoesNotChangeCode(string blankCode)
        {
            var product = CreateProduct().Value;
            var originalCode = product.Code;

            product.UpdateCode(blankCode);

            Assert.Equal(originalCode, product.Code);
        }

        [Fact]
        public void SetTaxes_ReplacesExistingTaxIds()
        {
            var product = CreateProduct().Value;
            var firstTaxId = TaxId.New();
            var secondTaxId = TaxId.New();

            product.SetTaxes(new[] { firstTaxId }, new[] { firstTaxId, secondTaxId });

            Assert.Single(product.PurchaseTaxIds);
            Assert.Equal(2, product.SaleTaxIds.Count);

            var thirdTaxId = TaxId.New();
            product.SetTaxes(new[] { thirdTaxId }, Array.Empty<TaxId>());

            Assert.Single(product.PurchaseTaxIds);
            Assert.Equal(thirdTaxId, product.PurchaseTaxIds[0]);
            Assert.Empty(product.SaleTaxIds);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var product = CreateProduct().Value;

            product.Deactivate();
            Assert.False(product.IsActive);

            product.Activate();
            Assert.True(product.IsActive);
        }
    }
}

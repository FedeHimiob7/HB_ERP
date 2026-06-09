using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Events;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Entities
{
    public sealed class ProductServiceLine : AggregateRoot<ProductServiceLineId>
    {
        private ProductServiceLine() { }

        private ProductServiceLine(ProductServiceLineId id, string description, string name, bool isActive)
            : base(id)
        {
            Description = description;
            Name = name;
            IsActive = isActive;
        }

        public string Description { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public static ErrorOr<ProductServiceLine> Create(string description, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ProductServiceLineErrors.NameIsRequired;

            var productServiceLine = new ProductServiceLine(
                ProductServiceLineId.New(),
                description,
                name,
                isActive: true);

            productServiceLine.Raise(new ProductServiceLineCreatedDomainEvent(
                productServiceLine.Id,
                productServiceLine.Description,
                productServiceLine.Name));

            return productServiceLine;
        }

        public static ProductServiceLine CreateExisting(Guid id, string description, string name, bool isActive)
        {
            return new ProductServiceLine(
                ProductServiceLineId.Create(id),
                description,
                name,
                isActive);
        }

        public ErrorOr<Success> UpdateDetails(string description, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ProductServiceLineErrors.NameIsRequired;

            Description = description;
            Name = name;

            return Result.Success;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}

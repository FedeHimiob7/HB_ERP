using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Enums;
using MasterData.Domain.Events;
using MasterData.Domain.VO;
using System.Text.RegularExpressions;

namespace MasterData.Domain.Entities
{
    public sealed class Company : AggregateRoot<CompanyId>
    {
        private static readonly Regex RifPattern = new(@"^[VEJPG]-\d{8,9}-\d$", RegexOptions.Compiled);

        private Company() { }

        private Company(CompanyId id, string rif, string legalName, string registeredAddress, TaxpayerType taxpayerType)
            : base(id)
        {
            Rif = rif;
            LegalName = legalName;
            RegisteredAddress = registeredAddress;
            TaxpayerType = taxpayerType;
        }

        public string Rif { get; private set; } = string.Empty;
        public string LegalName { get; private set; } = string.Empty;
        public string RegisteredAddress { get; private set; } = string.Empty;
        public TaxpayerType TaxpayerType { get; private set; }

        public static ErrorOr<Company> Create(string rif, string legalName, string registeredAddress, TaxpayerType taxpayerType)
        {
            var validation = Validate(rif, legalName, registeredAddress);
            if (validation.IsError) return validation.Errors;

            var company = new Company(CompanyId.Singleton, rif.ToUpper(), legalName.Trim(), registeredAddress.Trim(), taxpayerType);

            company.Raise(new CompanyCreatedDomainEvent(company.Id, company.Rif, company.LegalName));

            return company;
        }

        public static Company CreateExisting(string rif, string legalName, string registeredAddress, TaxpayerType taxpayerType)
        {
            return new Company(CompanyId.Singleton, rif, legalName, registeredAddress, taxpayerType);
        }

        public ErrorOr<Success> UpdateDetails(string rif, string legalName, string registeredAddress, TaxpayerType taxpayerType)
        {
            var validation = Validate(rif, legalName, registeredAddress);
            if (validation.IsError) return validation.Errors;

            Rif = rif.ToUpper();
            LegalName = legalName.Trim();
            RegisteredAddress = registeredAddress.Trim();
            TaxpayerType = taxpayerType;

            return Result.Success;
        }

        private static ErrorOr<Success> Validate(string rif, string legalName, string registeredAddress)
        {
            if (string.IsNullOrWhiteSpace(rif)) return CompanyErrors.RifIsRequired;
            if (!RifPattern.IsMatch(rif.ToUpper())) return CompanyErrors.InvalidRifFormat;
            if (string.IsNullOrWhiteSpace(legalName)) return CompanyErrors.LegalNameIsRequired;
            if (string.IsNullOrWhiteSpace(registeredAddress)) return CompanyErrors.RegisteredAddressIsRequired;

            return Result.Success;
        }
    }
}

using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Identity.Domain.VO
{
    public sealed record ActionName
    {
        public string Value { get; }

        private ActionName(string value)
        {
            Value = value;
        }

        public static ActionName Create(string value)
        {
            Guard.Against.NullOrWhiteSpace(value, nameof(ActionName));

            var trimmedValue = value.Trim();

            Guard.Against.LengthOutOfRange(trimmedValue, 3, 100, nameof(ActionName),
                "El nombre de la acción debe tener entre 3 y 100 caracteres.");

            // Regla de negocio: Formato (opcional, ajustado al estándar de nomenclatura)
            // Permite letras, números, guiones y puntos (ej. "Users.Create", "Invoices-View")
            if (!Regex.IsMatch(trimmedValue, @"^[a-zA-Z0-9\-\.]+$"))
            {
                throw new ArgumentException("El nombre de la acción contiene caracteres inválidos.", nameof(ActionName));
            }

            return new ActionName(trimmedValue);
        }
        
        public override string ToString() => Value;
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
    {
        public UpdateUnitCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        }
    }
}

using Clay.SmartDoor.Core.Dtos;
using FluentValidation;

namespace Clay.SmartDoor.Core.Models.Validators
{
    public class CreateDoorValidator : AbstractValidator<CreateDoorRecord>
    {
        public CreateDoorValidator()
        {
            RuleFor(d => d.Floor)
                .NotNull().WithMessage("Field is required")
                .NotEmpty().WithMessage("Field is required");
            RuleFor(d => d.Building)
                .NotNull().WithMessage("Field is required")
                .NotEmpty().WithMessage("Field is required");
            RuleFor(d => d.NameTag)
                .NotNull().WithMessage("Field is required")
                .NotEmpty().WithMessage("Field is required");
            RuleFor(d => d.CreatorId)
                .NotNull().WithMessage("Field is required")
                .NotEmpty().WithMessage("Field is required");
        }
    }
}

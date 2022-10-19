using Clay.SmartDoor.Core.DTOs.Doors;
using FluentValidation;

namespace Clay.SmartDoor.Core.Models.Validators.DoorValidators
{
    public class DoorAccessRequestValidator : AbstractValidator<DoorAccessRequest>
    {
        public DoorAccessRequestValidator()
        {
            RuleFor(x => x.DoorId)
                .NotEmpty().WithMessage("DoorId is Required")
                .NotNull().WithMessage("DoorId is Required");

            RuleFor(x => x.AccessGroupId)
                .NotEmpty().WithMessage("AccessGroupId is Required")
                .NotNull().WithMessage("AccessGroupId is Required");
        }
    }
}

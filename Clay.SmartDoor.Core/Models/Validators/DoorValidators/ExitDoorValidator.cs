using Clay.SmartDoor.Core.DTOs.Doors;
using FluentValidation;

namespace Clay.SmartDoor.Core.Models.Validators.DoorValidators
{
    public class ExitDoorValidator : AbstractValidator<ExitDoor>
    {
        public ExitDoorValidator()
        {
            RuleFor(x => x.DoorId)
                .NotEmpty().WithMessage("DoorId is Required")
                .NotNull().WithMessage("DoorId is Required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is Required")
                .NotNull().WithMessage("UserId is Required");
        }
    }
}

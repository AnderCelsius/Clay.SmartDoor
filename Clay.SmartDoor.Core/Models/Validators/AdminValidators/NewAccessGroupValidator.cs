using Clay.SmartDoor.Core.DTOs.Admin;
using FluentValidation;

namespace Clay.SmartDoor.Core.Models.Validators.AdminValidators
{
    public class NewAccessGroupValidator : AbstractValidator<NewAccessGroup>
    {
        public NewAccessGroupValidator()
        {
            RuleFor(grp => grp.GroupName)
                .NotEmpty().WithMessage("GroupName is Required")
                .NotNull().WithMessage("GroupName is Required");
        }
    }
}

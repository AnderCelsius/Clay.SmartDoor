using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.Extensions;
using FluentValidation;

namespace Clay.SmartDoor.Core.Models.Validators.AdminValidators
{
    public class NewUserRequestValidator : AbstractValidator<NewUserRequest>
    {
        public NewUserRequestValidator()
        {

            RuleFor(user => user.FirstName).HumanName();

            RuleFor(user => user.LastName).HumanName();

            RuleFor(user => user.Email).EmailAddress();

            RuleFor(user => user.Password).Password();

            RuleFor(user => user.AccessGroupId)
                .NotEmpty().WithMessage("AccessGroupId is Required")
                .NotNull().WithMessage("AccessGroupId is Required");
        }
    }
}

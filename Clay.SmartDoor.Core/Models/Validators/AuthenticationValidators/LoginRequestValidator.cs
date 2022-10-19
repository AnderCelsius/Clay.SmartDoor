using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Extensions;
using FluentValidation;

namespace Clay.SmartDoor.Core.Models.Validators.AuthenticationValidators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(user => user.Email).EmailAddress();

            RuleFor(user => user.Password).Password();
        }
    }
}

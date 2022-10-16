using FluentValidation;

namespace Clay.SmartDoor.Core.Extensions
{
    public static class IRuleBuilderExtension
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder.NotNull().WithMessage("Password is required")
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must contain at least 6 characters")
                .Matches("[A-Z]").WithMessage("Password must contain atleast 1 uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain atleast 1 lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain a number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain non alphanumeric");
            return options;
        }
        public static IRuleBuilder<T, string> HumanName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder.NotNull().WithMessage("Name cannot be null")
                .NotEmpty().WithMessage("Name must be provided")
                .Matches("[A-Za-z]").WithMessage("Name can only contain alphabeths")
                .MinimumLength(2).WithMessage("Name is limited to a minimum of 2 characters")
                .MaximumLength(50).WithMessage("Name is limited to a maximum of 25 characters");
            return options;
        }
        public static IRuleBuilder<T, string> IdGuidString<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder.Matches(@"^[a-z0-9]+-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]+$")
                .WithMessage("Value passed does not match an ID property");
            return options;
        }
    }
}

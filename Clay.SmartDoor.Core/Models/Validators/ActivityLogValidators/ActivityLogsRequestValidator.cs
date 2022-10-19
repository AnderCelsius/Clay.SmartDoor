using Clay.SmartDoor.Core.DTOs.ActivityLogs;
using FluentValidation;

namespace Clay.SmartDoor.Core.Models.Validators.ActivityLogValidators
{
    public class ActivityLogsRequestValidator : AbstractValidator<ActivityLogsRequest>
    {
        public ActivityLogsRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is Required")
                .NotNull().WithMessage("UserId is Required");

            RuleFor(x => x.FromDate)
                .NotEmpty().WithMessage("FromDate is Required")
                .NotNull().WithMessage("FromDate is Required");

            RuleFor(x => x.ToDate)
                .NotEmpty().WithMessage("ToDate is Required")
                .NotNull().WithMessage("ToDate is Required");
        }
    }
}

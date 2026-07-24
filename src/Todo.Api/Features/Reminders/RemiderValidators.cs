using FluentValidation;

namespace Todo.Api.Features.Reminders
{
    public class SnoozeReminderRequestValidator
    : AbstractValidator<SnoozeReminderRequest>
    {
        public SnoozeReminderRequestValidator()
        {
            RuleFor(x => x.Minutes)
                .GreaterThan(0)
                .LessThanOrEqualTo(7 * 24 * 60);
        }
    }
}

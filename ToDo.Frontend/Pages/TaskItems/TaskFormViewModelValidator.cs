using FluentValidation;
using Syncfusion.Blazor.Charts;
namespace ToDo.Frontend.Pages.TaskItems
{
    public class TaskFormViewModelValidator : AbstractValidator<TaskFormViewModel>
    {
        public TaskFormViewModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Заголовок обязателен")
                .MaximumLength(250).WithMessage("Максимум 250 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Максимум 1000 символов");

            RuleFor(x => x.DateRange)
                .NotNull().WithMessage("Укажите период")
                .Must(r => r!.End >= r.Start)
                .WithMessage("Конечная дата не может быть раньше начальной");

            When(x => !x.IsAllDay, () =>
            {
                RuleFor(x => x.StartTime)
                    .NotNull().WithMessage("Укажите время начала");

                RuleFor(x => x.EndTime)
                    .NotNull().WithMessage("Укажите время окончания");

                RuleFor(x => x)
                    .Must(x =>
                    {
                        var start = x.StartDate!.Value.Date + x.StartTime!.Value;
                        var end = x.EndDate!.Value.Date + x.EndTime!.Value;
                        return end >= start;
                    })
                    .WithMessage("Конечное время должно быть позже или равно начальному");
            });

            RuleFor(x => x.DurationBlocks)
                .InclusiveBetween(0, 16).WithMessage("Длительность вне допустимого диапазона");

            // и т. д. для остальных полей...
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<TaskFormViewModel>.CreateWithOptions((TaskFormViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

}

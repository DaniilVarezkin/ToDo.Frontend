using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Syncfusion.Blazor;
using ToDo.Frontend.Common.Exceptions;
using ToDo.Frontend.Models;
using ToDo.Frontend.Services.TaskItems;

namespace ToDo.Frontend.Pages.TaskItems
{
    public class FormTaskViewModel : BaseComponent
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] NavigationManager Nav { get; set; }
        [Inject] ITaskItemsService TaskItemsService { get; set; }

        [Parameter] public Guid? Id { get; set; }
        [Parameter] public DateTime? Date { get; set; }

        protected MudForm _form = null!;
        protected TaskFormModel _model = new();

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
            {
                // Редактирование
                var dto = await TaskItemsService.GetAsync(Id.Value);
                if (dto == null)
                {
                    Snackbar.Add("Задача не найдена", Severity.Error);
                    Nav.NavigateTo("/tasks");
                    return;
                }
                _model = new TaskFormModel(dto);
            }
            else if (Date.HasValue)
            {
                Console.WriteLine("-----------");
                _model.StartDate = Date;
            }
            else
            {
                _model.StartDate = DateTime.Today;
            }

            _model.StartTime = TimeSpan.FromHours(9);

            var start = _model.StartDate.Value.Date + _model.StartTime.Value;
            var end = start.AddMinutes(_model.DurationMinutes);
            _model.EndDate = end.Date;
            _model.EndTime = end.TimeOfDay;

            

            // И только теперь безопасно создаём DateRange
            if (_model.StartDate.HasValue && _model.EndDate.HasValue)
                _model.DateRange = new DateRange(
                    _model.StartDate.Value,
                    _model.EndDate.Value
                );
        }

        protected async Task SubmitAsync()
        {
            await _form.Validate();
            if (!_form.IsValid) return;

            var startLocal = _model.StartDate!.Value.Date + _model.StartTime!.Value;
            var endLocal = _model.EndDate!.Value.Date + _model.EndTime!.Value;
            var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(endLocal, DateTimeKind.Utc);

            if (_model.Id.HasValue)
            {
                var upd = _model.ToUpdateDto(startUtc, endUtc);
                try
                {
                    await TaskItemsService.UpdateAsync(upd);
                    Snackbar.Add("Задача успешно обновлена", Severity.Success);
                    Nav.NavigateTo("/tasks");
                }
                catch (ServerException ex)
                {

                    if (ex.ProblemDetails is ValidationProblemDetails pd)
                    {
                        pd.Errors.Select(pair =>
                        {
                            Snackbar.Add($"Ошибка {pair.Key}: {string.Join(", ", pair.Value)}", Severity.Error);
                            return ";";
                        });
                    }
                    Snackbar.Add($"Ошибка при сохранении: {ex.Message}", Severity.Error);
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Ошибка при сохранении: {ex.Message}", Severity.Error);
                }
            }
            else
            {
                var cr = _model.ToCreateDto(startUtc, endUtc);
                try
                {
                    await TaskItemsService.CreateAsync(cr);
                    Snackbar.Add("Задача успешно создана", Severity.Success);
                    Nav.NavigateTo("/tasks");
                }
                catch (ServerException ex)
                {

                    if (ex.ProblemDetails is ValidationProblemDetails pd)
                    {
                        pd.Errors.Select(pair =>
                        {
                            Snackbar.Add($"Ошибка {pair.Key}: {string.Join(", ", pair.Value)}", Severity.Error);
                            return ";";
                        });
                    }
                    Snackbar.Add($"Ошибка при создании1: {ex.Message}", Severity.Error);
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Ошибка при создании2: {ex.Message}", Severity.Error);
                }
            }
        }

        protected void OnIsAllDayChanged(bool value)
        {
            _model.IsAllDay = value;
        }

        protected string? ValidateEndTime(TimeSpan? endTime)
        {
            if (endTime == null)
                return "Укажите время окончания";

            var start = _model.StartDate!.Value.Date + _model.StartTime!.Value;
            var finish = _model.EndDate!.Value.Date + endTime.Value;
            return finish < start
                ? "Конечное время должно быть позже или равно начальному"
                : null;
        }

        protected void OnCancel(MouseEventArgs args)
        {
            Nav.NavigateTo("tasks/");
        }
    }
}

using MudBlazor;
using System.ComponentModel.DataAnnotations;
using ToDo.Shared.Dto.TaskItems;
using ToDo.Shared.Enums;

namespace ToDo.Frontend.Pages.TaskItems
{
    public class TaskFormViewModel
    {
        public Guid? Id { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsAllDay { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public TimeSpan? StartTime { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }
        [Required]
        public TimeSpan? EndTime { get; set; }

        // диапазон для DateRangePicker
        public DateRange? DateRange { get; set; }

        private int _durationBlocks = 4;

        public const int MaxSliderMinutes = 60 * 5;

        public int DurationMinutes { get; set; } = 60;


        [Required]
        public int DurationBlocks
        {
            get => _durationBlocks;
            set
            {
                if (value == _durationBlocks) return;
                _durationBlocks = value;
                UpdateEndByDuration();
            }
        }

        public string DurationLabel => FormatDuration(DurationMinutes, true);

        public readonly string[] DurationMarks =
            Enumerable.Range(0, MaxSliderMinutes / 15 + 1)
                      .Select(i => FormatDuration(i * 15, false))
                      .ToArray();

        [MaxLength(20)]
        public string Color { get; set; } = "#2196F3";

        public UserTaskStatus Status { get; set; } = UserTaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public TaskFormViewModel() { }

        public TaskFormViewModel(TaskItemDetailsVm dto)
        {
            Id = dto.Id;
            Title = dto.Title;
            Description = dto.Description;
            IsAllDay = dto.IsAllDay;
            StartDate = dto.StartDate.UtcDateTime.Date;
            StartTime = dto.StartDate.UtcDateTime.TimeOfDay;
            EndDate = dto.EndDate.UtcDateTime.Date;
            EndTime = dto.EndDate.UtcDateTime.TimeOfDay;
            DurationMinutes = (int)(dto.EndDate - dto.StartDate).TotalMinutes;
            Status = dto.Status;
            Priority = dto.Priority;
        }



        public CreateTaskItemDto ToCreateDto(DateTime startUtc, DateTime endUtc) => new()
        {
            Title = Title,
            Description = Description,
            IsAllDay = IsAllDay,
            StartDate = new DateTimeOffset(startUtc),
            EndDate = new DateTimeOffset(endUtc),
            Status = Status,
            Priority = Priority,
            IsRecurring = false,
        };

        public UpdateTaskItemDto ToUpdateDto(DateTime startUtc, DateTime endUtc) => new()
        {
            Id = Id!.Value,
            Title = Title,
            Description = Description,
            IsAllDay = IsAllDay,
            StartDate = new DateTimeOffset(startUtc),
            EndDate = new DateTimeOffset(endUtc),
            Color = Color,
            Status = Status,
            Priority = Priority,
            IsRecurring = false,
        };

        public void OnDurationMinutesChanged(int durationMinutes)
        {
            DurationMinutes = durationMinutes;
            UpdateEndByDuration();

        }

        public void OnStartTimeChanged(TimeSpan? startTime)
        {
            StartTime = startTime;
            UpdateDurationByTime();
        }

        public void OnEndTimeChanged(TimeSpan? endTime)
        {
            EndTime = endTime;
            UpdateDurationByTime();
        }

        private void UpdateDurationByTime()
        {
            DurationMinutes = (int)(EndTime.Value - StartTime.Value).TotalMinutes; 
        }

        private void UpdateEndByDuration()
        {
            if (StartDate.HasValue && StartTime.HasValue)
            {
                var start = StartDate.Value.Date + StartTime.Value;
                var end = start.AddMinutes(DurationMinutes);
                EndDate = end.Date;
                EndTime = end.TimeOfDay;
                DateRange = new DateRange(StartDate.Value, EndDate.Value);
            }
        }

        private static string FormatDuration(int minutes, bool IsDurationLabel = false)
        {
            var h = minutes / 60;
            var m = minutes % 60;

            if (minutes == 0)
                if(IsDurationLabel) return "Мгновенно";
                else return "0";

            if (!IsDurationLabel && minutes >= MaxSliderMinutes)
                return $"{h}+";

            if (m == 0 && h > 0)
                return $"{h}ч";

            if (!IsDurationLabel)
                return string.Empty;

            if (h > 0)
                return $"{h}ч {m} мин";
            else
                return $"{m} мин";

        }

        public string? ValidateEndTime(TimeSpan? endTime)
        {
            if (endTime == null)
                return "Укажите время окончания";

            var start = StartDate!.Value.Date + StartTime!.Value;
            var finish = EndDate!.Value.Date + endTime.Value;
            return finish < start
                ? "Конечное время должно быть позже или равно начальному"
                : null;
        }
    }
}
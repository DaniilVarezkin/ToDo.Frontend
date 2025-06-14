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

        public string DurationLabel => FormatDuration(_durationBlocks, true);

        public readonly string[] DurationMarks =
            Enumerable.Range(0, 17)
                      .Select(i => FormatDuration(i, false))
                      .ToArray();

        [MaxLength(20)]
        public string Color { get; set; } = "#2196F3";

        public UserTaskStatus Status { get; set; } = UserTaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }

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
            _durationBlocks = (int)(dto.EndDate - dto.StartDate).TotalMinutes / 15;
            Status = dto.Status;
            Priority = dto.Priority;
            IsRecurring = dto.IsRecurring;
            RecurrenceRule = dto.RecurrenceRule;
        }

        private void UpdateEndByDuration()
        {
            if (StartDate.HasValue && StartTime.HasValue)
            {
                var start = StartDate.Value.Date + StartTime.Value;
                var end = start.AddMinutes(_durationBlocks * 15);
                EndDate = end.Date;
                EndTime = end.TimeOfDay;
                DateRange = new DateRange(StartDate.Value, EndDate.Value);
            }
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
            IsRecurring = IsRecurring,
            RecurrenceRule = RecurrenceRule
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
            IsRecurring = IsRecurring,
            RecurrenceRule = RecurrenceRule
        };

        private static string FormatDuration(int blocks, bool useMin = false)
        {
            var totalMin = blocks * 15;
            var h = totalMin / 60;
            var m = totalMin % 60;

            if (totalMin == 0)
                return "0";

            if (m == 0 && h > 0)
                return $"{h}ч";

            if (!useMin)
                return string.Empty;

            if (h > 0)
                return $"{h}ч {m} мин";
            else
                return $"{m} мин";

        }
    }
}
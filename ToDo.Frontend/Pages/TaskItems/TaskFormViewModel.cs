using System.ComponentModel.DataAnnotations;
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

        private int _durationBlocks = 4; // 4×15м = 1ч
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

        // человекочитаемая подпись
        public string DurationLabel
        {
            get
            {
                var totalMin = _durationBlocks * 15;
                var h = totalMin / 60;
                var m = totalMin % 60;
                return h > 0
                    ? (m > 0 ? $"{h}ч {m} мин" : $"{h}ч")
                    : $"{m} мин";
            }
        }

        [MaxLength(20)]
        public string Color { get; set; } = "#2196F3";

        public UserTaskStatus Status { get; set; } = UserTaskStatus.Todo;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        // если нужна рекурсия
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
        private void UpdateEndByDuration()
        {
            if (StartDate.HasValue && StartTime.HasValue)
            {
                var start = StartDate.Value.Date + StartTime.Value;
                var end = start.AddMinutes(_durationBlocks * 15);
                EndDate = end.Date;
                EndTime = end.TimeOfDay;
            }
        }
    }

}

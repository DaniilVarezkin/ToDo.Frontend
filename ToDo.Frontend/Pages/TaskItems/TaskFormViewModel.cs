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

        [Required]
        public int DurationHours { get; set; }

        [MaxLength(20)]
        public string Color { get; set; } = "#2196F3";

        public UserTaskStatus Status { get; set; } = UserTaskStatus.Todo;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        // если нужна рекурсия
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
    }

}

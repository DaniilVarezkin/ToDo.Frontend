using MudBlazor;
using ToDo.Shared.Enums;

namespace ToDo.Frontend.Common.Extensions
{
    public static class TaskPriorityExtensions
    {
        public static Color GetPriorityMudColor(this TaskPriority pri) => pri switch
        {
            TaskPriority.High => Color.Error,
            TaskPriority.Medium => Color.Primary,
            TaskPriority.Low => Color.Secondary,
            _ => Color.Default
        };
    }
}

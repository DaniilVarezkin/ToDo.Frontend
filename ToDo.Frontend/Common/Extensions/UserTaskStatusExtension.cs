using MudBlazor;
using ToDo.Shared.Enums;

namespace ToDo.Frontend.Common.Extensions
{
    public static class UserTaskStatusExtension
    {
        public static Color GetStatusMudColor(this UserTaskStatus status) => status switch
        {
            UserTaskStatus.Done => Color.Success,
            UserTaskStatus.InProgress => Color.Info,
            UserTaskStatus.Todo => Color.Warning,
            _ => Color.Default
        };
    }
}

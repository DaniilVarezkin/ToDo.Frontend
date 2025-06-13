using ToDo.Shared.Dto.Common;
using ToDo.Shared.Dto.TaskItems;

namespace ToDo.Frontend.Services.TaskItems
{
    public interface ITaskItemsService
    {
        /// <summary>
        /// Получает постраничный список задач с фильтрацией и сортировкой.
        /// Отправляет запрос GET на /api/task-items/GetAll с параметрами из queryDto.
        /// </summary>
        Task<PagedResult<TaskItemLookupDto>> GetAllAsync(GetTaskItemListQueryDto queryDto);

        /// <summary>
        /// Получает детали конкретной задачи по её идентификатору.
        /// Отправляет запрос GET на /api/task-items/Get/{id}.
        /// </summary>
        Task<TaskItemDetailsVm> GetAsync(Guid id);

        /// <summary>
        /// Получает задачи в виде данных для календаря за указанный период.
        /// Отправляет запрос GET на /api/task-items/Calendar?start={start}&amp;end={end}.
        /// </summary>
        Task<CalendarTaskItemsVm> GetCalendarAsync(DateTimeOffset start, DateTimeOffset end);

        /// <summary>
        /// Создаёт новую задачу и возвращает её идентификатор.
        /// Отправляет POST-запрос на /api/task-items/Create с данными dto.
        /// </summary>
        Task<Guid> CreateAsync(CreateTaskItemDto dto);

        /// <summary>
        /// Полностью обновляет существующую задачу.
        /// Отправляет PUT-запрос на /api/task-items/Update с данными dto.
        /// </summary>
        Task UpdateAsync(UpdateTaskItemDto dto);

        /// <summary>
        /// Частично обновляет поля задачи.
        /// Отправляет PATCH-запрос на /api/task-items/PartialUpdate с данными dto.
        /// </summary>
        Task PartialUpdateAsync(PartialUpdateTaskItemDto dto);

        /// <summary>
        /// Удаляет задачу по её идентификатору.
        /// Отправляет DELETE-запрос на /api/task-items/Delete/{id}.
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Отмечает задачу как выполненную.
        /// Отправляет POST-запрос на /api/task-items/Complete/{id}.
        /// </summary>
        Task CompleteAsync(Guid id);

        /// <summary>
        /// Снимает отметку о выполнении задачи, возвращая её в статус "Todo".
        /// Отправляет POST-запрос на /api/task-items/Reopen/{id}.
        /// </summary>
        Task ReopenAsync(Guid id);
    }
}

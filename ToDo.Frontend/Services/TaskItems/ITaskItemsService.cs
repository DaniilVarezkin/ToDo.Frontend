using ToDo.Shared.Dto.Common;
using ToDo.Shared.Dto.TaskItems;

namespace ToDo.Frontend.Services.TaskItems
{
    public interface ITaskItemsService
    {
        /// <summary>
        /// GET api/task-items/GetAll
        /// </summary>
        Task<PagedResult<TaskItemLookupDto>> GetAllAsync(GetTaskItemListQueryDto queryDto);

        /// <summary>
        /// GET api/task-items/Get/{id}
        /// </summary>
        Task<TaskItemDetailsVm> GetAsync(Guid id);

        /// <summary>
        /// GET api/task-items/Calendar?start=&amp;end=
        /// </summary>
        Task<CalendarTaskItemsVm> GetCalendarAsync(DateTimeOffset start, DateTimeOffset end);

        /// <summary>
        /// POST api/task-items/Create
        /// </summary>
        Task<Guid> CreateAsync(CreateTaskItemDto dto);

        /// <summary>
        /// PUT api/task-items/Update
        /// </summary>
        Task UpdateAsync(UpdateTaskItemDto dto);

        /// <summary>
        /// PATCH api/task-items/PartialUpdate
        /// </summary>
        Task PartialUpdateAsync(PartialUpdateTaskItemDto dto);

        /// <summary>
        /// DELETE api/task-items/Delete/{id}
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// POST api/task-items/Complete/{id}
        /// </summary>
        Task CompleteAsync(Guid id);

        /// <summary>
        /// POST api/task-items/Reopen/{id}
        /// </summary>
        Task ReopenAsync(Guid id);
    }
}

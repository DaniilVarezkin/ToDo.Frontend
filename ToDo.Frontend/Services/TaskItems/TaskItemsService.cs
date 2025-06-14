using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using ToDo.Frontend.Common.Exceptions;
using ToDo.Shared.Dto.Common;
using ToDo.Shared.Dto.TaskItems;

namespace ToDo.Frontend.Services.TaskItems
{
    public class TaskItemsService : ITaskItemsService
    {
        private readonly HttpClient _http;

        public TaskItemsService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PagedResult<TaskItemLookupDto>> GetAllAsync(GetTaskItemListQueryDto queryDto)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["page"] = queryDto.Page.ToString(),
                ["pageSize"] = queryDto.PageSize.ToString(),
                ["sortBy"] = queryDto.SortBy ?? string.Empty,
                ["sortDescending"] = queryDto.SortDescending.ToString()
            };

            if (queryDto.Status.HasValue)
                queryParams["status"] = ((int)queryDto.Status.Value).ToString();
            if (queryDto.Priority.HasValue)
                queryParams["priority"] = ((int)queryDto.Priority.Value).ToString();
            if (queryDto.IsAllDay.HasValue)
                queryParams["isAllDay"] = queryDto.IsAllDay.Value.ToString();
            if (queryDto.DateFrom.HasValue)
                queryParams["dateFrom"] = queryDto.DateFrom.Value.ToString("o");
            if (queryDto.DateTo.HasValue)
                queryParams["dateTo"] = queryDto.DateTo.Value.ToString("o");
            if (!string.IsNullOrWhiteSpace(queryDto.Search))
                queryParams["search"] = queryDto.Search;

            var url = QueryHelpers.AddQueryString("api/task-items/GetAll", queryParams);
            var result = await _http.GetFromJsonAsync<PagedResult<TaskItemLookupDto>>(url);
            return result!;
        }

        public async Task<TaskItemDetailsVm> GetAsync(Guid id)
        {
            var response = await _http.GetAsync($"api/task-items/Get/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskItemDetailsVm>()!;
        }

        public async Task<CalendarTaskItemsVm> GetCalendarAsync(DateTimeOffset start, DateTimeOffset end)
        {
            var url = QueryHelpers.AddQueryString("api/task-items/Calendar", new Dictionary<string, string?>
            {
                ["start"] = start.ToString("o"),
                ["end"] = end.ToString("o")
            });
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CalendarTaskItemsVm>()!;
        }

        public async Task<Guid> CreateAsync(CreateTaskItemDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/task-items/Create", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Guid>()!;
            }
            ValidationProblemDetails? problemDetails = new ValidationProblemDetails();
            try
            {
                problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                var errors = problemDetails.Errors.SelectMany(e => $"{e.Key} {e.Value}");
                Console.WriteLine(string.Join(", ", errors));
            }
            catch { }

            var message = problemDetails?.Title
                              ?? $"Сервер вернул {(int)response.StatusCode} {response.ReasonPhrase}";
            throw new ServerException(message, problemDetails);

        }

        public async Task UpdateAsync(UpdateTaskItemDto dto)
        {
            var response = await _http.PutAsJsonAsync("api/task-items/Update", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task PartialUpdateAsync(PartialUpdateTaskItemDto dto)
        {
            var response = await _http.PatchAsJsonAsync("api/task-items/PartialUpdate", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/task-items/Delete/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task CompleteAsync(Guid id)
        {
            var response = await _http.PostAsync($"api/task-items/Complete/{id}", content: null);
            response.EnsureSuccessStatusCode();
        }

        public async Task ReopenAsync(Guid id)
        {
            var response = await _http.PostAsync($"api/task-items/Reopen/{id}", content: null);
            response.EnsureSuccessStatusCode();
        }
    }
}

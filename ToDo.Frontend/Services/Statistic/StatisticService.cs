using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using ToDo.Shared.Dto.Statistics;

namespace ToDo.Frontend.Services.Statistic
{
    public class StatisticService : IStatisticService
    {
        private readonly HttpClient _http;

        public StatisticService(HttpClient http)
        {
            _http = http;
        }

        public async Task<GlobalTaskStatisticsVm> GetGlobalAsync()
        {
            var response = await _http.GetAsync("api/statistics/GetGlobal");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GlobalTaskStatisticsVm>()!;
        }

        public async Task<DailyTaskStatisticsVm> GetDailyAsync(int days = 7)
        {
            var url = QueryHelpers.AddQueryString(
                "api/statistics/GetDaily",
                new Dictionary<string, string?>
                {
                    ["days"] = days.ToString()
                }
            );
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DailyTaskStatisticsVm>()!;
        }

        public async Task<TaskStatusHistoryVm> GetStatusHistoryAsync(int days = 7)
        {
            var url = QueryHelpers.AddQueryString(
                "api/statistics/GetStatusHistory",
                new Dictionary<string, string?>
                {
                    ["days"] = days.ToString()
                }
            );
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskStatusHistoryVm>()!;
        }
    }
}

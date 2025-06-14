using ToDo.Shared.Dto.Statistics;

namespace ToDo.Frontend.Services.Statistic
{
    public interface IStatisticService
    {
        /// <summary>
        /// Получает общую статистику по задачам пользователя.
        /// Отправляет GET-запрос на /api/statistics/GetGlobal.
        /// </summary>
        Task<GlobalTaskStatisticsVm> GetGlobalAsync();

        /// <summary>
        /// Получает ежедневную статистику завершения задач за указанный период.
        /// Отправляет GET-запрос на /api/statistics/GetDaily?days={days}.
        /// </summary>
        /// <param name="days">Количество дней для статистики (по умолчанию 7).</param>
        Task<DailyTaskStatisticsVm> GetDailyAsync(int days = 7);
    }
}

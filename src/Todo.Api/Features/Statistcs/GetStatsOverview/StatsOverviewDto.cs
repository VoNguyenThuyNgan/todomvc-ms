namespace Todo.Api.Features.Statistcs.GetStatsOverview
{
    public record StatsOverviewDto(
        int Total,
        int Active,
        int Completed,
        int Overdue,
        int CompletedToday,
        int CompletedThisWeek,
        double CompletionRate,
        IReadOnlyList<DailyCountDto> CompletedByDay);

    public record DailyCountDto(
        DateOnly Date,
        int Count);
}

namespace Todo.Bff.Clients.Statistics;

public interface IStatisticsApiClient
{
    Task<HttpResponseMessage> GetStatsOverviewAsync();
}
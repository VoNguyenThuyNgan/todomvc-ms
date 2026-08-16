namespace Todo.Bff.Clients.Statistics;

public class StatisticsApiClient : ApiClientBase, IStatisticsApiClient
{
    public StatisticsApiClient(
        IHttpClientFactory httpClientFactory)
        : base(httpClientFactory)
    {
    }

    public Task<HttpResponseMessage> GetStatsOverviewAsync()
    {
        return GetAsync("/api/stats/overview");
    }
}
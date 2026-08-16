using Carter;
using Todo.Bff.Clients.Statistics;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Statistics.GetStatsOverview;

public class GetStatsOverviewEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/bff/stats")
            .WithTags("Statistics");

        group.MapGet("/overview", Handle)
            .WithName("BffGetStatsOverview")
            .WithSummary("Get statistics overview")
            .WithDescription(
                "Proxy request to Todo.Api to retrieve todo statistics.");
    }

    private static async Task<IResult> Handle(
        IStatisticsApiClient client)
    {
        var response = await client.GetStatsOverviewAsync();

        return await response.ToResultAsync();
    }
}
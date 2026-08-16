using Carter;
using Todo.Api.Features.Statistcs.GetStatsOverview;

namespace Todo.Api.Features.Statistics.GetStatsOverview;

public class GetStatsOverviewEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/stats")
            .WithTags("Statistics");

        group.MapGet("/overview", Handle)
            .WithName("GetStatsOverview")
            .WithSummary("Get statistics overview")
            .WithDescription("Returns aggregated todo statistics.")
            .Produces<StatsOverviewDto>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Handle(
        CancellationToken cancellationToken)
    {
        var overview = await StatsQueries.GetOverviewAsync(
            cancellationToken);

        return Results.Ok(overview);
    }
}
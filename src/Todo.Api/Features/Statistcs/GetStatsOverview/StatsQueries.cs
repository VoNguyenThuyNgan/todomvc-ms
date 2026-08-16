using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Features.Statistcs.GetStatsOverview;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Statistics.GetStatsOverview;

public static class StatsQueries
{
    public static async Task<StatsOverviewDto> GetOverviewAsync(
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var today = now.Date;
        var tomorrow = today.AddDays(1);

        var weekStart = GetStartOfWeek(today);
        var weekEnd = weekStart.AddDays(7);

        var sevenDaysAgo = today.AddDays(-6);

        var facetStage = new BsonDocument(
            "$facet",
            new BsonDocument
            {
                {
                    "summary",
                    new BsonArray
                    {
                        new BsonDocument(
                            "$group",
                            new BsonDocument
                            {
                                {
                                    "_id",
                                    BsonNull.Value
                                },
                                {
                                    "total",
                                    new BsonDocument("$sum", 1)
                                },
                                {
                                    "active",
                                    new BsonDocument(
                                        "$sum",
                                        new BsonDocument(
                                            "$cond",
                                            new BsonArray
                                            {
                                                new BsonDocument(
                                                    "$eq",
                                                    new BsonArray
                                                    {
                                                        "$IsCompleted",
                                                        false
                                                    }),
                                                1,
                                                0
                                            }))
                                },
                                {
                                    "completed",
                                    new BsonDocument(
                                        "$sum",
                                        new BsonDocument(
                                            "$cond",
                                            new BsonArray
                                            {
                                                new BsonDocument(
                                                    "$eq",
                                                    new BsonArray
                                                    {
                                                        "$IsCompleted",
                                                        true
                                                    }),
                                                1,
                                                0
                                            }))
                                }
                            })
                    }
                },

                {
                    "overdue",
                    new BsonArray
                    {
                        new BsonDocument(
                            "$match",
                            new BsonDocument
                            {
                                {
                                    "IsCompleted",
                                    false
                                },
                                {
                                    "DueAt",
                                    new BsonDocument("$lt", now)
                                }
                            }),

                        new BsonDocument(
                            "$count",
                            "count")
                    }
                },

                {
                    "completedToday",
                    new BsonArray
                    {
                        new BsonDocument(
                            "$match",
                            new BsonDocument
                            {
                                {
                                    "IsCompleted",
                                    true
                                },
                                {
                                    "CompletedAt",
                                    new BsonDocument
                                    {
                                        {
                                            "$gte",
                                            today
                                        },
                                        {
                                            "$lt",
                                            tomorrow
                                        }
                                    }
                                }
                            }),

                        new BsonDocument(
                            "$count",
                            "count")
                    }
                },

                {
                    "completedThisWeek",
                    new BsonArray
                    {
                        new BsonDocument(
                            "$match",
                            new BsonDocument
                            {
                                {
                                    "IsCompleted",
                                    true
                                },
                                {
                                    "CompletedAt",
                                    new BsonDocument
                                    {
                                        {
                                            "$gte",
                                            weekStart
                                        },
                                        {
                                            "$lt",
                                            weekEnd
                                        }
                                    }
                                }
                            }),

                        new BsonDocument(
                            "$count",
                            "count")
                    }
                },

                {
                    "completedByDay",
                    new BsonArray
                    {
                        new BsonDocument(
                            "$match",
                            new BsonDocument
                            {
                                {
                                    "IsCompleted",
                                    true
                                },
                                {
                                    "CompletedAt",
                                    new BsonDocument(
                                        "$gte",
                                        sevenDaysAgo)
                                }
                            }),

                        new BsonDocument(
                            "$group",
                            new BsonDocument
                            {
                                {
                                    "_id",
                                    new BsonDocument(
                                        "$dateTrunc",
                                        new BsonDocument
                                        {
                                            {
                                                "date",
                                                "$CompletedAt"
                                            },
                                            {
                                                "unit",
                                                "day"
                                            }
                                        })
                                },
                                {
                                    "count",
                                    new BsonDocument(
                                        "$sum",
                                        1)
                                }
                            }),

                        new BsonDocument(
                            "$sort",
                            new BsonDocument("_id", 1))
                    }
                }
            });

        var result = await DB.Fluent<TodoItem>()
            .AppendStage<BsonDocument>(facetStage)
            .FirstOrDefaultAsync(cancellationToken);

        return BuildOverview(result, today);
    }

    private static DateTime GetStartOfWeek(DateTime date)
    {
        var diff =
            (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;

        return date.AddDays(-diff).Date;
    }

    private static StatsOverviewDto BuildOverview(
    BsonDocument? result,
    DateTime today)
    {
        if (result is null)
        {
            return new StatsOverviewDto(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                BuildSevenDays(today, []));
        }

        var summary = result["summary"]
            .AsBsonArray
            .FirstOrDefault()
            ?.AsBsonDocument;

        var total = summary?["total"].AsInt32 ?? 0;
        var active = summary?["active"].AsInt32 ?? 0;
        var completed = summary?["completed"].AsInt32 ?? 0;

        var overdue = GetCount(result, "overdue");
        var completedToday = GetCount(result, "completedToday");
        var completedThisWeek = GetCount(result, "completedThisWeek");

        var completionRate = total == 0
            ? 0
            : (double)completed / total;

        var completedByDay =
            GetCompletedByDay(result, today);

        return new StatsOverviewDto(
            total,
            active,
            completed,
            overdue,
            completedToday,
            completedThisWeek,
            completionRate,
            completedByDay);
    }

    private static int GetCount(
    BsonDocument result,
    string facetName)
    {
        var facet = result[facetName]
            .AsBsonArray;

        if (facet.Count == 0)
        {
            return 0;
        }

        return facet[0]
            .AsBsonDocument["count"]
            .AsInt32;
    }


    private static IReadOnlyList<DailyCountDto> GetCompletedByDay(
    BsonDocument result,
    DateTime today)
    {
        var counts = new Dictionary<DateTime, int>();

        var facet = result["completedByDay"]
            .AsBsonArray;

        foreach (var item in facet)
        {
            var document = item.AsBsonDocument;

            var date = document["_id"]
                .ToUniversalTime()
                .Date;

            var count = document["count"]
                .AsInt32;

            counts[date] = count;
        }

        return BuildSevenDays(
            today,
            counts);
    }

    private static IReadOnlyList<DailyCountDto> BuildSevenDays(
        DateTime today,
        Dictionary<DateTime, int> counts)
    {
        var result = new List<DailyCountDto>();

        for (var i = 6; i >= 0; i--)
        {
            var date = today.AddDays(-i);

            result.Add(
                new DailyCountDto(
                    DateOnly.FromDateTime(date),
                    counts.TryGetValue(date, out var count)
                        ? count
                        : 0));
        }

        return result;
    }
}
using System.Text;

namespace Todo.Bff.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<IResult> ToResultAsync(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            var contentType =
                response.Content.Headers.ContentType?.MediaType
                ?? "application/json";

            return Results.Content(
                content,
                contentType,
                Encoding.UTF8,
                (int)response.StatusCode);
        }
    }
}

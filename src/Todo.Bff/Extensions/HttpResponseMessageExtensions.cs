using System.Net;
using System.Text;

namespace Todo.Bff.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<IResult> ToResultAsync(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return Results.NoContent();
            }

            var content = await response.Content.ReadAsStringAsync();

            var contentType =
                response.Content.Headers.ContentType?.MediaType
                ?? "application/json";

            return Results.Text(
                content,
                contentType,
                Encoding.UTF8,
                (int)response.StatusCode);
        }
    }
}

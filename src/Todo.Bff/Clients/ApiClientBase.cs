namespace Todo.Bff.Clients
{
    public abstract class ApiClientBase
    {
        protected readonly HttpClient HttpClient;

        protected ApiClientBase(IHttpClientFactory httpClientFactory)
        {
            HttpClient = httpClientFactory.CreateClient("TodoApi");
        }

        protected Task<HttpResponseMessage> GetAsync(string url)
        {
            return HttpClient.GetAsync(url);
        }

        protected Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return HttpClient.DeleteAsync(url);
        }

        protected Task<HttpResponseMessage> PostAsync<TRequest>(
            string url,
            TRequest request)
        {
            return HttpClient.PostAsJsonAsync(url, request);
        }

        protected Task<HttpResponseMessage> PutAsync<TRequest>(
            string url,
            TRequest request)
        {
            return HttpClient.PutAsJsonAsync(url, request);
        }

        protected Task<HttpResponseMessage> PatchAsync<TRequest>(
            string url,
            TRequest request)
        {
            return HttpClient.PatchAsJsonAsync(url, request);
        }

        protected Task<HttpResponseMessage> PatchAsync(string url)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Patch,
                url);

            return HttpClient.SendAsync(request);
        }
    }
}

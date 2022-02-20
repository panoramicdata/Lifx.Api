using System.Net.Http.Headers;

namespace Lifx.Api.Cloud.Infrastructure
{
    internal static class RequestFactoryExtensions
    {
        public static HttpClient CreateClient(this IRequestFactory factory, AuthenticationHeaderValue auth)
        {
            var client = factory.CreateClient(auth);
            return client;
        }

        internal static HttpRequestMessage CreateRequest(this IRequestFactory factory, string url)
        {
            return CreateRequest(factory, url, HttpMethod.Get);
        }

        internal static HttpRequestMessage CreateRequest(this IRequestFactory factory, string url, HttpMethod method)
        {
            var request = factory.CreateRequest();
            request.RequestUri = new Uri(url);
            request.Method = method;
            return request;
        }

        internal static HttpRequestMessage CreateRequest(this IRequestFactory factory, Uri uri, HttpMethod method)
        {
            var request = factory.CreateRequest();
            request.RequestUri = uri;
            request.Method = method;
            return request;
        }
    }
}

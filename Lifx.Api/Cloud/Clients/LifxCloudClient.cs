using Lifx.Api.Cloud.Infrastructure;
using Lifx.Api.Cloud.Models;
using Lifx.Api.Cloud.Models.Request;
using Lifx.Api.Cloud.Models.Response;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Lifx.Api.Cloud.Clients
{
    public partial class LifxCloudClient : IDisposable
    {

        internal const float DEFAULT_DURATION = 1f;
        internal const PowerState DEFAULT_POWER_ON = PowerState.On;

        private HttpClient Client { get; set; }

        private const string Url = "https://api.lifx.com/v1/";

        private readonly string LightEndPoint = $"{Url}lights/";

        private readonly string SceneEndPoint = $"{Url}scenes/";

        private readonly string ColorEndPoint = $"{Url}color/";

        private LifxCloudClient()
        {
        }

        public static Task<LifxCloudClient> CreateAsync(string appToken)
        {
            LifxCloudClient client = new();
            client.Initialize(appToken);
            return Task.FromResult(client);
        }

        public void Initialize(string appToken)
        {
            RequestFactory _factory = new();
            Client = _factory.CreateClient(new AuthenticationHeaderValue("Bearer", appToken));

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            appToken);
        }

        public async Task<ColorResult> ValidateColor(string color)
        {
            var response = await GetResponseData<ColorResult>($"{ColorEndPoint}?string={color}");

            if (response.GetType() == typeof(ErrorResponse))
            {
                throw new Exception(((ErrorResponse)response).Error);
            }
            else
            {
                return (ColorResult)response;
            }
        }

        internal async Task<ApiResponse> PutResponseData<T>(string url, object data)
        {
            return await await ResilientCall(async () =>
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await Client.PutAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                ApiResponse resource;

                if (result.Contains("error"))
                {
                    resource = JsonConvert.DeserializeObject<ErrorResponse>(result);
                }
                else
                {
                    resource = JsonConvert.DeserializeObject<SuccessResponse>(result);
                }

                return resource;
            });
        }

        internal async Task<ApiResponse> PostResponseData<T>(string url, object data)
        {
            return await await ResilientCall(async () =>
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                ApiResponse resource;

                if (result.Contains("error"))
                {
                    resource = JsonConvert.DeserializeObject<ErrorResponse>(result);
                }
                else
                {
                    resource = JsonConvert.DeserializeObject<SuccessResponse>(result);
                }

                return resource;
            });
        }

        internal async Task<object> GetResponseData<T>(string url)
        {
            return await await ResilientCall(async () =>
            {
                var response = await Client.GetAsync(url);

                var result = await response.Content.ReadAsStringAsync();
                object resource;

                if (result.Contains("error"))
                {
                    resource = JsonConvert.DeserializeObject<ErrorResponse>(result);
                }
                else
                {
                    resource = JsonConvert.DeserializeObject<T>(result);
                }

                return resource;
            });
        }

        private async Task<T> ResilientCall<T>(Func<T> block)
        {
            int currentRetry = 0;
            TimeSpan delay = TimeSpan.FromSeconds(2);

            for (; ; )
            {
                try
                {
                    return block();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Operation Exception");

                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > 3 || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(delay);
            }
        }

        private static bool IsTransient(Exception ex)
        {
            if (ex is WebException webException)
            {
                // If the web exception contains one of the following status values
                // it might be transient.
                return new[] {WebExceptionStatus.ConnectionClosed,
                  WebExceptionStatus.Timeout,
                  WebExceptionStatus.RequestCanceled }.
                        Contains(webException.Status);
            }

            // Additional exception checking logic goes here.
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Client?.Dispose();
        }

        ~LifxCloudClient()
        {
            Dispose(false);
        }
    }
}

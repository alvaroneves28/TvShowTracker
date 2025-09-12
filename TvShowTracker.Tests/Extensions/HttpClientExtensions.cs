using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TvShowTracker.Tests.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpClient"/> to simplify JSON requests and responses.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Sends a POST request with JSON content to the specified URL.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <param name="client">The <see cref="HttpClient"/> instance.</param>
        /// <param name="url">The target URL.</param>
        /// <param name="data">The object to serialize as JSON.</param>
        /// <returns>The HTTP response message.</returns>
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient client, string url, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PostAsync(url, content);
        }

        /// <summary>
        /// Sends a PUT request with JSON content to the specified URL.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize as JSON.</typeparam>
        /// <param name="client">The <see cref="HttpClient"/> instance.</param>
        /// <param name="url">The target URL.</param>
        /// <param name="data">The object to serialize as JSON.</param>
        /// <returns>The HTTP response message.</returns>
        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(
            this HttpClient client, string url, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PutAsync(url, content);
        }

        /// <summary>
        /// Sets the Authorization header with a Bearer token.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> instance.</param>
        /// <param name="token">The Bearer token.</param>
        public static void SetBearerToken(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Reads HTTP content and deserializes it from JSON into the specified type.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> instance.</param>
        /// <returns>The deserialized object or null if content is empty.</returns>
        public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}

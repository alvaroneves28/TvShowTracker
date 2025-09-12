using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;

namespace TvShowTracker.Tests
{
    /// <summary>
    /// Provides custom assertion methods for unit and integration tests.
    /// </summary>
    public static class CustomAssertions
    {
        /// <summary>
        /// Asserts that two <see cref="TvShowDto"/> instances have the same property values.
        /// </summary>
        /// <param name="expected">The expected TV show DTO.</param>
        /// <param name="actual">The actual TV show DTO.</param>
        public static void AssertTvShowDto(TvShowDto expected, TvShowDto actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.Network, actual.Network);
            Assert.Equal(expected.Rating, actual.Rating);
            Assert.Equal(expected.ShowType, actual.ShowType);
        }

        /// <summary>
        /// Asserts that two <see cref="UserDto"/> instances have the same property values.
        /// </summary>
        /// <param name="expected">The expected user DTO.</param>
        /// <param name="actual">The actual user DTO.</param>
        public static void AssertUserDto(UserDto expected, UserDto actual)
        {
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Email, actual.Email);
        }

        /// <summary>
        /// Asserts that an <see cref="AuthResponseDto"/> is valid and contains a token and user.
        /// </summary>
        /// <param name="response">The authentication response DTO.</param>
        public static void AssertAuthResponse(AuthResponseDto response)
        {
            Assert.NotNull(response);
            Assert.NotNull(response.Token);
            Assert.NotNull(response.User);
            Assert.True(response.ExpiresAt > DateTime.UtcNow);
        }

        /// <summary>
        /// Asserts that a <see cref="PagedResultDto{T}"/> contains the expected number of items and valid paging information.
        /// </summary>
        /// <typeparam name="T">The type of items in the paged result.</typeparam>
        /// <param name="result">The paged result DTO.</param>
        /// <param name="expectedCount">The expected number of items in the page.</param>
        public static void AssertPagedResult<T>(PagedResultDto<T> result, int expectedCount)
        {
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(expectedCount, result.Data.Count());
            Assert.True(result.TotalCount >= expectedCount);
            Assert.True(result.TotalPages >= 1);
        }
    }
}

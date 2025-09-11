using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;

namespace TvShowTracker.Tests
{
    public static class CustomAssertions
    {
        public static void AssertTvShowDto(TvShowDto expected, TvShowDto actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.Network, actual.Network);
            Assert.Equal(expected.Rating, actual.Rating);
            Assert.Equal(expected.ShowType, actual.ShowType);
        }

        public static void AssertUserDto(UserDto expected, UserDto actual)
        {
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Email, actual.Email);
        }

        public static void AssertAuthResponse(AuthResponseDto response)
        {
            Assert.NotNull(response);
            Assert.NotNull(response.Token);
            Assert.NotNull(response.User);
            Assert.True(response.ExpiresAt > DateTime.UtcNow);
        }

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

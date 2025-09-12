using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Tests.Integration
{
    /// <summary>
    /// Custom <see cref="WebApplicationFactory{TStartup}"/> for integration tests.
    /// Replaces the real DbContext with an in-memory DbContext isolated for each test.
    /// </summary>
    /// <typeparam name="TStartup">The application's startup class (Program or Startup).</typeparam>
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        /// <summary>
        /// Configures the WebHost for tests by replacing services like DbContext and background services.
        /// </summary>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove registered DbContext and DbContextOptions
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TvShowContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbContextServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(TvShowContext));
                if (dbContextServiceDescriptor != null)
                    services.Remove(dbContextServiceDescriptor);

                // Remove any existing Entity Framework Core services
                var efServices = services.Where(d => d.ServiceType.Namespace != null &&
                    d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore")).ToList();
                foreach (var service in efServices)
                {
                    services.Remove(service);
                }

                // Add in-memory DbContext for tests with a unique database name
                var databaseName = Guid.NewGuid().ToString();
                services.AddDbContext<TvShowContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName);
                    options.UseApplicationServiceProvider(null); // Avoid conflicts with the service provider
                });

                // Remove background services that are not needed for tests
                var backgroundServices = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();
                foreach (var service in backgroundServices)
                {
                    services.Remove(service);
                }
            });

            // Set environment to "Testing" to avoid production logic
            builder.UseEnvironment("Testing");
        }

        /// <summary>
        /// Gets an instance of <see cref="TvShowContext"/> in memory for use in tests.
        /// </summary>
        /// <returns>An initialized <see cref="TvShowContext"/> instance.</returns>
        public TvShowContext GetDbContext()
        {
            var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TvShowContext>();
            context.Database.EnsureCreated(); // Ensure the in-memory database is created
            return context;
        }
    }
}

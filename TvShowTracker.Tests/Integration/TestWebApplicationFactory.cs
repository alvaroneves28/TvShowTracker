using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Tests.Integration
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remover TODOS os DbContext e DbContextOptions registrados
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TvShowContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbContextServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(TvShowContext));
                if (dbContextServiceDescriptor != null)
                    services.Remove(dbContextServiceDescriptor);

                // Remover todos os serviços do Entity Framework existentes
                var efServices = services.Where(d => d.ServiceType.Namespace != null &&
                    d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore")).ToList();
                foreach (var service in efServices)
                {
                    services.Remove(service);
                }

                // Adicionar DbContext em memória para testes com nome único
                var databaseName = Guid.NewGuid().ToString();
                services.AddDbContext<TvShowContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName);
                    options.UseApplicationServiceProvider(null); // Importante!
                });

                // Remover Background Service para testes
                var backgroundServices = services.Where(
                    d => d.ServiceType == typeof(IHostedService)).ToList();
                foreach (var service in backgroundServices)
                {
                    services.Remove(service);
                }
            });

            builder.UseEnvironment("Testing");
        }

        public TvShowContext GetDbContext()
        {
            var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TvShowContext>();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
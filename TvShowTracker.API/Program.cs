using TvShowTracker.Application.Mappings;
using Microsoft.EntityFrameworkCore;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Application.Services;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;
using TvShowTracker.Infrastructure.Repositories;

namespace TvShowTracker.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Registrar repositories
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ITvShowRepository, TvShowRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();
            builder.Services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<ITvShowService, TvShowService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Adicionar DbContext
            builder.Services.AddDbContext<TvShowContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

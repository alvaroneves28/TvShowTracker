using TvShowTracker.Application.Mappings;
using Microsoft.EntityFrameworkCore;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Application.Services;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;
using TvShowTracker.Infrastructure.Repositories;
using TvShowTracker.Infrastructure.ExternalServices;
using TvShowTracker.Infrastructure.BackgroundServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

namespace TvShowTracker.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configurar Swagger com suporte a JWT
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TV Show Tracker API",
                    Description = "API para gerenciamento de séries de TV e favoritos",
                });

                // Configurar autenticação JWT no Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Incluir comentários XML (opcional)
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            // Configurar JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Para desenvolvimento
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            // Registrar repositories
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ITvShowRepository, TvShowRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();
            builder.Services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();

            // Registrar services
            builder.Services.AddScoped<ITvShowService, TvShowService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Registrar HttpClient e serviços externos
            builder.Services.AddHttpClient<IExternalTvShowService, EpisodateService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "TvShowTracker/1.0");
            });

            // Registrar Background Service
            builder.Services.AddHostedService<TvShowSyncService>();

            // AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Adicionar DbContext
            builder.Services.AddDbContext<TvShowContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configurar CORS (opcional, para frontend)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Seed da base de dados em desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<TvShowContext>();

                    Console.WriteLine(" EXECUTANDO SEEDER...");

                    await TvShowTracker.API.Data.DatabaseSeeder.SeedAsync(context);

                    Console.WriteLine(" SEEDER CONCLUÍDO");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" ERRO NO SEEDER: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TV Show Tracker API V1");
                    c.RoutePrefix = "swagger"; // Swagger na raiz
                });
            }

            app.UseHttpsRedirection();

            // Importante: ordem correta dos middlewares
            app.UseCors("AllowAll");
            app.UseAuthentication(); // Adicionar antes de UseAuthorization
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
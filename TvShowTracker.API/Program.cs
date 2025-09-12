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
    /// <summary>
    /// Main application entry point and startup configuration for the TV Show Tracker API.
    /// Configures all services, middleware, authentication, documentation, and application pipeline.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entry point that configures services, builds the application pipeline,
        /// and starts the web server.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application</param>
        /// <returns>A task representing the asynchronous application execution</returns>
        public static async Task Main(string[] args)
        {
            // Initialize the web application builder with default configuration
            var builder = WebApplication.CreateBuilder(args);

            #region Core Service Registration

            // Register MVC controllers for API endpoint handling
            builder.Services.AddControllers();

            // Add API Explorer services for Swagger/OpenAPI generation
            builder.Services.AddEndpointsApiExplorer();

            #endregion

            #region Swagger/OpenAPI Configuration

            // Configure Swagger documentation with comprehensive API information and JWT support
            builder.Services.AddSwaggerGen(options =>
            {
                // Basic API information displayed in Swagger UI
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TV Show Tracker API",
                    Description = "REST API for managing TV shows, user favorites, and viewing tracking",
                    Contact = new OpenApiContact
                    {
                        Name = "TV Show Tracker Team",
                        Email = "support@tvshowtracker.com"
                    }
                });

                // Include XML documentation comments in Swagger
                // This enhances API documentation with detailed descriptions from code comments
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // Configure JWT Bearer token authentication in Swagger UI
                // This allows users to input their JWT token directly in the Swagger interface
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization", // Header name
                    In = ParameterLocation.Header, // Token location
                    Type = SecuritySchemeType.ApiKey, // Security scheme type
                    Scheme = "Bearer" // Authentication scheme name
                });

                // Apply JWT authentication requirement to all Swagger operations
                // This adds the "Authorize" button to every endpoint in Swagger UI
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" // Must match the security definition ID above
                            }
                        },
                        Array.Empty<string>() // No specific scopes required
                    }
                });
            });

            #endregion

            #region JWT Authentication Configuration

            // Retrieve JWT configuration settings from appsettings.json
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            // Configure JWT Bearer authentication scheme
            builder.Services.AddAuthentication(options =>
            {
                // Set JWT Bearer as the default authentication scheme
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Development setting - in production, set to true for HTTPS requirement
                options.RequireHttpsMetadata = false;

                // Save the token in the authentication properties for later use
                options.SaveToken = true;

                // Configure token validation parameters for security
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validate the signing key to ensure token authenticity
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    // Validate the issuer to prevent token reuse from other systems
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    // Validate the audience to ensure tokens are for this API
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    // Validate token expiration to prevent use of expired tokens
                    ValidateLifetime = true,

                    // No clock skew tolerance for precise expiration handling
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Register authorization services for policy-based access control
            builder.Services.AddAuthorization();

            #endregion

            #region Repository Layer Registration

            // Register Unit of Work pattern for transactional data operations
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register individual repository implementations for data access
            builder.Services.AddScoped<ITvShowRepository, TvShowRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();
            builder.Services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();

            #endregion

            #region Application Service Registration

            // Register business logic services for application operations
            builder.Services.AddScoped<ITvShowService, TvShowService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            #endregion

            #region External Services Configuration

            // Configure HTTP client for external TV show API integration
            builder.Services.AddHttpClient<IExternalTvShowService, EpisodateService>(client =>
            {
                // Set reasonable timeout for external API calls to prevent hanging requests
                client.Timeout = TimeSpan.FromSeconds(30);

                // Add User-Agent header for API identification and rate limiting compliance
                client.DefaultRequestHeaders.Add("User-Agent", "TvShowTracker/1.0");
            });

            #endregion

            #region Background Services Registration

            // Register background service for automatic TV show data synchronization
            // This service runs independently and synchronizes data from external sources
            builder.Services.AddHostedService<TvShowSyncService>();

            #endregion

            #region AutoMapper Configuration

            // Configure AutoMapper for object-to-object mapping between DTOs and entities
            builder.Services.AddAutoMapper(cfg =>
            {
                // Register the main mapping profile containing all entity-to-DTO mappings
                cfg.AddProfile<MappingProfile>();
            });

            #endregion

            #region Database Configuration

            // Configure Entity Framework Core with SQL Server database provider
            builder.Services.AddDbContext<TvShowContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            #endregion

            #region CORS Configuration

            // Configure Cross-Origin Resource Sharing for frontend integration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    // Development CORS policy - allows all origins, methods, and headers
                    // WARNING: This is permissive and should be restricted in production
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            #endregion

            // Build the configured application
            var app = builder.Build();

            #region Development Database Seeding

            // Execute database seeding only in development environment
            if (app.Environment.IsDevelopment())
            {
                try
                {
                    // Create a service scope to access scoped services like DbContext
                    using var scope = app.Services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<TvShowContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Executing database seeder for development environment");

                    // Execute the database seeding operation
                    await TvShowTracker.API.Data.DatabaseSeeder.SeedAsync(context);

                    logger.LogInformation("Database seeder completed successfully");
                }
                catch (Exception ex)
                {
                    // Use proper logging instead of console output for production readiness
                    using var scope = app.Services.CreateScope();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "Error occurred during database seeding");

                    // Note: Application continues even if seeding fails
                    // This prevents seeding issues from stopping the application startup
                }
            }

            #endregion

            #region Development-Specific Pipeline Configuration

            // Configure Swagger UI only in development environment
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger JSON endpoint
                app.UseSwagger();

                // Configure Swagger UI with custom settings
                app.UseSwaggerUI(c =>
                {
                    // Set the Swagger JSON endpoint for the UI
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TV Show Tracker API V1");

                    // Set custom route prefix for Swagger UI
                    c.RoutePrefix = "swagger";

                    // Additional UI customizations
                    c.DocumentTitle = "TV Show Tracker API Documentation";
                    c.DefaultModelsExpandDepth(-1); // Hide models section by default
                });
            }

            #endregion

            #region Middleware Pipeline Configuration

            // IMPORTANT: Middleware order is critical for proper functionality

            // Force HTTPS redirection for security (should be first)
            app.UseHttpsRedirection();

            // Enable CORS before authentication to allow preflight requests
            app.UseCors("AllowAll");

            // Authentication middleware - validates JWT tokens and populates User identity
            // MUST come before authorization middleware
            app.UseAuthentication();

            // Authorization middleware - enforces access policies based on authenticated user
            // MUST come after authentication middleware
            app.UseAuthorization();

            // Map controller endpoints - handles routing to controller actions
            app.MapControllers();

            #endregion

            #region Application Execution

            // Start the application and listen for requests
            // RunAsync allows for graceful shutdown handling
            await app.RunAsync();

            #endregion
        }
    }
}
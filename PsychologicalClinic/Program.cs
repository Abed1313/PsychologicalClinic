using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Models;
using PsychologicalClinic.Data;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Interface;
using PsychologicalClinic.Repository.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Database connection
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ClinicDbContext>(options => options.UseSqlServer(connectionString));

        // Identity configuration
        builder.Services.AddIdentity<Characters, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true; // Require unique email for users
        })
        .AddEntityFrameworkStores<ClinicDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = JwtTokenServeses.ValidateToken(builder.Configuration);
        });

        // Dependency injection
        builder.Services.AddScoped<IAcountUser, AccountUserService>();
        builder.Services.AddScoped<JwtTokenServeses>();
        builder.Services.AddScoped<MailjetEmailService>();
        builder.Services.AddScoped<IDoctor, DoctorService>();
        builder.Services.AddScoped<IQuiz, QuizService>();

        // CORS Policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.WithOrigins("http://localhost:3000") // Replace with your frontend URL
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Preserve original property casing
    });

        // Swagger configuration
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Psychological Clinic API",
                Version = "v1",
                Description = "API for managing the Psychological Clinic"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Please enter a valid JWT token."
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
        });

        var app = builder.Build();

        // Middleware
        app.UseCors("AllowReactApp");
        app.UseAuthentication();
        app.UseAuthorization();

        // Swagger middleware
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "api/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/api/v1/swagger.json", "Psychological Clinic API");
            options.RoutePrefix = "";
        });

        // Map controllers
        app.MapControllers();

        // Run the application
        app.Run();
    }
}

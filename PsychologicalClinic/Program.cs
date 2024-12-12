using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Data;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();


        string BuilderVar = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ClinicDbContext>(optionX => optionX.UseSqlServer(BuilderVar));

        builder.Services.AddIdentity<Characters, IdentityRole>(options =>
        {
            // Identity options can be configured here
            options.User.RequireUniqueEmail = true; // Require unique email for users
        })
           .AddEntityFrameworkStores<ClinicDbContext>()
           .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = JwtTokenServeses.ValidateToken(builder.Configuration);
        });


        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Data;
using PsychologicalClinic.Models;

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

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace oostfraeiskorg;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
        });

        // Add configuration for appsettings.json
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        // Add DotVVM services
        builder.Services.AddDotVVM<DotvvmStartup>();

        // Build the app
        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();

        // Initialize DotVVM
        app.UseDotVVM<DotvvmStartup>();

        // Run the app
        app.Run("http://localhost:5000");
    }
}

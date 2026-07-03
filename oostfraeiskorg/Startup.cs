using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotVVM.Framework.Routing;
using System;
using System.Threading.RateLimiting;
using oostfraeiskorg.Services;

namespace oostfraeiskorg;

public class Startup
{

    public IConfiguration Configuration { get; private set; }

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddAuthorization();
        services.AddWebEncoders();
        services.AddAuthentication();
        services.AddDotVVM<DotvvmStartup>();
        services.AddSingleton<TranslationCounterService>();

        // Configure native rate limiting
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;

            // Global rate limiter: per IP address with sliding window
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(ipAddress, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 600,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 30
                });
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        //default files
        app.UseDefaultFiles();

        // use static files
        app.UseStaticFiles();

        // Enable rate limiting middleware
        app.UseRateLimiter();

        // use DotVVM
        var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);
        dotvvmConfiguration.AssertConfigurationIsValid();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => 
        {
            endpoints.MapDotvvmHotReload();
            // register ASP.NET Core MVC and other endpoint routing middlewares
        });

        // setup app's root folders
        AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
        AppDomain.CurrentDomain.SetData("WebRootPath", env.WebRootPath);
    }
}

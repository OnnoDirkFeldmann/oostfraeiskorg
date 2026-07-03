using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using System;

namespace oostfraeiskorg;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging((context, builder) =>
            {
                builder.AddConsole();
            })
            .UseKestrel((context, serverOptions) =>
            {
                // Slowloris Protection: Max concurrent connections
                serverOptions.Limits.MaxConcurrentConnections = 1000;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 1000;

                // Max request body size: 10 MB
                serverOptions.Limits.MaxRequestBodySize = 10485760;

                // Request headers timeout: Critical for Slowloris protection
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(5);

                // Keep-alive timeout: Prevents slow connections from holding resources
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(15);

                // Minimum data rate for request body: Prevents slow POST attacks
                serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(
                    bytesPerSecond: 240,
                    gracePeriod: TimeSpan.FromSeconds(5)
                );

                // Minimum data rate for response: Prevents slow read attacks
                serverOptions.Limits.MinResponseDataRate = new MinDataRate(
                    bytesPerSecond: 240,
                    gracePeriod: TimeSpan.FromSeconds(5)
                );

                // Additional Slowloris protections
                serverOptions.Limits.MaxRequestHeaderCount = 100;
                serverOptions.Limits.MaxRequestHeadersTotalSize = 32768; // 32 KB
                serverOptions.Limits.MaxRequestLineSize = 8192; // 8 KB

                // Connection buffer limits
                serverOptions.Limits.MaxRequestBufferSize = 1048576; // 1 MB
                serverOptions.Limits.MaxResponseBufferSize = 65536; // 64 KB
            })
            .UseUrls("http://localhost:5000")
            .Build();
}

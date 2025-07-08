using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Tests.Integration.TestUtilities;

public static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<T> WithTestLogging<T>(this WebApplicationFactory<T> factory)
        where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        });
    }

    public static WebApplicationFactory<T> WithTestEnvironment<T>(this WebApplicationFactory<T> factory,
        string environment = "Testing") where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
        });
    }
}
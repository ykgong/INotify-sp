using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Settings.Configuration;

namespace Message.Subscript.Server
{
    public static class ConfigureExtension
    {
        public static WebApplicationBuilder AddHostConfig(this WebApplicationBuilder builder, IConfiguration? configuration = null)
        {
            if (configuration == null)
            {
                configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            }


            var minLevelStr = configuration.GetValue<string>("Logging:LogLevel:Default") ?? "Information";
            var minLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), minLevelStr, true);

            Log.Logger = new LoggerConfiguration().MinimumLevel.Is(minLevel).ReadFrom.Configuration(configuration).CreateLogger();
            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext();
            });

            builder.WebHost.UseUrls("http://*:5001");

            return builder;
        }
    }
}

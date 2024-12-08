using Common.Notify.MessageProvider;
using Common.Notify.Tools;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Settings.Configuration;

namespace Message.Subscript.Server.Extensions
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

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext();
            });
#if DEBUG
            builder.WebHost.UseUrls("http://*:5001");
#endif

            builder.Services.AddSingleton<MailProvider>();
            builder.Services.AddSingleton<SmsProvider>();
            builder.Services.AddSingleton<WechatProvider>();
            builder.Services.AddSingleton<WechatTemplateProvider>();
            builder.Services.AddSingleton<DingTalkProvider>();
            builder.Services.AddSingleton<AesEncryption>();
            builder.Services.AddScoped<IMessageAccountHelper, MessageAccountHelper> ();

            return builder;
        }
    }
}

using System.ComponentModel;
using System.Text;
using Common.Notify.DTO;
using Message.WebApi.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Settings.Configuration;

namespace Message.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Set();
            }).AddDapr();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.AddHostConfig();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseErrorHandling();
            app.UseAuthorization();
            app.MapControllers();
            //app.MapSubscribeHandler();//¿ªÆô¶©ÔÄ
            app.Run();
        }
    }
}

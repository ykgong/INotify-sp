using Serilog;
using System.Text;
using Message.Subscript.Server.Extensions;
using Common.Notify.Tools;
using System.Diagnostics;
using System.ComponentModel;
using System.Management;


namespace Message.Subscript.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Set();
            }).AddMyDapr("SubWebApi",5001,35001,50002);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => {
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                c.IncludeXmlComments(Path.Combine(basePath, "Common.Notify.xml"));
                c.IncludeXmlComments(Path.Combine(basePath, "Message.Subscript.Server.xml"));
            });
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
            app.MapSubscribeHandler();//开启订阅
            app.Run();
        }
    }    
}

using Message.WebApi.Extensions;
using Common.Notify.Tools;

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
            builder.Services.AddSwaggerGen(c => {
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                c.IncludeXmlComments(Path.Combine(basePath, "Common.Notify.xml"));
                c.IncludeXmlComments(Path.Combine(basePath, "Message.WebApi.xml"));
            });
            builder.AddHostConfig();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseErrorHandling();
            app.UseAuthorization();
            app.MapControllers();
            //app.MapSubscribeHandler();//开启订阅
            app.Run();
        }
    }
}

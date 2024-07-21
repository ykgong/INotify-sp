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
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//��ȡӦ�ó�������Ŀ¼�����ԣ����ܹ���Ŀ¼Ӱ�죬������ô˷�����ȡ·����
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
            //app.MapSubscribeHandler();//��������
            app.Run();
        }
    }
}

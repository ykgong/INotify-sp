using System;
using System.Text;
using System.Threading.Tasks;
using Common.Notify.DTO;
using Common.Notify.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Message.Subscript.Server.Extensions
{
    /// <summary>
    /// 错误处理中间件
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// 中间件构造函数
        /// </summary>
        /// <param name="next"></param>
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
        {
            try
            {
                context.Request.EnableBuffering();//允许重复读取body
                                                  // 读取请求体内容
                string requestBody = "空";
                // 读取请求体内容
                if (context.Request.ContentLength.HasValue && context.Request.ContentLength > 0)
                {
                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 2048, true))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }
                    //context.Request.Body.Position= 0;
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    var form = context.Request.Query;
                    var parameters = new Dictionary<string, string>();
                    foreach (var t in form)
                    {
                        parameters.Add(t.Key, t.Value);
                    }

                    if (parameters.Count > 0)
                        requestBody = parameters.ToJson();
                }

                // 输出请求体内容到控制台
                logger.LogInformation($"[{DateTime.UtcNow},{"".GetLocalIpAddress()}收到队列请求({context.Connection.RemoteIpAddress})] {context.Request.Method} {context.Request.Path}\r\nRequest Body: {requestBody}");

                // 继续处理请求管道中的下一个中间件
                await next(context);
            }
            catch (Exception ex)
            {
                var statusCode = context.Response.StatusCode;
                var msg = ex.Message;

                logger.LogError(ex, "请求处理错误{0}，状态：{1},路径：{2}\r\n,错误：{3}", DateTime.Now.ToString("MM-dd HH:mm:ss.fff"), statusCode, context.Request.Path, ex.Message);

                await HandleExceptionAsync(context, statusCode, msg);
            }
        }

        private async static Task HandleExceptionAsync(HttpContext context, int statusCode, string msg = "")
        {
            var response = new ResultOutDto();
            if (!string.IsNullOrWhiteSpace(msg))
                response.Error = msg;
            response.Status = statusCode;

            await context.Response.WriteAsync(response.ToJson());
        }
    }

    /// <summary>
    /// 错误处理扩展调用
    /// </summary>
    public static class ErrorHandlingExtensions
    {
        /// <summary>
        /// 处理错误
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }

}

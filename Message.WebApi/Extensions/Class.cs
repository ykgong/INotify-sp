//using Newtonsoft.Json.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using Wide.Core.Common.Extensions;
//using Wide.Core.Common.Security;

//namespace Message.WebApi.Extensions
//{

//    public static class RequestInfoExtension
//    {
//        public static async Task<T> ReadBody<T>(this HttpContext context, Action<string> SetContentType,Action<string> log)
//        {
//            context.Request.EnableBuffering();

//            if (context.Request.ContentType != null)
//            {
//                SetContentType?.Invoke(context.Request.ContentType);
//                if (context.Request.ContentType.Contains("multipart/form-data"))
//                {
//                    var form = await context.Request.ReadFormAsync();
//                    if (form.Files.Any())
//                    {
//                        foreach (var file in form.Files)
//                        {                            
//                            log?.Invoke($"收到文件: {file.FileName}, 大小: {file.Length} 字节");
//                        }
//                    }
//                    else
//                    {
//                        log?.Invoke("收到 multipart/form-data 请求，但没有文件上传");
//                    }
//                    // 不需要重置请求体位置，因为 ReadFormAsync() 不会消耗流
//                }
//                else if (context.Request.ContentType.Contains("application/json"))
//                {
//                    // JSON 请求
//                    if (context.Request.ContentLength.HasValue && context.Request.ContentLength > 0)
//                    {
//                        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
//                        {
//                            string requestBody = await reader.ReadToEndAsync();
//                            log?.Invoke($"收到 JSON 请求: {requestBody}");

//                            // 重置请求体位置，以便后续中间件或控制器可以再次读取
//                            context.Request.Body.Seek(0, SeekOrigin.Begin);
//                        }
//                    }
//                }
//                else if (context.Request.ContentType.Contains("application/xml"))
//                {
//                    // XML 请求
//                    if (context.Request.ContentLength.HasValue && context.Request.ContentLength > 0)
//                    {
//                        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
//                        {
//                            string requestBody = await reader.ReadToEndAsync();
//                            log?.Invoke($"收到 XML 请求: {requestBody}");

//                            // 重置请求体位置，以便后续中间件或控制器可以再次读取
//                            context.Request.Body.Seek(0, SeekOrigin.Begin);
//                        }
//                    }
//                }
//                else
//                {
//                    // 其他类型的请求
//                    log?.Invoke($"收到未知类型的请求: {context.Request.ContentType}");
//                }
//            }
//        }
//    }
//}

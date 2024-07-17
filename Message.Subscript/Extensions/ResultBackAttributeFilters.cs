using Message.Subscript.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Message.WebApi.Extensions
{
    /// <summary>
    /// 返回处理
    /// </summary>
    public class ResultBackAttributeFilters : ResultFilterAttribute
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<ResultBackAttributeFilters> logger;

        public ResultBackAttributeFilters(IConfiguration configuration, ILogger<ResultBackAttributeFilters> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Result is ObjectResult))
            {
                base.OnResultExecuting(context);
                return;
            }

            var objresult = ((ObjectResult)context.Result).Value;
            if (objresult == null)
            {
                base.OnResultExecuting(context);
                return;
            }
            var result= objresult.ToJson();
            logger.LogInformation($"[{DateTime.UtcNow}请求返回-{context.HttpContext.Response.StatusCode}]:  {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}\r\nresponse:{result}");

            ////Aes加密
            //if (!string.IsNullOrWhiteSpace(secretKey))
            //{
            //    //ToDo:解密RequestHeader.Secret
            //    //ToDo:加密返回结果
            //}

            base.OnResultExecuting(context);
        }
    }
}

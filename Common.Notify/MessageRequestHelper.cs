using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Notify.DTO;
using Common.Notify.Security;
using Common.Notify.Tools;
using Microsoft.Extensions.Configuration;

namespace Common.Notify.Test
{
    public class MessageRequestHelper:IMessageRequestHelper
    {
        private readonly IConfiguration configuration;

        public MessageRequestHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<ResultOutDto> Send<T>(T message, Action<string> loggin, string url="")
        {
            var requestInfo = new RequestIntDto<T>()
            {
                MessageId = Guid.NewGuid().ToString("N"),
                Stamp = (decimal)DateTime.Now.ToOADate(),
                Nonce = DateTime.Now.ToOADate().ToString(),
                Data = message
            };
            var sign = requestInfo.ToJson();
            var key = string.Empty;
            requestInfo.Signature = requestInfo.SignatureOriginal(key, m => loggin?.Invoke("签名日志：" + m), HashTypeEnum.HMACSHA256);
            loggin?.Invoke("请求：" + requestInfo.ToJson());

            if (string.IsNullOrWhiteSpace(url))
                url = configuration.GetValue<string>("MessageUrl");

            var result = await HttpHelper.PostAsync<ResultOutDto>(url, message);
            return result;
        }
    }

    public interface IMessageRequestHelper
    {
        Task<ResultOutDto> Send<T>(T message, Action<string> loggin, string url);
    }
}

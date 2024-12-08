using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Notify.Config;
using Common.Notify.Enums;
using Microsoft.Extensions.Configuration;

namespace Common.Notify.Tools
{
    public class MessageAccountHelper : IMessageAccountHelper
    {
        private readonly IConfiguration configuration;
        private readonly AesEncryption aesEncryption;

        public MessageAccountHelper(IConfiguration configuration, AesEncryption aesEncryption)
        {
            this.configuration = configuration;
            this.aesEncryption = aesEncryption;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configTypeConst"></param>
        /// <returns></returns>
        public Task<T> GetMessageConfig<T>(string configTypeConst)
        {
            var secretValue = configuration.GetValue<string>($"secret:config:{configTypeConst}");
            var secretStr = aesEncryption.Decrypt(secretValue);
            var testData = secretStr.FromJson<T>();

            return Task.FromResult(testData);
        }
    }

    public interface IMessageAccountHelper
    {
        Task<T> GetMessageConfig<T>(string configTypeConst);
    }
}

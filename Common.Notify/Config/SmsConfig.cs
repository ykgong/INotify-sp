using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Notify.Enums;

namespace Common.Notify.Config
{
    public class SmsConfig:BaseConfig
    {
        public SmsConfig()
        {
            //dapr队列配置名-config-sms
            ConfigName = ConfigTypeConst.QueueSMSConfig;
        }

        /// <summary>
        /// appid
        /// </summary>
        public string AppId { get;set; }
        /// <summary>
        /// appkey密钥
        /// </summary>
        public string Secret { get; set; }
    }
}

using Common.Notify.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Config
{
    /// <summary>
    /// 微信
    /// </summary>
    public class WechatTempConfig : BaseConfig
    {
        public WechatTempConfig()
        {
            //dapr配置队列名-config-webchat-temp
            ConfigName = ConfigTypeConst.QueueWeChatTempConfig;
        }

        /// <summary>
        /// 微信appid
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// 微信密钥
        /// </summary>
        public string AppSecret { get; set; }
    }
}

using Common.Notify.Enums;

namespace Common.Notify.Config
{
    /// <summary>
    /// 企业微信
    /// </summary>
    public class WechatConfig : BaseConfig
    {
        public WechatConfig()
        {
            //dapr配置队列名-config-webchat
            ConfigName = ConfigTypeConst.QueueWeChatConfig;
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string Corpid { get; set; }
        /// <summary>
        /// 应用的凭证密钥
        /// </summary>
        public string Corpsecret { get; set; }
        /// <summary>
        /// 企业应用id-小程序通知消息时不需要此字段
        /// </summary>
        public string AgengId { get; set; }
    }
}

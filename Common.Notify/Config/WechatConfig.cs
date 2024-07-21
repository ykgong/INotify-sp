using Common.Notify.Enums;

namespace Common.Notify.Config
{
    public class WechatConfig : BaseConfig
    {
        public WechatConfig()
        {
            ConfigName = ConfigTypeConst.QueueWeChatConfig;
        }

        public string Corpid { get; set; }
        public string Corpsecret { get; set; }

        public string AgengId { get; set; }
    }
}

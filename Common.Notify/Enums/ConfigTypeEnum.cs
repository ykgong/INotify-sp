namespace Common.Notify.Enums
{
    public enum ConfigTypeEnum
    {
        /// <summary>
        /// 测试
        /// </summary>
        Test=-1,
        /// <summary>
        /// 企业微信
        /// </summary>
        WeChat = 0,
        /// <summary>
        /// 钉钉
        /// </summary>
        DingTalk = 1,
        EMail = 2,
        SMS = 3,
        /// <summary>
        /// 微信
        /// </summary>
        WeChatTemp = 4,
    }
    public class ConfigTypeConst
    {
        public const string StateRedis = "statestore";

        public const string QueueTest = "queue-test";
        public const string QueueTestTopic = "topic-test";

        public const string QueueWeChat = "queue-webchat";
        public const string QueueWeChatTopic = "topic-webchat";
        public const string QueueWeChatConfig = "config-webchat";

        public const string QueueWeChatTemp = "queue-webchat-temp";
        public const string QueueWeChatTempTopic = "topic-webchat-temp";
        public const string QueueWeChatTempConfig = "config-webchat-temp";


        public const string QueueSMS = "queue-sms";
        public const string QueueSMSTopic = "topic-sms";
        public const string QueueSMSConfig = "config-sms";


        public const string QueueEMail = "queue-email";
        public const string QueueEMailTopic = "topic-email";
        public const string QueueEMailConfig = "config-email";


        public const string QueueDingTalk = "queue-dingtalk";
        public const string QueueDingTalkTopic = "topic-dingtalk";
        public const string QueueDingTalkConfig = "config-dingtalk";
    }
}

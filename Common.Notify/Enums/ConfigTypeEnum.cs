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
        /// <summary>
        /// 邮件
        /// </summary>
        EMail = 2,
        /// <summary>
        /// 短信
        /// </summary>
        SMS = 3,
        /// <summary>
        /// 微信
        /// </summary>
        WeChatTemp = 4,
    }
    public class ConfigTypeConst
    {
        /// <summary>
        /// 状态存储
        /// </summary>
        public const string StateRedis = "statestore";
        /// <summary>
        /// 密钥存储
        /// </summary>
        public const string SecretStore = "secrets01";

        public const string QueueTest = "queue-test";
        public const string QueueTestTopic = "topic-test";

        public const string QueueName = "pubsub-rabbitmq";

        //企业微信
        public const string QueueWeChatConfig = "configWebchat";
        public const string QueueWeChat = "queue-webchat";
        public const string QueueWeChatTopic = "topic-webchat";

        //微信
        public const string QueueWeChatTempConfig = "configWebchatTemp";
        public const string QueueWeChatTemp = "queue-webchat-temp";
        public const string QueueWeChatTempTopic = "topic-webchat-temp";

        //短信
        public const string QueueSMSConfig = "configSms";
        public const string QueueSMS = "queue-sms";
        public const string QueueSMSTopic = "topic-sms";

        //邮件
        public const string QueueEMailConfig = "configEmail";
        public const string QueueEMail = "queue-email";
        public const string QueueEMailTopic = "topic-email";

        //钉钉
        public const string QueueDingTalkConfig = "configDingtalk";
        public const string QueueDingTalk = "queue-dingtalk";
        public const string QueueDingTalkTopic = "topic-dingtalk";
    }
}

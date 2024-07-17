namespace Common.Notify.Enums
{
    public enum ConfigTypeEnum
    {
        WeChat = 0,
        DingTalk = 1,
        EMail = 2,
        SMS = 3
    }
    public class ConfigTypeConst
    {
        public const string QueueWeChat = "queue-webchat";
        public const string QueueWeChatTopic = "topic-webchat";


        public const string QueueSMS = "queue-sms";
        public const string QueueSMSTopic = "topic-sms";


        public const string QueueEMail = "queue-email";
        public const string QueueEMailTopic = "topic-email";


        public const string QueueDingTalk = "queue-dingtalk";
        public const string QueueDingTalkTopic = "topic-dingtalk";
    }
}

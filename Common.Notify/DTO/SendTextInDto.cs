using Common.Notify.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace Common.Notify.DTO
{
    /// <summary>
    /// 消息发送
    /// </summary>
    [Serializable]
    public class SendTextInDto
    {
        public SendTextInDto()
        {
            SendTo = new List<string>();
        }

        /// <summary>
        /// 消息类型数字：WeChat = 0,DingTalk = 1,EMail = 2,SMS = 3
        /// </summary>         
        public ConfigTypeEnum MsgType { get; set; }
        /// <summary>
        /// 主题-发送邮件时使用
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 发送的目标
        /// </summary>
        public List<string> SendTo { get; set; }
        /// <summary>
        /// 测试token
        /// </summary>
        public string TestToken { get; set; }

        /// <summary>
        /// 发送的文本
        /// </summary>
        public string Content { get; set; }

        public string GetQueueTopicName(out string topic)
        {
            string queueName = string.Empty;
            topic=string.Empty;
            switch (MsgType)
            {
                case ConfigTypeEnum.EMail:
                    queueName = ConfigTypeConst.QueueEMail;
                    topic = ConfigTypeConst.QueueEMailTopic;
                    break;
                case ConfigTypeEnum.SMS:
                    queueName = ConfigTypeConst.QueueSMS;
                    topic = ConfigTypeConst.QueueSMSTopic;
                    break;
                case ConfigTypeEnum.WeChat:
                    queueName = ConfigTypeConst.QueueWeChat;
                    topic = ConfigTypeConst.QueueWeChatTopic;
                    break;
                case ConfigTypeEnum.WeChatTemp:
                    queueName = ConfigTypeConst.QueueWeChatTemp;
                    topic = ConfigTypeConst.QueueWeChatTempTopic;
                    break;
                case ConfigTypeEnum.DingTalk:
                    queueName = ConfigTypeConst.QueueDingTalk;
                    topic = ConfigTypeConst.QueueDingTalkTopic;
                    break;

                case ConfigTypeEnum.Test:
                    queueName = ConfigTypeConst.QueueTest;
                    topic = ConfigTypeConst.QueueTestTopic;
                    break;
            }

            return queueName;
        }
    }
}

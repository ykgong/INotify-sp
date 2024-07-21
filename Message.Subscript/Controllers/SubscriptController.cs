using Common.Notify.Config;
using Common.Notify.DTO;
using Common.Notify.Enums;
using Common.Notify.MessageProvider;
using Common.Notify.Tools;
using Common.Notify.Tools.Mail;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using System.Text;
using static MailKit.Net.Imap.ImapEvent;

namespace Message.Subscript.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly DaprClient daprClient;
        private readonly MailProvider mailProvider;
        private readonly WechatProvider wechatProvider;
        private readonly WechatTemplateProvider templateProvider;
        private readonly DingTalkProvider dingTalkProvider;

        public SubscriptController(ILogger<SubscriptController> logger, DaprClient daprClient, MailProvider mailProvider, WechatProvider wechatProvider, WechatTemplateProvider templateProvider, DingTalkProvider dingTalkProvider)
        {
            this.logger = logger;
            this.daprClient = daprClient;
            this.mailProvider = mailProvider;
            this.wechatProvider = wechatProvider;
            this.templateProvider = templateProvider;
            this.dingTalkProvider = dingTalkProvider;
        }


        [Topic(ConfigTypeConst.QueueTest, ConfigTypeConst.QueueTestTopic)]
        [HttpPost("sub-test")]
        public async Task<IActionResult> TestAsync([FromBody] DaprInfo<SendTextInDto> message)
        {
            logger.LogInformation($"\r\n------------------\r\n收到[Test]订阅消息：{message.ToJson()}");

            if (message.data == null || message.data.SendTo == null || message.data.SendTo.Count <= 0)
            {
                logger.LogInformation($"\r\n------------------\r\nTest信息不存在");
                return StatusCode(500, "Test信息不存在");
            }

            logger.LogInformation($"\r\n------------------\r\n订阅消息-[测试]处理中-{string.Join(",", message.data.SendTo)}");
            return Ok();
        }

        #region Email

        [Topic(ConfigTypeConst.QueueEMail, ConfigTypeConst.QueueEMailTopic)]
        [HttpPost("sub-email")]
        public async Task<ActionResult> SubEmailAsync([FromBody] DaprInfo<SendTextInDto> message)
        {
            //var stream = Request.Body;
            //byte[] buffer = new byte[Request.ContentLength.Value];
            //stream.Position = 0L;
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //var subContent = Encoding.UTF8.GetString(buffer);
            //var message= subContent.FromJson<SendTextInDto>();
            logger.LogInformation($"\r\n------------------\r\n收到[Email]订阅消息：{message.data.ToJson()}");

            var stateValue = await daprClient.GetStateAsync<string>(ConfigTypeConst.StateRedis, ConfigTypeConst.QueueEMailConfig);
            logger.LogInformation($"\r\n------------------\r\n获取[Email]配置信息：{stateValue}");

            var testData = stateValue.FromJson<MailConfig>();

            if (testData == null || string.IsNullOrWhiteSpace(testData.MailFrom) || string.IsNullOrWhiteSpace(testData.MailPwd) || string.IsNullOrWhiteSpace(testData.MailHost) || testData.MailPort <= 0)
            {
                logger.LogInformation($"\r\n------------------\r\n邮件配置信息未设置");
                return StatusCode(500, "邮件配置信息未设置");
            }
            mailProvider.SendMessage(testData, message.data);


            return Ok("邮件处理中");
        }
        #endregion

        #region wechat

        /// <summary>
        /// 企业微信
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Topic(ConfigTypeConst.QueueWeChat, ConfigTypeConst.QueueWeChatTopic)]
        [HttpPost("sub-wechat")]
        public async Task<ActionResult> SubWeChatAsync([FromBody] DaprInfo<SendTextInDto> message)
        {
            //var stream = Request.Body;
            //byte[] buffer = new byte[Request.ContentLength.Value];
            //stream.Position = 0L;
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //var subContent = Encoding.UTF8.GetString(buffer);

            logger.LogInformation($"\r\n------------------\r\n收到[企业微信]订阅消息：{message.data.ToJson()}");

            var stateValue = await daprClient.GetStateAsync<string>(ConfigTypeConst.StateRedis, ConfigTypeConst.QueueWeChatConfig);
            logger.LogInformation($"\r\n------------------\r\n获取[企业微信]配置信息：{stateValue}");
            var testData = stateValue.FromJson<WechatConfig>();

            if (testData == null || string.IsNullOrWhiteSpace(testData.AgengId) || string.IsNullOrWhiteSpace(testData.Corpid) || string.IsNullOrWhiteSpace(testData.Corpsecret))
            {
                logger.LogInformation($"\r\n------------------\r\n企业微信配置信息未设置");
                return StatusCode(500, "企业微信配置信息未设置");
            }
            wechatProvider.SendMessage(testData, message.data);

            return Ok("企业微信消息处理中");
        }

        /// <summary>
        /// 微信模板消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Topic(ConfigTypeConst.QueueWeChatTemp, ConfigTypeConst.QueueWeChatTempTopic)]
        [HttpPost("sub-wechat-temp")]
        public async Task<ActionResult> SubWeChatTempAsync([FromBody] DaprInfo<SendTextInDto> message)
        {
            //var stream = Request.Body;
            //byte[] buffer = new byte[Request.ContentLength.Value];
            //stream.Position = 0L;
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //var subContent = Encoding.UTF8.GetString(buffer);

            logger.LogInformation($"\r\n------------------\r\n收到[微信模板消息]订阅消息：{message.data.ToJson()}");
            var stateValue = await daprClient.GetStateAsync<string>(ConfigTypeConst.StateRedis, ConfigTypeConst.QueueWeChatTempConfig);
            logger.LogInformation($"\r\n------------------\r\n获取[微信]配置信息：{stateValue}");
            var testData = stateValue.FromJson<WechatTempConfig>();
            if (testData == null || string.IsNullOrWhiteSpace(testData.AppID) || string.IsNullOrWhiteSpace(testData.AppSecret))
            {
                logger.LogInformation($"\r\n------------------\r\n微信模板消息未设置");
                return StatusCode(500, "n微信模板消息未设置");
            }
            templateProvider.SendMessage(testData, message.data);

            return Ok("收到[编程式]订阅消息：" + message);
        }
        #endregion

        #region sms

        [Topic(ConfigTypeConst.QueueSMS, ConfigTypeConst.QueueSMSTopic)]
        [HttpPost("sub-sms")]
        public async Task<ActionResult> SubSMSAsync([FromBody] DaprInfo<SendTextInDto> message)
        {
            //var stream = Request.Body;
            //byte[] buffer = new byte[Request.ContentLength.Value];
            //stream.Position = 0L;
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //var subContent = Encoding.UTF8.GetString(buffer);

            logger.LogInformation($"\r\n------------------\r\n收到[编程式-redis]订阅消息：{message}");
            return Ok("收到[编程式]订阅消息：" + message);
        }
        #endregion


        #region dingtalk

        [Topic(ConfigTypeConst.QueueDingTalk, ConfigTypeConst.QueueDingTalkTopic)]
        [HttpPost("sub-dingtalk")]
        public async Task<ActionResult> SubDingTalkAsync([FromBody] SendTextInDto message)
        {
            //var stream = Request.Body;
            //byte[] buffer = new byte[Request.ContentLength.Value];
            //stream.Position = 0L;
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //var subContent = Encoding.UTF8.GetString(buffer);

            logger.LogInformation($"\r\n------------------\r\n收到[编程式-redis]订阅消息：{message}");
            return Ok("收到[编程式]订阅消息：" + message);
        }

        #endregion
    }
}

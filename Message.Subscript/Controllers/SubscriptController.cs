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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using System.Configuration;
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
        private readonly SmsProvider smsProvider;
        private readonly IConfiguration configuration;
        private readonly AesEncryption aesEncryption;
        private readonly IMessageAccountHelper messageAccountHelper;

        public SubscriptController(ILogger<SubscriptController> logger, DaprClient daprClient, MailProvider mailProvider, 
            WechatProvider wechatProvider, WechatTemplateProvider templateProvider, DingTalkProvider dingTalkProvider,SmsProvider smsProvider,
            IConfiguration configuration, AesEncryption aesEncryption, IMessageAccountHelper messageAccountHelper)
        {
            this.logger = logger;
            this.daprClient = daprClient;
            this.mailProvider = mailProvider;
            this.wechatProvider = wechatProvider;
            this.templateProvider = templateProvider;
            this.dingTalkProvider = dingTalkProvider;
            this.smsProvider = smsProvider;
            this.configuration = configuration;
            this.aesEncryption = aesEncryption;
            this.messageAccountHelper = messageAccountHelper;
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

        [Topic(ConfigTypeConst.QueueName, ConfigTypeConst.QueueEMailTopic)]
        [HttpPost("sub-email")]
        public async Task<ActionResult> SubEmailAsync([FromBody] DaprInfo<SendTextInDto> message)
        {
            //var stream = Request.Body;
            //byte[] buffer = new byte[Request.ContentLength.Value];
            //stream.Position = 0L;
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //var subContent = Encoding.UTF8.GetString(buffer);
            //logger.LogInformation($"\r\n------------------\r\n[Email]收到数据：{subContent}");
            ////var message = subContent.FromJson<SendTextInDto>();

            //var stateValue = await daprClient.GetSecretAsync(ConfigTypeConst.SecretStore, ConfigTypeConst.QueueEMailConfig);

            //var secretValue= configuration.GetValue<string>($"secret:config:{ConfigTypeConst.QueueEMailConfig}");
            //var secretStr= aesEncryption.Decrypt(secretValue);
            //var testData= secretStr.FromJson<MailConfig>();

            var testData =await messageAccountHelper.GetMessageConfig<MailConfig>(ConfigTypeConst.QueueEMailConfig);

            if (testData == null || string.IsNullOrWhiteSpace(testData.MailFrom) || string.IsNullOrWhiteSpace(testData.MailPwd) || string.IsNullOrWhiteSpace(testData.MailHost) || testData.MailPort <= 0)
            {
                logger.LogInformation($"\r\n------------------\r\n邮件配置信息未设置");
                return StatusCode(500, "邮件配置信息未设置");
            }
            mailProvider.SendMessage(testData, message.data);

            logger.LogInformation($"\r\n------------------\r\n[Email]已发送完成：{message.ToJson()}");
            return Ok(message);
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

            //logger.LogInformation($"\r\n------------------\r\n收到[企业微信]订阅消息：{message.data.ToJson()}");

            //var stateValue = await daprClient.GetStateAsync<string>(ConfigTypeConst.StateRedis, ConfigTypeConst.QueueWeChatConfig);
            //logger.LogInformation($"\r\n------------------\r\n获取[企业微信]配置信息：{stateValue}");
            //var testData = stateValue.FromJson<WechatConfig>();

            var testData = await messageAccountHelper.GetMessageConfig<WechatConfig>(ConfigTypeConst.QueueEMailConfig);

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

            //logger.LogInformation($"\r\n------------------\r\n收到[微信模板消息]订阅消息：{message.data.ToJson()}");
            //var stateValue = await daprClient.GetStateAsync<string>(ConfigTypeConst.StateRedis, ConfigTypeConst.QueueWeChatTempConfig);
            //logger.LogInformation($"\r\n------------------\r\n获取[微信]配置信息：{stateValue}");
            //var testData = stateValue.FromJson<WechatTempConfig>();

            var testData = await messageAccountHelper.GetMessageConfig<WechatTempConfig>(ConfigTypeConst.QueueEMailConfig);
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

            //logger.LogInformation($"\r\n------------------\r\n收到[短信消息]订阅消息：{message.data.ToJson()}");
            //var stateValue = await daprClient.GetStateAsync<string>(ConfigTypeConst.StateRedis, ConfigTypeConst.QueueSMSConfig);
            //logger.LogInformation($"\r\n------------------\r\n获取[微信]配置信息：{stateValue}");
            //var testData = stateValue.FromJson<SmsConfig>();

            var testData = await messageAccountHelper.GetMessageConfig<SmsConfig>(ConfigTypeConst.QueueEMailConfig);
            if (testData == null || string.IsNullOrWhiteSpace(testData.AppId) || string.IsNullOrWhiteSpace(testData.Secret))
            {
                logger.LogInformation($"\r\n------------------\r\n短信消息未设置");
                return StatusCode(500, "n短信消息未设置");
            }
            smsProvider.SendMessage(testData, message.data);
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

            var testData = await messageAccountHelper.GetMessageConfig<DingTalkConfig>(ConfigTypeConst.QueueEMailConfig);
            return Ok("收到[编程式]订阅消息：" + message);
        }

        #endregion
    }
}

using Common.Notify.Config;
using Common.Notify.DTO;
using Common.Notify.Enums;
using Common.Notify.Tools.Mail;
using Dapr;
using Message.Subscript.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using static MailKit.Net.Imap.ImapEvent;

namespace Message.Subscript.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptController : ControllerBase
    {
        private readonly ILogger logger;

        public SubscriptController(ILogger<SubscriptController> logger)
        {
            this.logger = logger;
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
            logger.LogInformation($"\r\n------------------\r\n收到[编程式-redis]订阅消息：{message.data.ToJson()}");

            MailHelper.SendMail(new MailConfig
            {
                MailHost = "smtp.exmail.qq.com",
                MailPort = 465,
                MailFrom = "gongyinkui@cqyzx.com",
                MailPwd = "170506,.revV",
                SecurityType = MailKit.Security.SecureSocketOptions.Auto

            }, message.data.Subject, message.data.Content, message.data.SendTo);             

            return Ok("收到消息处理中");
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

        #region wechat

        [Topic(ConfigTypeConst.QueueWeChat, ConfigTypeConst.QueueWeChatTopic)]
        [HttpPost("sub-wechat")]
        public async Task<ActionResult> SubWeChatAsync([FromBody] SendTextInDto message)
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

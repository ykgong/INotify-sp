using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Dapr.Client;
using Common.Notify.DTO;
using Message.WebApi.Extensions;
using Common.Notify.Tools;
using Common.Notify.Enums;

namespace Message.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<InfoController> logger;
        private int[] msgType= new int[] { -1,0,1,2,3,4};

        public InfoController(DaprClient daprClient, ILogger<InfoController> logger)
        {
            _daprClient = daprClient;
            this.logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Post([FromBody] RequestIntDto<SendTextInDto> requestMessage)
        {
            var message = requestMessage.Data;

            // 存储消息到不同类型的队列
            var queueName = message.GetQueueTopicName(out var topic);

            logger.LogInformation($"收到消息{queueName},{topic}：{message.ToJson()}");

            await StoreMessageInQueueAsync(queueName, topic, message);

            return Ok(new ResultOutDto(1, "消息已经发送中."));
        }

        private async Task StoreMessageInQueueAsync(string queueName, string topic, SendTextInDto message)
        {
            if (message.SendTo.Count > 1 && msgType.Contains((int)message.MsgType) && !message.IsAll)
            {
                //微信模板消息发送
                var touser = new List<string>() { "" };
                var idx = 1;
                message.SendTo.ForEach(async toUser =>
                {
                    touser[0] = toUser;
                    var msg = new SendTextInDto
                    {
                        SendTo = touser,
                        MsgType = ConfigTypeEnum.WeChatTemp,
                        Subject = message.Subject,
                        Content = (idx++).ToString() + message.Content,
                        TestToken = message.TestToken,
                    };
                    //await _daprClient.PublishEventAsync(queueName.ToLower(), topic.ToLower(), msg);
                    await _daprClient.PublishEventAsync(ConfigTypeConst.QueueName.ToLower(), topic.ToLower(), msg);
                });
            }
            else
                //await _daprClient.PublishEventAsync(queueName.ToLower(), topic.ToLower(), message);
                await _daprClient.PublishEventAsync(ConfigTypeConst.QueueName.ToLower(), topic.ToLower(), message);
        }
    }
}

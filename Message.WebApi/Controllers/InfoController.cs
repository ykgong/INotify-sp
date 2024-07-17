using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Dapr.Client;
using Common.Notify.DTO;
using Message.WebApi.Extensions;

namespace Message.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<InfoController> logger;

        public InfoController(DaprClient daprClient,ILogger<InfoController> logger)
        {
            _daprClient = daprClient;
            this.logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Post([FromBody] SendTextInDto message)
        {

            // 存储消息到不同类型的队列
            var queueName = message.GetQueueTopicName(out var topic);

            logger.LogInformation($"收到消息{queueName},{topic}：{message.ToJson()}");

            await StoreMessageInQueueAsync(queueName, topic, message);

            return Ok(new ResultOutDto(1, "消息已经发送中."));
        }

        private async Task StoreMessageInQueueAsync(string queueName,string topic, SendTextInDto message)
        {
            await _daprClient.PublishEventAsync(queueName.ToLower(), topic.ToLower(), message);
        }
    }
}

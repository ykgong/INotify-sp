using Common.Notify.Config;
using Common.Notify.Tools;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message.Subscript.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly ILogger<StateController> logger;
        private readonly DaprClient daprClient;

        public StateController(ILogger<StateController> logger, DaprClient daprClient)
        {
            this.logger = logger;
            this.daprClient = daprClient;
        }

        const string STATE_STORE = "statestore";
        const string KEY_NAME = "guid";

        /// <summary>
        /// 获取配置示例参数
        /// </summary>
        /// <param name="allConfig"></param>
        /// <returns></returns>
        [HttpGet("config")]
        public async Task<ActionResult> PostConfigAsync()
        {
            return new JsonResult(new ConfigModels());
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] KeyValueData[] testDatas = null)
        {
            try
            {
                if (testDatas == null || testDatas.Length <= 0 || string.IsNullOrWhiteSpace(testDatas[0].Value))
                    testDatas = new KeyValueData[] { new KeyValueData { Key = KEY_NAME, Value = "测试val：" + Guid.NewGuid().ToString() } };

                foreach (var testData in testDatas)
                {
                    var key = testData.Key;
                    if (string.IsNullOrWhiteSpace(testData.Key))
                        key = KEY_NAME;

                    await daprClient.SaveStateAsync<string>(STATE_STORE, key, testData.Value, new Dapr.Client.StateOptions() { Consistency = ConsistencyMode.Strong });

                    logger.LogInformation($"------------------\r\nState状态存储成功：{key}-{testData.Value}");
                }
                return Ok("nState状态存储成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "保存错误", message = ex.Message });
            }
        }

        //通过tag防止并发冲突，保存一个值
        [HttpPost("SaveWithETag")]
        public async Task<ActionResult> PostWithTagAsync([FromBody] KeyValueData[] testDatas = null)
        {
            try
            {
                if (testDatas == null || testDatas.Length <= 0 || string.IsNullOrWhiteSpace(testDatas[0].Value))
                    testDatas = new KeyValueData[] { new KeyValueData { Key = KEY_NAME, Value = "测试val：" + Guid.NewGuid().ToString() } };

                foreach (var testData in testDatas)
                {
                    var key = testData.Key;
                    if (string.IsNullOrWhiteSpace(testData.Key))
                        key = KEY_NAME;

                    var (value, tag) = await daprClient.GetStateAndETagAsync<string>(STATE_STORE, key);
                    var re = await daprClient.TrySaveStateAsync(STATE_STORE, key, testData.Value, tag);
                    logger.LogInformation($"------------------\r\nState-etag状态存储成功：{tag}-{testData.Value}");
                }
                return Ok("State-etag状态存储成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "保存错误", message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(string key = "", string isTag = "")
        {
            key = string.IsNullOrWhiteSpace(key) ? KEY_NAME : key;
            if (!string.IsNullOrWhiteSpace(isTag) || isTag.ToLower().Equals("etag"))
            {
                var (value, etag) = await daprClient.GetStateAndETagAsync<string>(STATE_STORE, key);
                logger.LogInformation($"------------------\r\nState-ETag状态获取成功：{key}-{etag}-{value}");
                return Ok(new { etag, key, data = value });
            }
            var testData = await daprClient.GetStateAsync<string>(STATE_STORE, key);
            logger.LogInformation($"------------------\r\nState-状态获取成功：{key}-{testData}");
            return Ok(testData);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteAsync(string key = "", string etag = "")
        {
            if (string.IsNullOrWhiteSpace(key))
                key = KEY_NAME;

            if (!string.IsNullOrWhiteSpace(etag) || etag.ToLower().Equals("etag"))
            {
                var (value, tag) = await daprClient.GetStateAndETagAsync<string>(STATE_STORE, key);
                var re = await daprClient.TryDeleteStateAsync(STATE_STORE, key, tag);
                logger.LogInformation($"------------------\r\nState-Tag状态删除{(re ? "成功" : "失败")}：{key}-{tag}");
                return Ok("done");
            }

            await daprClient.DeleteStateAsync(STATE_STORE, key);
            logger.LogInformation($"------------------\r\nState-状态删除成功：{key}");
            return Ok("done");
        }

        /// <summary>
        /// keys以逗号分隔
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult> GetListAsync(string keys = "")
        {
            var listKeys = keys.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (listKeys == null || listKeys.Count <= 0)
                listKeys = new List<string> { KEY_NAME };

            var result = await daprClient.GetBulkStateAsync(STATE_STORE, listKeys, 10);
            return Ok(result);
        }

        // 删除多个个值
        [HttpDelete("deletteList")]
        public async Task<ActionResult> DeleteListAsync(string keys = "")
        {
            var listKeys = keys.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (listKeys == null || listKeys.Count <= 0)
                listKeys = new List<string> { KEY_NAME };

            var data = await daprClient.GetBulkStateAsync(STATE_STORE, listKeys, 10);
            var removeList = new List<BulkDeleteStateItem>();
            foreach (var item in data)
            {
                removeList.Add(new BulkDeleteStateItem(item.Key, item.ETag));
            }
            await daprClient.DeleteBulkStateAsync(STATE_STORE, removeList);
            return Ok("done");
        }

    }
    public class KeyValueData
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// 配置参数示例
    /// </summary>
    public class ConfigModels
    {
        public ConfigModels()
        {
            MailConfig = new MailConfig()
            {
                MailHost = "smtp.exmail.qq.com",
                MailPort = 465,
                MailFrom = "gongyinkui@cqyzx.com",
                MailPwd = "pwd",
                SecurityType = MailKit.Security.SecureSocketOptions.Auto
            };
            WechatTempConfig = new WechatTempConfig()
            {
                AppID = "微信appid",
                AppSecret = "微信密钥"
            };
            WechatConfig = new WechatConfig()
            {
                AgengId = "应用id-1000002",
                Corpid = "企业id-ww78ad43af25b2957f",
                Corpsecret = "密钥-Secret"
            };
            DingTalkConfig = new DingTalkConfig();
        }

        /// <summary>
        /// 邮件配置配置
        /// </summary>
        public MailConfig MailConfig { get; set; }
        /// <summary>
        /// 微信模板消息配置
        /// </summary>
        public WechatTempConfig WechatTempConfig { get; set; }
        /// <summary>
        /// 企业微信配置
        /// </summary>
        public WechatConfig WechatConfig { get; set; }
        /// <summary>
        /// 钉钉配置
        /// </summary>
        public DingTalkConfig DingTalkConfig { get; set; }
    }

}

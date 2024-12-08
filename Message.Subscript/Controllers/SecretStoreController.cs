using Common.Notify.Config;
using Common.Notify.Tools;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Ocsp;

namespace Message.Subscript.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretStore : ControllerBase
    {
        private readonly ILogger<StateController> logger;
        private readonly DaprClient daprClient;
        private readonly AesEncryption aesEncryption;
        private readonly string addr = "D:/Works/Temps/SecretStore/secrets01.json";
        private readonly string aeskey = "";
        private readonly string aesiv = "";

        public SecretStore(ILogger<StateController> logger, DaprClient daprClient, IConfiguration configuration,AesEncryption aesEncryption)
        {
            this.logger = logger;
            this.daprClient = daprClient;
            this.aesEncryption = aesEncryption;
            addr = configuration.GetValue<string>("secret:addr") ?? addr;
            aeskey = configuration.GetValue<string>("secret:aes:key") ?? addr;
            aesiv = configuration.GetValue<string>("secret:aes:iv") ?? addr;
        }

        const string STATE_STORE = "secrets01";
        const string KEY_NAME = "testkey";

        /// <summary>
        /// 获取配置示例参数
        /// </summary>
        /// <param name="allConfig"></param>
        /// <returns></returns>
        [HttpGet("config")]
        public async Task<ActionResult> PostConfigAsync()
        {
            var addrs = addr;
#if !DEBUG
            addrs = "";
#endif

            return new JsonResult(new { 结构= "/secrets/\r\n  ├── database-connection-string.txt\r\n  └── api-key.txt", 默认存储地址= addrs, 数据= new KeyValueData[] { new KeyValueData { Key = "secretKey", Value = "value" } } });
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] ConfigModel testDatas = null)
        {
            try
            {
                if (testDatas == null)
                    return StatusCode(500, new { error = "保存错误", message = "参数不能为空" });

                var idx = 0;
                if (!string.IsNullOrEmpty(testDatas.configEmail))
                {
                    testDatas.configEmail = aesEncryption.Encrypt(testDatas.configEmail);
                    idx = 1;
                }

                if (!string.IsNullOrEmpty(testDatas.configSms))
                {
                    testDatas.configSms = aesEncryption.Encrypt(testDatas.configSms);
                    idx = 1;
                }

                if (!string.IsNullOrEmpty(testDatas.configWebchat))
                {
                    testDatas.configWebchat = aesEncryption.Encrypt(testDatas.configWebchat);
                    idx = 1;
                }

                if (!string.IsNullOrEmpty(testDatas.configWebchatTemp))
                {
                    testDatas.configWebchatTemp = aesEncryption.Encrypt(testDatas.configWebchatTemp);
                    idx = 1;
                }
                if (!string.IsNullOrEmpty(testDatas.configDingtalk))
                {
                    testDatas.configDingtalk = aesEncryption.Encrypt(testDatas.configDingtalk);
                    idx = 1;
                }
                if(idx==0)
                    return StatusCode(500, new { error = "保存错误", message = "参数不能为空" });


                // 初始化 SecretManager
                var secretManager = new SecretManager(addr);
                // 存储一个数据库连接字符串秘密
                var val = testDatas.ToJson();
                secretManager.StoreSecret(val);
                logger.LogInformation($"------------------\r\nSecret存储成功\r\n{addr}\r\n{val}");

                return Ok("nSecret存储成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "保存错误", message = ex.Message });
            }
        }         

        [HttpGet]
        public async Task<ActionResult> GetAsync(string key = "")
        {
            key = string.IsNullOrWhiteSpace(key) ? KEY_NAME : key;
            var testData = await daprClient.GetSecretAsync(STATE_STORE, key);
            var ds= aesEncryption.Decrypt(testData[key]);
            logger.LogInformation($"------------------\r\nSecret-状态获取成功：{key}-{testData}-{ds}");
            return Ok(testData);
        }
    }


    public class ConfigModel
    {
        /*
         {
	"configEmail": "{\"MailFrom\":\"gongyinkui@cqyzx.com\",\"MailPwd\":\"Qq@123456\",\"MailHost\":\"smtp.exmail.qq.com\",\"MailPort\":465,\"SecurityType\":1,\"ConfigName\":\"configEmail\"}",
	"configSms": "{\"AppId\":\"重庆银之鑫\",\"Secret\":\"zdf325\",\"ConfigName\":\"configSms\"}",
	"configWebchatTemp": "{\"AppID\":\"wx1fe2259ad0fb697c\",\"AppSecret\":\"d58bf4a9985dc2fcacba659ddd9bbe88\",\"ConfigName\":\"configWebchatTemp\"}",
        "configDingtalk":"",
        "configWebchat":""
}
         */
        public string configEmail { get; set; }
        public string configSms { get; set; }
        public string configWebchat { get; set; }
        public string configWebchatTemp { get; set; }
        public string configDingtalk { get; set; }
    }
}

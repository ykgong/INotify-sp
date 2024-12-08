using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Common.Notify.Config;
using Common.Notify.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common.Notify.Test;

public class KeyValueData
{
    public string Key { get; set; }
    public string Value { get; set; }
}

[TestClass]
public class ConfigBuild
{
    /// <summary>
    /// email
    /// </summary>
    [TestMethod]
    public void TestMailBuild()
    {
        var config = new Config.MailConfig
        {
            MailHost = "smtp.exmail.qq.com",
            MailPort = 465,
            MailFrom = "gongyinkui@cqyzx.com",
            MailPwd = "Qq@123456",
            SecurityType = MailKit.Security.SecureSocketOptions.Auto
        };
        var lists = new KeyValueData[] { new KeyValueData { Key=config.ConfigName,Value= config.ToJson() } };
        Console.WriteLine("email:" + lists.ToJson());
    }

    /// <summary>
    /// DingTalk
    /// </summary>
    [TestMethod]
    public void TestDingBuild()
    {
        var config = new DingTalkConfig
        {
            AgentId = "AgengId",
            AppKey = "AppKey",
            AppSecret = "AppSecret"
        };
        var lists = new KeyValueData[] { new KeyValueData { Key = config.ConfigName, Value = config.ToJson() } };
        Console.WriteLine("DingTalk:" + lists.ToJson());
    }
    /// <summary>
    /// 企业微信
    /// </summary>
    [TestMethod]
    public void TestWebchatBuild()
    {
        var config = new WechatConfig
        {
            AgengId = "AgengId",
            Corpid = "Corpid",
            Corpsecret = "Corpsecret"
        };
        var lists = new KeyValueData[] { new KeyValueData { Key = config.ConfigName, Value = config.ToJson() } };
        Console.WriteLine("企业微信:" + lists.ToJson());
    }
    /// <summary>
    /// 微信
    /// </summary>
    [TestMethod]
    public void TestWechatBuild()
    {
        var config = new Config.WechatTempConfig
        {
            AppID= "wx1fe2259ad0fb697c",
            AppSecret= "d58bf4a9985dc2fcacba659ddd9bbe88"
        };
        var lists = new KeyValueData[] { new KeyValueData { Key = config.ConfigName, Value = config.ToJson() } };
        Console.WriteLine("微信:" + lists.ToJson());
    }
    /// <summary>
    /// SMS
    /// </summary>
    [TestMethod]
    public void TestSmsBuild()
    {
        var config = new Config.SmsConfig
        {
            AppId= "重庆银之鑫",
            Secret= "zdf325",
        };
        var lists = new KeyValueData[] { new KeyValueData { Key = config.ConfigName, Value = config.ToJson() } };
        Console.WriteLine("SMS:" + lists.ToJson());
    }

    public byte[] GenerateKey(int keySize)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.KeySize = keySize;
            aesAlg.GenerateKey();
            return aesAlg.Key;
        }
    }

    public byte[] GenerateIv()
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.GenerateIV();
            return aesAlg.IV;
        }
    }



    [TestMethod]
    public void TestAesKeys()
    {
        var key = GenerateKey(256); // 生成 256 位密钥
        var iv = GenerateIv(); // 生成 128 位 IV

        Console.WriteLine("key:" + Convert.ToBase64String(key));
        Console.WriteLine("iv:" + Convert.ToBase64String(iv));
    }

}

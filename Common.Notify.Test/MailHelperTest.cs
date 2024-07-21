using Common.Notify.Tools.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Common.Notify.Test
{
    [TestClass]
    public class MailHelperTest
    {
        [TestMethod]
        public void SendMail()
        {
            MailHelper.SendMail(new Config.MailConfig
            {
                MailHost = "smtp.exmail.qq.com",
                MailPort = 465,
                MailFrom = "gongyinkui@cqyzx.com",
                MailPwd = "",
                SecurityType = MailKit.Security.SecureSocketOptions.Auto

            }, "测试", "测试邮件内容", new List<string>
            {
                "g.yk@qq.com",
                "leiyang@cqyzx.com",
                "1184470032@qq.com"
            });
        }
    }
}

using Common.Notify.DTO;
using Common.Notify.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Wide.Core.Common.Security;

namespace Common.Notify.Test
{
    [TestClass]
    public class RequestHelper
    {
        [TestMethod]
        public async Task Test1()
        {
            SendTextInDto message = new SendTextInDto()
            {
                MsgType = Enums.ConfigTypeEnum.EMail,
                Subject = "消息测试邮件",
                SendTo = new List<string>(){
                    "g.yk@qq.com",
                    "275049836@qq.com",
                    "ykgong@hotmail.com"
                },
                Content = "这是一封基于分布式集群的消息队列邮件发送测试，收到不用回复"
            };

            var url = "http://localhost:5000/api/Info/send";
            IMessageRequestHelper msgRequestHelper = new MessageRequestHelper(null);
            var result = await msgRequestHelper.Send(message,(msg)=>Console.WriteLine(msg),url);
            Console.WriteLine(result);
        }
    }
}

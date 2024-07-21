using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Tools.Wechat.DTO.Message.MessageDto.Template
{
    public class TemplateWeChatBaseInDto: SendTemplateWeChatInDto
    {
        /// <summary>
        /// 接收者openid
        /// </summary>
        public string touser { get; set; }
    }
}

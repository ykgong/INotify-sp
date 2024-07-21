using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Tools.Wechat.DTO.Message.MessageDto.Template
{
    public class SendTemplateWeChatInDto
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string template_id { get; set; }
        /// <summary>
        /// 模板跳转链接（海外账号没有跳转能力）
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 跳小程序所需数据，不需跳小程序可不用传该数据
        /// </summary>
        public TemplateWeChatMiniProgram? miniprogram { get; set; }
        /// <summary>
        /// 防重入id。对于同一个openid + client_msg_id, 只发送一条消息,10分钟有效,超过10分钟不保证效果。若无防重入需求，可不填
        /// </summary>
        public string client_msg_id { get; set; }=Guid.NewGuid().ToString();
        /// <summary>
        /// 模板数据
        /// </summary>
        public dynamic data { get; set; }
    }

    /// <summary>
    /// 跳小程序所需数据，不需跳小程序可不用传该数据
    /// </summary>
    public class TemplateWeChatMiniProgram
    {
        /// <summary>
        /// 所需跳转到的小程序appid（该小程序appid必须与发模板消息的公众号是绑定关联关系，暂不支持小游戏）
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 所需跳转到小程序的具体页面路径，支持带参数,（示例index?foo=bar），要求该小程序已发布，暂不支持小游戏
        /// </summary>
        public string pagepath { get; set; }
    }
}

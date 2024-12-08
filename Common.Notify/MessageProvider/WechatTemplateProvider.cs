using Common.Notify.Config;
using Common.Notify.DTO;
using Common.Notify.Tools;
using Common.Notify.Tools.Wechat;
using Common.Notify.Tools.Wechat.DTO.Message.MessageDto;
using Common.Notify.Tools.Wechat.DTO.Message.MessageDto.Template;
using System;
using System.Text;

namespace Common.Notify.MessageProvider
{
    /// <summary>
    /// 微信
    /// </summary>
    public class WechatTemplateProvider : BaseProvider
    {
        public void SendMessage(BaseConfig config, object message)
        {
            if (config is not WechatTempConfig wechatConfig)
            {
                throw new Exception("Error DingTalk Config");
            }

            if (message is not TemplateWeChatBaseInDto wechatMessage)
            {
                throw new Exception("Error DingTalk MessageData");
            }

            WechatTemplateHelper.SendMessage(wechatConfig.AppID, wechatConfig.AppSecret, wechatMessage);
        }

        public void SendMessage(BaseConfig config, SendTextInDto dto)
        {
            if (config is not WechatTempConfig wechatConfig)
            {
                throw new Exception("Error DingTalk Config");
            }
            if(dto == null || string.IsNullOrWhiteSpace(dto.Content))
            {
                Console.Write("开始前的数据：请求数据为空");
                return;
            }
            Console.Write("开始前的数据："+dto.ToJson());
            dto.SendTo.ForEach(touser => {
                var msgBuilder = new StringBuilder(dto.Content);
                msgBuilder.Replace("$currSendTo$", touser);
                msgBuilder.Replace("$currMsgId$", Guid.NewGuid().ToString());

                string message = msgBuilder.ToString(); 

                //var modelInTo = dto.Content.FromJson<SendTemplateWeChatInDto>();
                //if (string.IsNullOrEmpty(modelInTo.client_msg_id)) { modelInTo.client_msg_id = Guid.NewGuid().ToString(); }

                //var sendData = new TemplateWeChatBaseInDto
                //{
                //    touser = touser,
                //    template_id = modelInTo.template_id,
                //    url = modelInTo.url,
                //    miniprogram = modelInTo.miniprogram,
                //    client_msg_id = modelInTo.client_msg_id,
                //    data = modelInTo.data
                //};

                WechatTemplateHelper.SendMessage(wechatConfig.AppID, wechatConfig.AppSecret, message, dto.TestToken);
            });
        }
    }
}

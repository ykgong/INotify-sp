using Common.Notify.Config;
using Common.Notify.DTO;
using Common.Notify.Tools.Sms;

namespace Common.Notify.MessageProvider
{
    /// <summary>
    /// 短信
    /// </summary>
    public class SmsProvider : BaseProvider
    {
        public void SendMessage(BaseConfig config, object message)
        {
            if (config is not SmsConfig smsConfig)
            {
                throw new System.Exception("Error Sms Config");
            }

            if (message is not SmsInDto sendData)
            {
                throw new System.Exception("Error Sms MessageData");
            }

            SmsHelper.SendSms(smsConfig, sendData.Content, sendData.SendTo);
        }

        public void SendMessage(BaseConfig config, SendTextInDto dto)
        {
            if (config is not SmsConfig smsConfig)
            {
                throw new System.Exception("Error Sms Config");
            }

            SmsHelper.SendSms(smsConfig, dto.Content, dto.SendTo);
        }
    }
}

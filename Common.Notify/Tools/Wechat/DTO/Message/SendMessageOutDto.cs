namespace Common.Notify.Tools.Wechat.DTO.Message
{
    public class SendMessageOutDto : WechatCommonOutDto
    {
        /// <summary>
        /// 企业微信返回
        /// </summary>
        public string invalidparty { get; set; }
        /// <summary>
        /// 企业微信返回
        /// </summary>
        public string invalidtag { get; set; }
        public string msgid { get; set; }
    }

}

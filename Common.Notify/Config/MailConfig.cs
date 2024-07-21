using Common.Notify.Enums;
using MailKit.Security;

namespace Common.Notify.Config
{
    public class MailConfig : BaseConfig
    {
        public MailConfig()
        {
            ConfigName = ConfigTypeConst.QueueEMailConfig;
        }

        /// <summary>
        /// 发件邮箱
        /// </summary>
        public string MailFrom { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string MailPwd { get; set; }
        /// <summary>
        /// 发件服务器
        /// </summary>
        public string MailHost { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int MailPort { get; set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public SecureSocketOptions SecurityType { get; set; } = SecureSocketOptions.Auto;

    }
}

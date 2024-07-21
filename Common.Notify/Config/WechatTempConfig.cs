using Common.Notify.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Config
{
    public class WechatTempConfig : BaseConfig
    {
        public WechatTempConfig()
        {
            ConfigName = ConfigTypeConst.QueueWeChatTempConfig;
        }

        public string AppID { get; set; }

        public string AppSecret { get; set; }
    }
}

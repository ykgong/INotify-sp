using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Tools.Wechat.DTO
{
    public class AccessTokenOutDto : WechatCommonOutDto
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }

        private readonly DateTime init_time=DateTime.Now;
        public DateTime expires_time => init_time.AddSeconds(expires_in).AddMinutes(-5.0);

        public bool is_expires
        {
            get
            {
                if ((expires_time - DateTime.Now).TotalSeconds > 0.0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}

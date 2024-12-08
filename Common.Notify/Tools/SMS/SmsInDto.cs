using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Tools.Sms
{
    public class SmsInDto
    {
        public string Content { get; internal set; }
        public List<string> SendTo { get; internal set; }
    }
}

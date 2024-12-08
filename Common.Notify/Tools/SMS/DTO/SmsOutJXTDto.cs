using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Tools.SMS.DTO
{

    /// <summary>
    /// 短信返回实体-吉信通
    /// </summary>
    public class SmsOutJXTDto
    {
        public static Dictionary<string, string> ErrorMessages { get; } = new Dictionary<string, string>
    {
        { "000", "已发送" },
        { "-1", "发送请求返回空" },
        { "-02", "未开通接口授权" },
        { "-03", "账号密码错误" },
        { "-04", "参数个数不对或者参数类型错误" },
        { "-110", "IP被限制" },
        { "-12", "其他错误" }
    };


        public SmsOutJXTDto()
        {
        }
        public SmsOutJXTDto(string resultStr)
        {
            //000/Send:1/Consumption:.1/Tmoney:751.05/sid:0620135629504294	状态码/扣费条数/扣费金额/余额/编号
            if(string.IsNullOrWhiteSpace(resultStr))
            {
                Status = "-1";
            }
            else
            {
                var arr = resultStr.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (arr == null || arr.Length <= 0)
                    Status = "-1";
                else
                {
                    Status = arr[0];
                    if (arr.Length >1)
                        SendNum = arr[1];
                    if (arr.Length > 2)
                        Consumption = arr[2];
                    if (arr.Length > 3)
                        Tmoney = arr[3];
                    if (arr.Length > 4)
                        Sid = arr[4];
                }
            }
            Error = ErrorMessages[Status];
        }

        /*
         -02	未开通接口授权
         -03	账号密码错误
         -04	参数个数不对或者参数类型错误
         -110	IP被限制
         -12	其他错误
         */

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        public string Error { get; set; }

        public bool IsOk
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(Status) && Status.Equals("000"))
                {
                    return true;
                }
                return false;
            }
            set { }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public string Sid { get; set; }
        /// <summary>
        /// 扣费条数
        /// </summary>
        public string SendNum { get; set; }
        /// <summary>
        /// 扣费金额
        /// </summary>
        public string Consumption { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public string Tmoney { get; set; }
    }
}

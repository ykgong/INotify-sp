using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.DTO
{
    public class ResultOutDto
    {
        public ResultOutDto():this(1,"成功")
        {
        }

        public ResultOutDto(int status, string error)
        {
            Status = status;
            Error = error;
        }
        /// <summary>
        /// 成功=1/200
        /// </summary>
        public int Status {  get; set; }
        public string Error { get; set; }       
    }
    public class ResultOutDto<T> : ResultOutDto where T : class
    {
        public ResultOutDto(int status, string error) : base(status, error)
        {
        }

        public T Data { get; set; }
    }
}

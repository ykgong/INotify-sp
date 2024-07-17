using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common.Notify.DTO
{
    public class DaprInfo<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string datacontenttype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pubsubname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string specversion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string topic { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string traceid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string traceparent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tracestate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
    }
}

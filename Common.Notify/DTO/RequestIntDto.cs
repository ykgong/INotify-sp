using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common.Notify.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestIntDto<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Stamp{get;set;}
        /// <summary>
        /// 
        /// </summary>
        public string Nonce { get;set;}
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Signature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; set; }
    }
}

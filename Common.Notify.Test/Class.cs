//using Newtonsoft.Json.Linq;
//using System;
//using System.Text.RegularExpressions;
//using Wide.Core.Common.Extensions;
//using Wide.Core.Common.Security;

//namespace Message.WebApi.Extensions
//{
//    /// <summary>
//    /// 通用方法类
//    /// </summary>
//    public static class SignatureOriginalExtensions
//    {
//        #region 生成签名

//        /// <summary>
//        /// 生成签名-等号拼接
//        /// </summary>
//        /// <typeparam name="T">签名对象有</typeparam>
//        /// <param name="args">签名参数</param>
//        /// <param name="key">签名key</param>
//        /// <param name="hashType">签名方式默认HMACSHA256</param>
//        /// <param name="log">日志</param>
//        /// <returns></returns>
//        public static string SignatureOriginal<T>(this T args, string key, Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.HMACSHA256, bool isSignBase64 = false)
//        {
//            if (args == null)
//                return string.Empty;
//            var dataBody = args.ToJsonWithFront();
//            var md5 = SignTostr(dataBody, string.Empty, key, log, hashType, isSignBase64);
//            return md5;
//        }

//        /// <summary>
//        /// 生成签名
//        /// </summary>
//        /// <param name="jsonData"></param>
//        /// <param name="token"></param>
//        /// <returns></returns>
//        public static string SignatureOriginal(this string jsonData, string key, Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.HMACSHA256, bool isSignBase64 = false)
//        {
//            if (string.IsNullOrWhiteSpace(jsonData))//!(data.Contains(":") && data.Contains("{") && data.Contains("}")))
//                return string.Empty;

//            if (!Regex.IsMatch(jsonData, @"^{[\s\S]+}$"))
//            {
//                return SignTostr(jsonData, string.Empty, key, log, hashType, isSignBase64);
//            }
//            var md5 = SignTostr(jsonData, string.Empty, key, log, hashType, isSignBase64);
//            return md5;
//        }


//        #endregion

//        #region 验证签名

//        /// <summary>
//        /// 签名验证-等号拼接
//        /// </summary>
//        /// <param name="requestBody"></param>
//        /// <param name="headerJson"></param>
//        /// <param name="signKey">有默认key和其它key</param>
//        /// <param name="sign">签名请求串</param>
//        /// <param name="hashType">SHA256/HMACSHA256/MD5</param>
//        /// <returns></returns>
//        public static bool CheckSignatureOriginal(this string jsonBody, string signKey, string sign, Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.HMACSHA256, bool isSignBase64 = false)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(jsonBody))
//                    jsonBody = "{}";

//                if (jsonBody == "{}" && !Regex.IsMatch(jsonBody, @"^{[\s\S]+}$"))
//                    return false;

//                var temp = JObject.Parse(jsonBody);
//                //if (!string.IsNullOrWhiteSpace(headerJson) && Regex.IsMatch(headerJson, @"^{[\s\S]+}$"))
//                //{
//                //    if (temp.ContainsKey("h"))
//                //        temp.Remove("h");
//                //    temp.Add("h", JObject.Parse(headerJson));
//                //}
//                var md5 = SignTostr(jsonBody, sign, signKey, log, hashType, isSignBase64);
//                return md5 == sign;
//            }
//            catch //(Exception exception)
//            {
//                //throw new ArgumentException(string.Format("{0}----------{1}", request, exception));
//                return false;
//            }
//        }

//        #endregion

//        #region 签名方法

//        private static string SignTostr(string data, string sign = "", string key = "", Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.MD5, bool isSignBase64 = false)
//        {
//            var s = string.Empty;
//            var md5 = string.Empty;
//            if (hashType == HashTypeEnum.MD5)
//            {
//                s = string.Format("{0}{1}", data, key);
//                md5 = s.GetMd5();
//            }
//            else if (hashType == HashTypeEnum.SHA256)
//            {
//                s = string.Format("{0}{1}", s, key);
//                md5 = s.Sha256();
//            }
//            else if (hashType == HashTypeEnum.HMACSHA256)
//            {
//                s = data;
//                md5 = s.HmacSha256ByBC(key);
//            }
//            else
//            {
//                log?.Invoke("签名失败：HashType为SHA256/HMACSHA256/MD5");
//                return md5;
//            }
//            if (isSignBase64)
//                md5 = md5.FromHex64String();

//            if (string.IsNullOrWhiteSpace(sign))
//                log?.Invoke(string.Format("Original签名原串：{0},生成签名：{1}", s, md5));
//            else
//                log?.Invoke(string.Format("Original签名原串：{0},生成签名：{1},传入签名：{2},验证:{3}", s, md5, sign, md5.Equals(sign)));

//            return md5;
//        }

//        #endregion
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Notify.Tools;
using Newtonsoft.Json.Linq;

namespace Common.Notify.Security
{
    public static class SignatureOriginalExtensions
    {
        public static string SignatureOriginal<T>(this T args, string key, Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.HMACSHA256, bool isSignBase64 = false)
        {
            if (args == null)
            {
                return string.Empty;
            }

            return SignTostr(args.ToJson(), string.Empty, key, log, hashType, isSignBase64);
        }

        public static string SignatureOriginal(this string jsonData, string key, Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.HMACSHA256, bool isSignBase64 = false)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return string.Empty;
            }

            Regex.IsMatch(jsonData, "^{[\\s\\S]+}$");
            return SignTostr(jsonData, string.Empty, key, log, hashType, isSignBase64);
        }

        public static bool CheckSignatureOriginal(this string jsonBody, string signKey, string sign, Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.HMACSHA256, bool isSignBase64 = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonBody))
                {
                    jsonBody = "{}";
                }

                if (jsonBody == "{}" && !Regex.IsMatch(jsonBody, "^{[\\s\\S]+}$"))
                {
                    return false;
                }

                JObject.Parse(jsonBody);
                return SignTostr(jsonBody, sign, signKey, log, hashType, isSignBase64) == sign;
            }
            catch
            {
                return false;
            }
        }

        private static string SignTostr(string data, string sign = "", string key = "", Action<string> log = null, HashTypeEnum hashType = HashTypeEnum.MD5, bool isSignBase64 = false)
        {
            string empty = string.Empty;
            string empty2 = string.Empty;
            switch (hashType)
            {
                case HashTypeEnum.MD5:
                    empty = $"{data}{key}";
                    empty2 = Encipherment.MD5(empty);
                    break;
                case HashTypeEnum.SHA256:
                    empty = $"{empty}{key}";
                    empty2 = empty.Sha256();
                    break;
                case HashTypeEnum.HMACSHA256:
                    empty = data;
                    empty2 = empty.HmacSha256ByBC(key);
                    break;
                default:
                    log?.Invoke("签名失败：HashType为SHA256/HMACSHA256/MD5");
                    return empty2;
            }

            if (isSignBase64)
            {
                empty2 = Convert.ToBase64String(empty2.ToHexBytes());

            }
                 

            if (string.IsNullOrWhiteSpace(sign))
            {
                log?.Invoke($"Original签名原串：{empty},生成签名：{empty2}");
            }
            else
            {
                log?.Invoke($"Original签名原串：{empty},生成签名：{empty2},传入签名：{sign},验证:{empty2.Equals(sign)}");
            }

            return empty2;
        }

        private static byte[] ToHexBytes(this string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                hexString = hexString ?? "";
            }

            byte[] array = new byte[hexString.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return array;
        }
    }
}

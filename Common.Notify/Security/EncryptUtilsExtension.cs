using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Macs;

namespace Common.Notify.Security
{
    public static class EncryptUtilsExtension
    {
        public static char[] DIGITS = new char[16]
        {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'a', 'b', 'c', 'd', 'e', 'f'
        };

        public static string HmacSha256(this string message, string secret, Encoding encoding = null)
        {
            string text = "";
            secret = secret ?? "";
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] bytes = encoding.GetBytes(secret);
            byte[] bytes2 = encoding.GetBytes(message);
            using HMACSHA256 hMACSHA = new HMACSHA256(bytes);
            return hMACSHA.ComputeHash(bytes2).EncodeToHex();
        }

        public static string HmacSha256ByBC(this string sourceString, string secret, Encoding encoding = null)
        {
            //IL_000a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0014: Expected O, but got Unknown
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_001c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0026: Expected O, but got Unknown
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            HMac val = new HMac((IDigest)new Sha256Digest());
            val.Init((ICipherParameters)new KeyParameter(encoding.GetBytes(secret)));
            byte[] array = new byte[val.GetMacSize()];
            byte[] bytes = encoding.GetBytes(sourceString);
            val.BlockUpdate(bytes, 0, bytes.Length);
            val.DoFinal(array, 0);
            return Sha256HexEncode(array);
        }

        public static byte[] HmacSha256ByBCBtye(this string sourceString, string secret, Encoding encoding = null)
        {
            //IL_000a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0014: Expected O, but got Unknown
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_001c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0026: Expected O, but got Unknown
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            HMac val = new HMac((IDigest)new Sha256Digest());
            val.Init((ICipherParameters)new KeyParameter(encoding.GetBytes(secret)));
            byte[] array = new byte[val.GetMacSize()];
            byte[] bytes = encoding.GetBytes(sourceString);
            val.BlockUpdate(bytes, 0, bytes.Length);
            val.DoFinal(array, 0);
            return array;
        }

        public static string Sha256(this string sourceString, Encoding encoding = null)
        {
            //IL_0011: Unknown result type (might be due to invalid IL or missing references)
            //IL_0017: Expected O, but got Unknown
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            byte[] bytes = encoding.GetBytes(sourceString);
            Sha256Digest digest = new Sha256Digest();
            return Sha256Encode(bytes, (IDigest)(object)digest);
        }

        public static byte[] Sha256Byte(this string sourceString, Encoding encoding = null)
        {
            //IL_0012: Unknown result type (might be due to invalid IL or missing references)
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }

            byte[] bytes = encoding.GetBytes(sourceString);
            Sha256Digest val = new Sha256Digest();
            ((GeneralDigest)val).BlockUpdate(bytes, 0, bytes.Length);
            byte[] array = new byte[((GeneralDigest)val).GetDigestSize()];
            ((GeneralDigest)val).DoFinal(array, 0);
            return array;
        }

        private static string Sha256Encode(byte[] data, IDigest digest)
        {
            digest.BlockUpdate(data, 0, data.Length);
            byte[] array = new byte[digest.GetDigestSize()];
            digest.DoFinal(array, 0);
            return Sha256HexEncode(array);
        }

        private static string Sha256HexEncode(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        public static string EncodeToHex(this byte[] data)
        {
            int num = data.Length;
            char[] array = new char[num << 1];
            int i = 0;
            int num2 = 0;
            for (; i < num; i++)
            {
                array[num2++] = DIGITS[(0xF0 & data[i]) >> 4];
                array[num2++] = DIGITS[0xF & data[i]];
            }

            return new string(array);
        }

        private static string EncodeToHex2(this byte[] data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public static string HexToString(this string mHex)
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0)
            {
                return "";
            }

            byte[] array = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
            {
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out array[i / 2]))
                {
                    array[i / 2] = 0;
                }
            }

            return Encoding.Default.GetString(array);
        }

        public static string GetBody(this Dictionary<string, string> paramMap)
        {
            return getSignContent(paramMap);
        }

        private static string getSignContent(Dictionary<string, string> sortedParams)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<string> list = sortedParams.Keys.ToList();
            list.Sort();
            int num = 0;
            int num2 = list.Count();
            for (int i = 0; i < num2; i++)
            {
                string text = list[i];
                string text2 = sortedParams[text];
                if (!string.IsNullOrEmpty(text2))
                {
                    stringBuilder.Append(((num == 0) ? "" : "&") + text + "=" + text2);
                    num++;
                }
            }

            return stringBuilder.ToString();
        }
    }
}

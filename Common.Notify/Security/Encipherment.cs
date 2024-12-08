using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Security
{

    public static class Encipherment
    {
        public static string MD5(string sourceString)
        {
            return MD5(sourceString, Encoding.UTF8);
        }

        public static string MD5(byte[] data)
        {
            byte[] array = new MD5CryptoServiceProvider().ComputeHash(data);
            StringBuilder stringBuilder = new StringBuilder(32);
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x").PadLeft(2, '0'));
            }

            return stringBuilder.ToString();
        }

        public static byte[] ToMD5(string data, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(data));
        }

        public static byte[] ToMD5(byte[] data, int offset, int count)
        {
            return new MD5CryptoServiceProvider().ComputeHash(data, offset, count);
        }

        public static string MD5(string sourceString, Encoding encoding)
        {
            byte[] array = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(sourceString));
            StringBuilder stringBuilder = new StringBuilder(32);
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x").PadLeft(2, '0'));
            }

            return stringBuilder.ToString();
        }

        public static string HMAC_MD5(string value, string key, Encoding encoding)
        {
            HMACMD5 hMACMD = new HMACMD5(encoding.GetBytes(key));
            byte[] bytes = encoding.GetBytes(value);
            byte[] array = hMACMD.ComputeHash(bytes, 0, bytes.Length);
            StringBuilder stringBuilder = new StringBuilder(32);
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x").PadLeft(2, '0'));
            }

            return stringBuilder.ToString();
        }

        public static long TimeId(uint autoid)
        {
            DateTime utcNow = DateTime.UtcNow;
            return ((long)((utcNow.Month << 21) | (utcNow.Day << 16) | ((utcNow.Hour << 12) | (utcNow.Minute << 6) | utcNow.Second)) << 25) | autoid;
        }

        public static uint GetUInt(byte[] inData, int offset, int len)
        {
            uint num = 0u;
            int num2 = 0;
            num2 = ((len <= 8) ? (offset + len) : (offset + 8));
            for (int i = 0; i < num2; i++)
            {
                num <<= 8;
                num |= inData[i];
            }

            return num;
        }
    }
}

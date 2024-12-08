using Common.Notify.Config;
using Common.Notify.Tools.SMS.DTO;
using Common.Notify.Tools.Wechat.DTO.Message;
using MimeKit;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Common.Notify.Tools.Sms
{
    /// <summary>
    /// 短信接口实现
    /// </summary>
    public static class SmsHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config">服务器配置</param>
        /// <param name="content">正文</param>
        /// <param name="sendTo">接收号码</param>
        /// <returns></returns>
        public static void SendSms(SmsConfig config, string content, List<string> sendTo)
        {
            if (sendTo == null || sendTo.Count <= 0)
            {
                throw new Exception("sms to null number");
            }
            if(string.IsNullOrWhiteSpace(content))
            {
                throw new Exception("sms to null content");
            }

            foreach (string item in sendTo)
            {
                string Phone = item;
                string Msg = content;

                string url = "https://service.winic.org/sys_port/gateway/index.asp?";
                string dataStr = "id=" + $"{config.AppId}" + "&pwd=" + $"{config.Secret}" + "&to=" + Phone + "&content=" + Msg + "&time=";
                var resultStr = PostData2(url, dataStr);
                var result = new SmsOutJXTDto(resultStr);
                if(!result.IsOk)
                {
                    throw new Exception($"短信发送失败：{result.ToJson()}");
                }
            }
        }

        public static string PostData(string purl, string str)
        {
            try
            {
                // 获取 GB18030 编码
                Encoding gb18030 = Encoding.GetEncoding("GB18030");
                byte[] data = gb18030.GetBytes(str); // 将字符串转换为 GB18030 编码的字节数组

                // 准备请求
                var client = new RestClient();
                var request = new RestRequest(purl,Method.Post);

                // 设置超时
                request.Timeout = new TimeSpan(0,0,30);

                // 设置请求体和内容类型
                request.AddParameter("application/x-www-form-urlencoded", gb18030.GetString(data), ParameterType.RequestBody);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=GB18030");

                // 发送请求
                var response = client.Execute(request);

                // 处理响应
                return response.Content;
            }
            catch (Exception ex)
            {
                return "posterror";
            }
        }


        public static string PostData2(string purl, string str)
        {
            try
            {
                byte[] data = System.Text.Encoding.GetEncoding("GB18030").GetBytes(str);
                // 准备请求    
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(purl);
                //设置超时     
                req.Timeout = 30000;
                req.Method = "Post";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = data.Length;
                Stream stream = req.GetRequestStream();
                // 发送数据   
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
                Stream receiveStream = rep.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                // Pipes the stream to a higher level stream reader with the required encoding format.   
                StreamReader readStream = new StreamReader(receiveStream, encode);

                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                StringBuilder sb = new StringBuilder("");
                while (count > 0)
                {
                    String readstr = new String(read, 0, count);
                    sb.Append(readstr);
                    count = readStream.Read(read, 0, 256);
                }

                rep.Close();
                readStream.Close();

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "posterror";
            }
        }
    }
}

using Common.Notify.Tools.Wechat.DTO;
using System;
using System.Threading.Tasks;

namespace Common.Notify.Tools
{
    public static class HttpRequestHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">完整Url</param>
        /// <returns></returns>
        public static T Get<T>(string url) where T : WechatCommonOutDto
        {
            Task<T> result = HttpHelper.GetAsync<T>($"{url}");
            result.Wait();

            return GetData(result.Result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">完整Url</param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        public static T Post<T>(string url, object bodyData) where T : WechatCommonOutDto
        {
            Task<T> result = HttpHelper.PostAsync<T>($"{url}", bodyData);
            result.Wait();

            return GetData(result.Result);
        }

        public static T GetData<T>(T data) where T : WechatCommonOutDto
        {
            if (data.errcode == 0)
            {
                return data;
            }

            throw new Exception(data.errmsg);
        }
    }
}

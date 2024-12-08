using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Tools
{
    public static class HttpHelper
    {
        #region Get
        public static async Task<TResult> GetAsync<TResult>(string url, Dictionary<string, object> parameter = null, Dictionary<string, string> Headers = null, int timeout = 10000)
        {
            var result = await GetAsync(url, parameter, Headers, timeout);
            return JsonConvert.DeserializeObject<TResult>(result);
        }
        public static async Task<string> GetAsync(string url, Dictionary<string, object> parameter = null, Dictionary<string, string> Headers = null, int timeout = 10000)
        {
            RestClient client;
            RestRequest request;
            GetInit(url, parameter, Headers, timeout, out client, out request);

            RestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }

        private static void GetInit(string url, Dictionary<string, object> parameter, Dictionary<string, string> Headers, int timeout, out RestClient client, out RestRequest request)
        {
            client = new RestClient();
            request = new RestRequest(url, Method.Get);
            //request.AddHeader("Content-Type", "application/json");
            request.Timeout = TimeSpan.FromSeconds(timeout);

            GenerateHeader(request, Headers);

            if (parameter != null)
            {
                foreach (KeyValuePair<string, object> item in parameter)
                {
                    Parameter para = Parameter.CreateParameter(item.Key, item.Value, ParameterType.GetOrPost);
                    request.AddParameter(para);
                }
            }
        }
        #endregion

        #region Post
        public static async Task<TResult> PostAsync<TResult>(string url, object obj = null, Dictionary<string, string> Headers = null, int timeout = 10000, int bodyType = 0)
        {
            var result = await PostAsync(url, obj, Headers, timeout,bodyType);
            return JsonConvert.DeserializeObject<TResult>(result);
        }
        public static async Task<string> PostAsync(string url, object obj = null, Dictionary<string, string> Headers = null, int timeout = 10000, int bodyType = 0)
        {
            RestClient client;
            RestRequest request;
            PostInit(url, obj, Headers, timeout, out client, out request, bodyType);

            RestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="Headers"></param>
        /// <param name="timeout"></param>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="bodyType">0:Json,1:body,2:string</param>
        private static void PostInit(string url, object obj, Dictionary<string, string> Headers, int timeout, out RestClient client, out RestRequest request,int bodyType=0)
        {
            client = new RestClient();
            request = new RestRequest(url, Method.Post);
            //request.AddHeader("Content-Type", "application/json");
            request.Timeout = TimeSpan.FromSeconds(timeout);

            GenerateHeader(request, Headers);
            if (bodyType == 1)
                request.AddBody(obj);
            else
                request.AddJsonBody(obj);
        }

        /// <summary>
        /// 将header添加到请求中
        /// </summary>
        private static void GenerateHeader(RestRequest request, Dictionary<string, string> Headers)
        {
            if (Headers != null && Headers.Count > 0)
            {
                request.AddOrUpdateHeaders(Headers);
            }
        }
        #endregion
    }
}

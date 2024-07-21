using Common.Notify.Tools.Wechat.DTO;
using Common.Notify.Tools.Wechat.DTO.Message;
using Common.Notify.Tools.Wechat.DTO.Message.MessageDto;
using System.Collections.Concurrent;
using System.IO;

namespace Common.Notify.Tools.Wechat
{
    public static class WechatHelper
    {
        public static readonly string baseUrl = "https://qyapi.weixin.qq.com/cgi-bin/";
        private static readonly ConcurrentDictionary<string, AccessTokenOutDto> _WechatAccessToken = new ConcurrentDictionary<string, AccessTokenOutDto>();

        public static string GetToken(string corpid, string corpsecret)
        {
            if (_WechatAccessToken.TryGetValue(corpid + "AccessToken", out AccessTokenOutDto accessTokenResp) && accessTokenResp != null && !accessTokenResp.is_expires)
            {
                return accessTokenResp.access_token;
            }

            string url = Path.Combine(baseUrl, $"gettoken?corpid={corpid}&corpsecret={corpsecret}");
            AccessTokenOutDto result = HttpRequestHelper.Get<AccessTokenOutDto>(url);
            //_logger.LogInformation($"请求基础AccessToken({appid}),返回:{responseString}"); 
            if (result != null && !string.IsNullOrWhiteSpace(result.access_token))
            {
                _WechatAccessToken.AddOrUpdate(corpid + "AccessToken", result, (oldkey, oldvalue) => result);
                return result.access_token;
            }
            return result.access_token;
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        public static DepartmentOutDto GetDepartment(string corpid, string corpsecret)
        {
            var token = GetToken(corpid, corpsecret);

            var result = HttpRequestHelper.Get<DepartmentOutDto>(Path.Combine(baseUrl, $"department/list?access_token={token}"));

            return result;
        }

        public static DepartmentUserSimple GetDepartmentUser(string corpid, string corpsecret, int departmentId)
        {
            var token = GetToken(corpid, corpsecret);

            var departmentUser = HttpRequestHelper.Get<DepartmentUserSimple>(Path.Combine(baseUrl, $"user/simplelist?access_token={token}&department_id={departmentId}"));

            return departmentUser;
        }


        public static DepartmentUserDetail GetDepartmentUserDetail(string corpid, string corpsecret, int departmentId)
        {
            var token = GetToken(corpid, corpsecret);

            var result = HttpRequestHelper.Get<DepartmentUserDetail>(Path.Combine(baseUrl, $"user/list?access_token={token}&department_id={departmentId}"));


            return result;
        }

        /// <summary>
        /// https://developer.work.weixin.qq.com/document/path/90236
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>

        public static SendMessageOutDto SendMessage(string corpid, string corpsecret, BaseMessageInDto message)
        {
            var token = GetToken(corpid, corpsecret);

            var result = HttpRequestHelper.Post<SendMessageOutDto>(Path.Combine(baseUrl, $"message/send?access_token={token}"), message);

            return result;
        }
        
        /// <summary>
        /// 根据手机号获取userid
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <param name="mobile"></param>
        public static UserOutDto GetUserIdByPhone(string corpid, string corpsecret, string mobile)
        {
            var token = GetToken(corpid, corpsecret);
            var result = HttpRequestHelper.Post<UserOutDto>(Path.Combine(baseUrl, $"user/getuserid?access_token={token}"), new
            {
                mobile= mobile
            });

            return result;
        }
    }
}

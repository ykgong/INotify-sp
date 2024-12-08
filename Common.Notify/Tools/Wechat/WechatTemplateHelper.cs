using Common.Notify.Tools.Wechat.DTO;
using Common.Notify.Tools.Wechat.DTO.Message;
using Common.Notify.Tools.Wechat.DTO.Message.MessageDto;
using System.Collections.Concurrent;
using System;
using System.IO;
using Common.Notify.Tools.Wechat.DTO.Message.MessageDto.Template;

namespace Common.Notify.Tools.Wechat;

/// <summary>
/// 微信接口实现
/// </summary>
public static class WechatTemplateHelper
{
    private static readonly ConcurrentDictionary<string, AccessTokenOutDto> _WechatAccessToken = new ConcurrentDictionary<string, AccessTokenOutDto>();
    /// <summary>
    /// AccessToken请求地址
    /// <para>Get请求</para>
    /// </summary>
    private static string accessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
    /// <summary>
    /// 发送模板消息请求地址
    /// <para>POST请求</para>
    /// </summary>
    private static string templateMsgUrl = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";

    /// <summary>
    /// 获取基础AccessToken
    /// </summary>
    public static string GetAccessToken(string appid, string secret)
    {
        try
        {
            if (_WechatAccessToken.TryGetValue(appid + "AccessToken", out AccessTokenOutDto accessTokenResp) && accessTokenResp != null && !accessTokenResp.is_expires)
            {
                return accessTokenResp.access_token;
            }

            string url = string.Format(accessTokenUrl, appid, secret);
            AccessTokenOutDto result = HttpRequestHelper.Get<AccessTokenOutDto>(url);
            //_logger.LogInformation($"请求基础AccessToken({appid}),返回:{responseString}");
            if (result != null && !string.IsNullOrWhiteSpace(result.access_token))
            {
                _WechatAccessToken.AddOrUpdate(appid + "AccessToken", result, (oldkey, oldvalue) => result);
                return result.access_token;
            }
            return result.access_token;
        }
        catch (Exception ex)
        {
            //_logger.LogInformation($"获取基础AccessToken({appid})出错,详细信息:{ex.Message},位置:{ex.StackTrace}");
            return "";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="corpid"></param>
    /// <param name="corpsecret"></param>

    public static SendMessageOutDto SendMessage(string appid, string secret, TemplateWeChatBaseInDto message, string token="")
    {
        var msg = message.ToJson();
        Console.Write($"开始发送时：token：{string.Format(templateMsgUrl, token)},数据：" + msg);
        if (string.IsNullOrWhiteSpace(token))
        {
            token = GetAccessToken(appid, secret);
        }
        var url=string.Format(templateMsgUrl, token);
        var result = HttpRequestHelper.Post<SendMessageOutDto>(url, msg);

        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="corpid"></param>
    /// <param name="corpsecret"></param>

    public static SendMessageOutDto SendMessage(string appid, string secret, string message, string token="")
    {
        Console.Write($"开始发送时：token：{string.Format(templateMsgUrl, token)},数据：" + message);
        if (string.IsNullOrWhiteSpace(token))
        {
            token = GetAccessToken(appid, secret);
        }

        if (string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(token))
        {
            return new SendMessageOutDto { errcode = -1, errmsg = "消息或Token为空" };
        }

        var url =string.Format(templateMsgUrl, token);
        var result = HttpRequestHelper.Post<SendMessageOutDto>(url, message);

        return result;
    }
}

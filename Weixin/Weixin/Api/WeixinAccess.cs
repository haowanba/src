using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;
using Common.Config;
using Weixin.Tools;

namespace Weixin.Api
{
    public class WeixinAccess
    {
        public static string CheckSignature(HttpRequestBase request)
        {
            string signature = request.Params["signature"];
            string timestamp = request.Params["timestamp"];
            string nonce = request.Params["nonce"];
            string echostr = request.Params["echostr"];

            string token = ConfigReader.GetAppSettingsValue("Weixin.Token");
            List<string> tmp = new List<string> { token, timestamp, nonce };
            tmp.Sort();
            string s = String.Join("", tmp.ToArray());
            string encryptedStr = Encrypt.Sha1(s);
            if (signature.Equals(encryptedStr))
            {
                return echostr;
            }
            return String.Empty;
        }

        public static string GetToken()
        {
            object token = WxCache.Get(WeixinConstName.Token);
            if (token != null)
            {
                return token.ToString();
            }
            string appid = ConfigReader.GetAppSettingsValue(WeixinConstName.Appid);
            string appsecret = ConfigReader.GetAppSettingsValue(WeixinConstName.Appsecret);
            string json = LaHttpRequest.Get(String.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, appsecret));
            TokenResultJson r = JsonConverter.Parse<TokenResultJson>(json);
            WxCache.Set(WeixinConstName.Token, r.access_token, r.expires_in);
            return r.access_token;
        }

        public static string GetJsApiTicket()
        {
            object ticket = WxCache.Get(WeixinConstName.Ticket);
            if (ticket != null)
            {
                return ticket.ToString();
            }

            string accessToken = GetToken();
            string json = LaHttpRequest.Get(String.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", accessToken));
            TicketResultJson result = JsonConverter.Parse<TicketResultJson>(json);
            if (result.errcode == 0 && result.errmsg == "ok")
            {
                int timeout = result.expires_in;
                WxCache.Set(WeixinConstName.Ticket, result.ticket, timeout);
                return result.ticket;
            }
            return String.Empty;
        }

        public static WxConfig GetWxConfig(HttpRequestBase request)
        {
            Uri requestUrl = request.Url;
            if (requestUrl == null)
            {
                return new WxConfig();
            }
            string host = requestUrl.Host;
            string pathAndQuery = requestUrl.PathAndQuery;
            string url = requestUrl.Scheme + "://" + host + pathAndQuery;
            string appid = ConfigReader.GetAppSettingsValue(WeixinConstName.Appid);
            int timeStamp = Math.Abs((int)DateTime.UtcNow.Ticks);
            string nonceStr = Guid.NewGuid().ToString();
            string ticket = GetJsApiTicket();

            string string1 = String.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", ticket, nonceStr, timeStamp, url);
            string signature = Encrypt.Sha1(string1);
            WxConfig r = new WxConfig
            {
                AppId = appid,
                NonceStr = nonceStr,
                Signature = signature,
                Timestamp = timeStamp,
                Link = url,
                Type = "link"
            };
            return r;
        }
    }

    public class WeixinConstName
    {
        public const string Appid = "Weixin.appID";
        public const string Appsecret = "Weixin.appsecret";

        public const string Ticket = "weixin_ticket";
        public const string Token = "weixin_token";
    }

    [DataContract]
    public class TokenResultJson
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public int expires_in { get; set; }
    }

    [DataContract]
    public class TicketResultJson
    {
        [DataMember]
        public int errcode { get; set; }
        [DataMember]
        public string errmsg { get; set; }
        [DataMember]
        public string ticket { get; set; }
        [DataMember]
        public int expires_in { get; set; }
    }

    [Serializable]
    public class WxConfig
    {
        public string AppId { get; set; }
        public int Timestamp { get; set; }
        public string NonceStr { get; set; }
        public string Signature { get; set; }
        /// <summary>
        /// 分享标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 分享描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 分享链接
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 分享图标
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 分享类型,music、video或link，不填默认为link
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 如果type是music或video，则要提供数据链接，默认为空
        /// </summary>
        public string DataUrl { get; set; }
    }
}

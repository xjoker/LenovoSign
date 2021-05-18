using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestSharp;

namespace LenovoSign
{
    public class LenovoUtils
    {
        private const string UserAgent = "Apache-HttpClient/UNAVAILABLE(java 1.5)";
        private const string Source = "android:com.lenovo.club.app-V5.0.1";
        private readonly LenovoBaseInfoModel _baseInfo;
        private readonly string _baseInfoBase64;

        private readonly Regex _getResultValueRegex = new Regex(@"<Value>(.+?)<\/Value>", RegexOptions.Compiled);
        private readonly string _password;

        private readonly WebProxy _proxy;
        private readonly string _username;

        public LenovoUtils(string username, string password, LenovoBaseInfoModel baseInfo, WebProxy proxy = null)
        {
            _username = username;
            _password = password;
            _baseInfo = baseInfo;
            _baseInfoBase64 = Base64Encode(JsonConvert.SerializeObject(baseInfo));
            _proxy = proxy;
        }

        /// <summary>
        ///     登录联想账号
        /// </summary>
        /// <returns></returns>
        public string Login()
        {
            var url = $"https://uss.lenovomm.com/authen/1.2/tgt/user/get?msisdn={_username}";

            var requestBodyDict = new Dictionary<string, string>
            {
                {"lang", "zh-CN-#Hans"},
                {"source", Source},
                {"deviceidtype", "mac"},
                {"deviceid", _baseInfo.imei},
                {"devicecategory", "unknown"},
                {"devicevendor", _baseInfo.phonebrand},
                {"devicefamily", "unknown"},
                {"devicemodel", _baseInfo.phoneModel},
                {"osversion", _baseInfo.systemVersion},
                {"osname", "Android"},
                {"password", _password}
            };

            var requestHeaderDict = new Dictionary<string, string>
            {
                {"baseinfo", _baseInfoBase64},
                {"unique", _baseInfo.imei},
                {"token", ""},
                {"Authorization", ""}
            };

            var client = new RestClient(url)
            {
                Proxy = _proxy
            };
            var request = new RestRequest(Method.POST);
            client.UserAgent = UserAgent;

            request.AddHeaders(requestHeaderDict);

            foreach (var keyValuePair in requestBodyDict) request.AddParameter(keyValuePair.Key, keyValuePair.Value);
            var response = client.Execute(request);

            if (_getResultValueRegex.IsMatch(response.Content))
                return _getResultValueRegex.Match(response.Content).Groups[1].Value;

            throw new Exception("登录失败");
        }

        /// <summary>
        ///     获取Token
        /// </summary>
        /// <param name="lpsust"></param>
        /// <returns></returns>
        public string GetToken(string lpsust)
        {
            var url =
                $"https://uss.lenovomm.com/authen/1.2/st/get?lpsutgt={lpsust}&source={Source}&lang=zh-CN&realm=club.lenovo.com.cn";

            var client = new RestClient(url)
            {
                Proxy = _proxy
            };
            var request = new RestRequest(Method.GET);
            client.UserAgent = UserAgent;

            var response = client.Execute(request);

            if (_getResultValueRegex.IsMatch(response.Content))
                return _getResultValueRegex.Match(response.Content).Groups[1].Value;

            throw new Exception("获取Token失败");
        }

        /// <summary>
        ///     获取SessionId
        /// </summary>
        /// <param name="lpsust"></param>
        /// <returns></returns>
        public LenovoSessionIdResultModel GetSessionId(string lpsust)
        {
            var url =
                $"https://api.club.lenovo.cn/users/getSessionID?s={BuildSValue()}";

            var client = new RestClient(url)
            {
                Proxy = _proxy
            };
            var request = new RestRequest(Method.GET);
            client.UserAgent = UserAgent;

            request.AddHeader("Authorization", $"Lenovosso {lpsust}");
            request.AddHeader("token", lpsust);
            request.AddHeader("BaseInfo", _baseInfoBase64);

            var response = client.Execute(request);

            return JsonConvert.DeserializeObject<LenovoSessionIdResultModel>(response.Content);
        }

        /// <summary>
        ///     每日签到
        /// </summary>
        /// <param name="lenovoId"></param>
        /// <param name="sessionId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public LenovoSignResultModel DaySign(long lenovoId, string sessionId, string token)
        {
            var url = "https://api.club.lenovo.cn/signin/v2/add";

            var client = new RestClient(url)
            {
                Proxy = _proxy
            };
            var request = new RestRequest(Method.POST);
            client.UserAgent = UserAgent;

            request.AddParameter("imei", _baseInfo.imei);
            request.AddParameter("uid", lenovoId);

            request.AddHeader("Authorization", $"Lenovo {sessionId}");
            request.AddHeader("token", token);
            request.AddHeader("BaseInfo", _baseInfoBase64);

            var response = client.Execute(request);

            return JsonConvert.DeserializeObject<LenovoSignResultModel>(response.Content);
        }

        /// <summary>
        ///     构建获取SessionId用的s参数
        /// </summary>
        /// <returns></returns>
        private static string BuildSValue()
        {
            var s = new StringBuilder();
            s.Append("8091b7729079682e9a58f609035619625b984e3a2d2b229caf45c7108b372218");
            foreach (var i in Enumerable.Range(1, 16)) s.Append(Guid.NewGuid().ToString("N"));

            return s.ToString();
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
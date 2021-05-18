using CommandLine;
using System;

namespace LenovoSign
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string username = null;
            string password = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    username = o.username;
                    password = o.password;
                });

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("用户名密码必须提供");
                return;
            }

            var baseInfoModel = new LenovoBaseInfoModel
            {
                imei = "16213049994264620",
                phoneincremental = "eng.073325",
                phoneproduct = "havoc",
                phonedisplay = "QQ3A.25.001",
                appVersion = "V5.0.1",
                phoneModel = "Redmi 6 Plus",
                lenovoClubChannel = "xiaomi",
                phonebrand = "Xiaomi",
                androidsdkversion = "29",
                loginName = "",
                phoneManufacturer = "Xiaomi",
                systemVersion = "10"
            };

            var t = new LenovoUtils(username, password, baseInfoModel);
            var step1 = t.Login();
            var step2 = t.GetToken(step1);
            var step3 = t.GetSessionId(step2);
            var step4 = t.DaySign(step3.Res.Lenovoid, step3.Res.Sessionid, step3.Res.Token);

            if (step4.Res.Success) Console.WriteLine("签到成功");
            Console.WriteLine($"签到结果:{step4.Res.RewardTips}");

            Console.WriteLine();
        }

        public class Options
        {
            [Option('u', "username", Required = true, HelpText = "用户名")]
            public string username { get; set; }

            [Option('p', "password", Required = true, HelpText = "密码")]
            public string password { get; set; }
        }
    }
}
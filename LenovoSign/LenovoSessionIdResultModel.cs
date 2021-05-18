namespace LenovoSign
{
    public class LenovoSessionIdResultModel
    {
        public long Status { get; set; }
        public Res Res { get; set; }
        public long Time { get; set; }
    }

    public partial class Res
    {
        public string Cookiekey { get; set; }
        public long Lenovoid { get; set; }
        public string CerpPassport { get; set; }
        public string Cookieval { get; set; }
        public string Sessionid { get; set; }
        public string Token { get; set; }
    }
}
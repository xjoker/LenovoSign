using System;
using System.Collections.Generic;

namespace LenovoSign
{
    public class LenovoSignResultModel
    {
        public long Status { get; set; }
        public Res Res { get; set; }
        public long Time { get; set; }
    }

    public partial class Res
    {
        public bool Success { get; set; }
        public long ContinueCount { get; set; }
        public long YanbaoValue { get; set; }
        public long LedouValue { get; set; }
        public long ScoreValue { get; set; }
        public List<object> CouponValue { get; set; }
        public string RewardTips { get; set; }
        public string RewardSubTips { get; set; }
        public string CouponUrl { get; set; }
        public Uri DetailUrl { get; set; }
    }
}
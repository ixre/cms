using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Cms.Infrastructure
{
    public class DateHelper
    {
        public static long ToUnix(DateTime time)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var t = (time.Ticks - startTime.Ticks) / 10000; //除10000调整为13位
            return t;
        }
    }
}
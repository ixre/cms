using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T2.Cms.Infrastructure
{
    public class DateHelper
    {
        public static long ToUnix(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
            return t;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andon
{
    class UTC
    {
        public static int ConvertDateTimeLong(DateTime Time)
        {
            double doubleResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime endTime = Time;
            doubleResult = (endTime - startTime).TotalSeconds;
            return (int)(doubleResult);
        }

        public static DateTime ConvertLongDateTime(int UTCTime)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            time = startTime.AddSeconds(UTCTime);
            return time;
        }
    }
}

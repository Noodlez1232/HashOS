using Cosmos.HAL;
using Sys = Cosmos.System;

namespace HashOS.Drivers
{
    public class Time
    {
        public int Hour() { return RTC.Hour; }
        public int Minute() { return RTC.Minute; }
        public int Second() { return RTC.Second; }
        public int Day() { return RTC.DayOfTheMonth; }
        public int Month() { return RTC.Month; }
        public int Year() { return RTC.Year; }
        public int DayOfTheWeek() { return RTC.DayOfTheWeek; }
        public string getTime24(bool hour, bool min, bool sec)
        {
            string timeStr = "";
            if (hour) { timeStr += Hour().ToString(); }
            if (min)
            {
                timeStr += ":";
                timeStr += Minute().ToString();
            }
            if (sec)
            {
                timeStr += ":";
                timeStr += Second().ToString();
            }
            return timeStr;
        }
        public string getTime12(bool hour, bool min, bool sec)
        {
            string timeStr = "";
            if (hour)
            {
                if (Hour() > 12)
                    timeStr += Hour() - 12;
                else
                    timeStr += Hour();
            }
            if (min)
            {
                timeStr += ":";
                timeStr += Minute().ToString();
            }
            if (sec)
            {
                timeStr += ":";
                timeStr += Second().ToString();
            }
            if (hour)
            {
                if (Hour() > 12)
                    timeStr += " PM";
                else
                    timeStr += " AM";
            }
            return timeStr;
        }
    }
}

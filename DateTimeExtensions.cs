using System;

namespace ConsoleApp1
{
    public static class DateTimeExtensions
    {
        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }


    }
}

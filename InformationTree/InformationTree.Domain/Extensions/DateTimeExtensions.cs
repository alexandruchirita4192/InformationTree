using System;

namespace InformationTree.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFormattedString(this DateTime dateTime)
        {
            return dateTime.ToString(Constants.DateTimeFormats.DateTimeFormatSeparatedWithDot);
        }
    }
}
using System;
using System.Globalization;
using System.IO;

namespace InformationTree.Domain.Extensions
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNotEmpty(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        public static DateTime? ToDateTime(this string s, NLog.Logger _logger)
        {
            var convertedDateTime = (DateTime?)null;
            if (s.IsNotEmpty())
            {
                try { convertedDateTime = DateTime.ParseExact(s, Constants.DateTimeFormats.DateTimeFormatSeparatedWithDot, CultureInfo.InvariantCulture); } catch (Exception ex) { _logger?.Error(ex); }
                if (convertedDateTime == null)
                    try { convertedDateTime = DateTime.ParseExact(s, Constants.DateTimeFormats.DateTimeFormatSeparatedWithSlash, CultureInfo.InvariantCulture); } catch (Exception ex) { _logger?.Error(ex); }
                if (convertedDateTime == null)
                    try { convertedDateTime = DateTime.Parse(s); } catch (Exception ex) { _logger?.Error(ex); }
            }
            return convertedDateTime;
        }
    }
}
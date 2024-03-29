﻿using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using NLog;

namespace InformationTree.Domain.Extensions
{
    public static class StringExtensions
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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

        public static string GetToolTipText(this string text, int linesCount = 10, int charsCount = 200)
        {
            try
            {
                var lines = text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > linesCount)
                    return string.Join(Environment.NewLine, lines.Take(linesCount)) + Environment.NewLine + "[...]";
                else
                    return string.Join(Environment.NewLine, lines) + Environment.NewLine;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            if (text.Length <= charsCount)
                return text;

            return new string(text.Take(charsCount).ToArray()) + "[...]";
        }

        public static bool StartsWithSignature(this string testString, string signature)
        {
            return testString.IsNotEmpty()
                && testString.Trim()
                .StartsWith(signature);
        }

        public static bool IsRichText(this string testString)
        {
            return testString.StartsWithSignature("{\\rtf");
        }

        public static bool IsPgpEncrypted(this string testString)
        {
            return testString.StartsWithSignature("-----BEGIN PGP");
        }

        public static string RemoveEndLines(this string text)
        {
            return text
                ?.Replace("\n", string.Empty)
                ?.Replace("\r", string.Empty)
                ?? string.Empty;
        }

        public static Color? ToColor(this string colorName)
        {
            if (colorName.IsEmpty())
                return null;
            if (Enum.TryParse<KnownColor>(colorName, true, out var knownColor) == false)
                return null;

            var color = Color.FromKnownColor(knownColor);
            return color;
        }
    }
}
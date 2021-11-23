using InformationTree.PgpEncryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace InformationTree.TextProcessing
{
    public static class TextProcessingHelper
    {
        public static string GetTextAndProcentCompleted(string attrText, ref decimal attrPercentCompleted, bool getTextWithoutProgress = false)
        {
            // The format of text is:
            // 1. Text [X% completed]
            // 2. Text text2 text3 ][[ [X% completed]
            // Where X is attrPercentCompleted
            var words = attrText.Trim().Split('[');
            if (words.Length < 2)
            {
            }
            else if (words.Length == 2) // 1. Text [X% completed]
            {
                attrText = words[0].TrimEnd(); // fix attrText to be simple
                try
                {
                    attrPercentCompleted = decimal.Parse(words[1].Split('%')[0]); // fix attrPercentCompleted from text (if necessary)
                }
                catch // either there's no % or there's no number
                {
                    attrText = string.Join("[", words); // fix it
                }
            }
            else if (words.Length > 2) // 2. Text text2 text3 ][[ [X% completed]
            {
                try
                {
                    attrPercentCompleted = decimal.Parse(words[words.Length - 1].Split('%')[0]);
                    attrText = string.Join("[", words.Except(new List<string>() { words[words.Length - 1] })).Trim();
                }
                catch // either there's no % or there's no number
                {
                    attrText = string.Join("[", words); // fix it
                }
            }

            if (attrPercentCompleted < 0)
                attrPercentCompleted = 0;
            else if (attrPercentCompleted > 100)
                attrPercentCompleted = 100;

            if (!getTextWithoutProgress)
                attrText = attrText.Trim() + " [" + attrPercentCompleted.ToString() + "% completed]";
            attrText = attrText.Trim();

            return attrText;
        }

        public static string UpdateTextAndProcentCompleted(string attrText, ref decimal attrPercentCompleted, bool getTextWithoutProgress = false)
        {
            if (attrPercentCompleted < 0)
                attrPercentCompleted = 0;
            else if (attrPercentCompleted > 100)
                attrPercentCompleted = 100;

            var words = attrText.Trim().Split('[');
            if (words.Length < 2)
            {
            }
            else if (words.Length == 2)
                attrText = words[0].TrimEnd(); // fix attrText to be simple
            else if (words.Length > 2)
            {
                try
                {
                    attrText = string.Join("[", words.Except(new List<string>() { words[words.Length - 1] }));
                }
                catch // either there's no % or there's no number
                {
                    attrText = string.Join("[", words); // fix it
                }
            }

            if (!getTextWithoutProgress)
                attrText = attrText.Trim() + " [" + attrPercentCompleted.ToString() + "% completed]";
            attrText = attrText.Trim();

            return attrText;
        }

        public static string GetToolTipText(string text, int linesCount = 10, int charsCount = 200)
        {
            try
            {
                var lines = text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > linesCount)
                    return string.Join(Environment.NewLine, lines.Take(linesCount)) + Environment.NewLine + "[...]";
                else
                    return string.Join(Environment.NewLine, lines) + Environment.NewLine;
            }
            catch { }

            if (text.Length <= charsCount)
                return text;

            return new String(text.Take(charsCount).ToArray()) + "[...]";
        }

        public static string GetDecompressedData(string data)
        {
            if (String.IsNullOrEmpty(data))
                return data;

            try
            {
                var urlDecodedBytes = HttpServerUtility.UrlTokenDecode(data);
                if (urlDecodedBytes == null)
                    return data;

                using (var outputStream = Decompress(urlDecodedBytes))
                using (var sr = new StreamReader(outputStream))
                    return sr.ReadToEnd();
            }
            catch { }

            return data;
        }

        public static string GetCompressedData(string data)
        {
            if (String.IsNullOrEmpty(data))
                return data;

            try
            {
                using (var inputStream = PGPEncryptDecrypt.GenerateStreamFromString(data))
                {
                    var result = Compress(inputStream);
                    var urlEncodedResult = HttpServerUtility.UrlTokenEncode(result);
                    var integrityCheck = result.SequenceEqual(HttpServerUtility.UrlTokenDecode(urlEncodedResult));
                    if (integrityCheck && (urlEncodedResult.Length < data.Length))
                        return urlEncodedResult;
                }
            }
            catch { }

            return data;
        }

        private static byte[] Compress(Stream input)
        {
            using (var compressStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
            {
                input.CopyTo(compressor);
                compressor.Close();
                return compressStream.ToArray();
            }
        }

        private static Stream Decompress(byte[] input)
        {
            var output = new MemoryStream();

            using (var compressStream = new MemoryStream(input))
            using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
                decompressor.CopyTo(output);

            output.Position = 0;
            return output;
        }
    }
}
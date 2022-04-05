using InformationTree.PgpEncryption;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace InformationTree.TextProcessing
{
    [Obsolete("Break this into several classes")]
    // TODO: Split class into it's purposes: TreeNodeData (or text) processing (like an extension of TreeNodeData??), Compression/Decompression feature
    public static class TextProcessingHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

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
                var urlDecodedBytes = UrlTokenDecode(data);
                if (urlDecodedBytes == null)
                    return data;

                using (var outputStream = Decompress(urlDecodedBytes))
                using (var sr = new StreamReader(outputStream))
                    return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return data;
        }

        public static string GetCompressedData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            try
            {
                using (var inputStream = PGPEncryptDecrypt.GenerateStreamFromString(data))
                {
                    var result = Compress(inputStream);
                    var urlEncodedResult = UrlTokenEncode(result);
                    var integrityCheck = result.SequenceEqual(UrlTokenDecode(urlEncodedResult));
                    if (integrityCheck && (urlEncodedResult.Length < data.Length))
                        return urlEncodedResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

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

        // TODO: Cleanup?? HttpServerUtility.UrlTokenEncode copy-paste code from Microsoft... ( https://stackoverflow.com/questions/50731397/httpserverutility-urltokenencode-replacement-for-netstandard )
        private static string UrlTokenEncode(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (input.Length < 1)
                return String.Empty;
            char[] base64Chars = null;

            ////////////////////////////////////////////////////////
            // Step 1: Do a Base64 encoding
            string base64Str = Convert.ToBase64String(input);
            if (base64Str == null)
                return null;

            int endPos;
            ////////////////////////////////////////////////////////
            // Step 2: Find how many padding chars are present in the end
            for (endPos = base64Str.Length; endPos > 0; endPos--)
            {
                if (base64Str[endPos - 1] != '=') // Found a non-padding char!
                {
                    break; // Stop here
                }
            }

            ////////////////////////////////////////////////////////
            // Step 3: Create char array to store all non-padding chars,
            //      plus a char to indicate how many padding chars are needed
            base64Chars = new char[endPos + 1];
            base64Chars[endPos] = (char)((int)'0' + base64Str.Length - endPos); // Store a char at the end, to indicate how many padding chars are needed

            ////////////////////////////////////////////////////////
            // Step 3: Copy in the other chars. Transform the "+" to "-", and "/" to "_"
            for (int iter = 0; iter < endPos; iter++)
            {
                char c = base64Str[iter];

                switch (c)
                {
                    case '+':
                        base64Chars[iter] = '-';
                        break;

                    case '/':
                        base64Chars[iter] = '_';
                        break;

                    case '=':
                        Debug.Assert(false);
                        base64Chars[iter] = c;
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }
            return new string(base64Chars);
        }

        // TODO: Cleanup?? HttpServerUtility.UrlTokenDecode copy-paste code from Microsoft... ( https://stackoverflow.com/questions/50731397/httpserverutility-urltokenencode-replacement-for-netstandard )
        private static byte[] UrlTokenDecode(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int len = input.Length;
            if (len < 1)
                return new byte[0];

            ///////////////////////////////////////////////////////////////////
            // Step 1: Calculate the number of padding chars to append to this string.
            //         The number of padding chars to append is stored in the last char of the string.
            int numPadChars = (int)input[len - 1] - (int)'0';
            if (numPadChars < 0 || numPadChars > 10)
                return null;


            ///////////////////////////////////////////////////////////////////
            // Step 2: Create array to store the chars (not including the last char)
            //          and the padding chars
            char[] base64Chars = new char[len - 1 + numPadChars];


            ////////////////////////////////////////////////////////
            // Step 3: Copy in the chars. Transform the "-" to "+", and "*" to "/"
            for (int iter = 0; iter < len - 1; iter++)
            {
                char c = input[iter];

                switch (c)
                {
                    case '-':
                        base64Chars[iter] = '+';
                        break;

                    case '_':
                        base64Chars[iter] = '/';
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            ////////////////////////////////////////////////////////
            // Step 4: Add padding chars
            for (int iter = len - 1; iter < base64Chars.Length; iter++)
            {
                base64Chars[iter] = '=';
            }

            // Do the actual conversion
            return Convert.FromBase64CharArray(base64Chars, 0, base64Chars.Length);
        }
    }
}
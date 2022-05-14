using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using NLog;

namespace InformationTree.Render.WinForms.Services
{
    public class CompressionProvider : ICompressionProvider
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public string Decompress(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            try
            {
                byte[] urlDecodedBytes;
                
                try
                {
                    urlDecodedBytes = Convert.FromBase64String(data); // Used instead of UrlTokenDecode
                }
                catch (FormatException)
                {
                    // Use the legacy code if the new code fails
                    urlDecodedBytes = UrlTokenDecode(data);
                }
                
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

        public string Compress(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            try
            {
                using (var inputStream = data.ToStream())
                {
                    var result = Compress(inputStream);
                    var urlEncodedResult = Convert.ToBase64String(result); // used instead of UrlTokenEncode
                    var dataIntegrityCheck = result.SequenceEqual(Convert.FromBase64String(urlEncodedResult)); // Used instead of UrlTokenDecode
                    
                    // Use compressed result only if the result is smaller and data has integrity
                    if (dataIntegrityCheck && (urlEncodedResult.Length < data.Length))
                        return urlEncodedResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return data;
        }

        /// <summary>
        /// Compression returns a byte array which later needs to be encoded.
        /// </summary>
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

        /// <summary>
        /// Decompression returns a stream which later needs to be read.
        /// </summary>
        private static Stream Decompress(byte[] input)
        {
            var output = new MemoryStream();

            using (var compressStream = new MemoryStream(input))
            using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
                decompressor.CopyTo(output);

            output.Position = 0;
            return output;
        }

        // TODO: Cleanup after a few versions
        private static byte[] UrlTokenDecode(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

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
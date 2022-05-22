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

                urlDecodedBytes = Convert.FromBase64String(data); // Used instead of UrlTokenDecode

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
    }
}
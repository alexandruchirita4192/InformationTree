﻿using System.IO;

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
    }
}
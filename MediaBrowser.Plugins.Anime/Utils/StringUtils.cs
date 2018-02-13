using System;
using System.Collections.Generic;
using System.Text;

namespace MediaBrowser.Plugins.Anime.Utils
{
    public static class StringUtils {
        const string _decodeFallbackString = " ";
        static readonly string _decodeFallbackClearPattern = string.Concat(_decodeFallbackString, _decodeFallbackString);

        static Encoding asciiEncoding = null;

        static StringUtils() {
            asciiEncoding = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderReplacementFallback(_decodeFallbackString), new DecoderReplacementFallback(_decodeFallbackString));
        }

        public static string filterUnicodeArt(string input)
        {
            string stringToClean = asciiEncoding.GetString(asciiEncoding.GetBytes(input));
            int oldLen = 0;
            int newLen = stringToClean.Length;

            do {
                oldLen = newLen;
                stringToClean = stringToClean.Replace(_decodeFallbackClearPattern, _decodeFallbackString);
                newLen = stringToClean.Length;
            } while (oldLen != newLen);

            return stringToClean;
        }
    }
}

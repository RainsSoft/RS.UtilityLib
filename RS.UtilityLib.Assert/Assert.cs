namespace RS.UtilityLib.AssertLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Asserts that some condition is met.
    /// </summary>
    public static partial class Assert
    {
        private static Action<bool, string> Check;

        static Assert()
        {
            UseDebug();
        }

        /// <summary>
        /// Use System.Diagnostics.Debug.Assert.
        /// </summary>
        public static void UseDebug()
        {
            Check = (condition, message) => Debug.Assert(condition, message);
        }

        /// <summary>
        /// Use System.Diagnostics.Trace.Assert.
        /// </summary>
        public static void UseTrace()
        {
            Check = (condition, message) => Trace.Assert(condition, message);
        }

        /// <summary>
        /// Throws AssertLibrary.AssertException when needed.
        /// </summary>
        public static void UseException()
        {
            Check = (condition, message) =>
            {
                if (!condition)
                    throw new AssertException(message);
            };
        }

        #region extend1
        public static string Join(this IEnumerable<string> strings, string delimiter) {
            StringBuilder sb = new StringBuilder();
            bool delimit = false;
            foreach (string str in strings) {
                if (delimit)
                    sb.Append(delimiter);
                else
                    delimit = true;

                sb.Append(str);
            }
            return sb.ToString();
        }
        public static bool None<T>(this IEnumerable<T> enumerable) {
            return !enumerable.Any();
        }

        public static string NullIfEmpty(this string value) {
            return value == string.Empty
                ? null
                : value;
        }

        public static int IndexOfOrDefault<T>(this IEnumerable<T> sequence, Func<T, bool> predicate, int startIndex, int defaultValue) {
            int i = startIndex;
            foreach (T item in sequence.Skip(startIndex)) {
                if (predicate(item))
                    return i;
                i++;
            }
            return defaultValue;
        }
        #endregion
        #region extend2
        private static readonly Random random = new Random();
        public static string RandomString(int length = 10) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var content = new char[length];
            for (int i = 0; i < length; i++) {
                content[i] = chars[random.Next(chars.Length)];
            }

            return new string(content);
        }
        private static char sensitive = '*';
        private static char at = '@';
        private static readonly Regex UrlizeRegex = new Regex(@"[^A-Za-z0-9_~]+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled);
        public static string UrlEncode(this string url) {
            return Uri.EscapeDataString(url);
        }
        public static bool IsEmail(this string field) {
            // Return true if strIn is in valid e-mail format.
            bool IsPresent = !(string.IsNullOrEmpty(field)||string.IsNullOrEmpty(field.Trim()));
            //IsPresent = field.IsPresent();
            return IsPresent && EmailRegex.IsMatch(field);
        }
        private static string UrlCombine(string path1, string path2) {
            path1 = path1.TrimEnd('/') + "/";
            path2 = path2.TrimStart('/');

            return Path.Combine(path1, path2)
                .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        public static string UrlPathCombine(this string path1, params string[] path2) {
            path1 = path1.TrimEnd('/') + "/";
            foreach (var s in path2) {
                path1 = UrlCombine(path1, s).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            return path1;

        }
        public static string AddSpacesToSentence(this string state) {
            var text = state.ToCharArray();
            var chars = new char[text.Length + HowManyCapitalizedChars(text) - 1];

            chars[0] = text[0];
            int j = 1;
            for (int i = 1; i < text.Length; i++) {
                if (char.IsUpper(text[i])) {
                    if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1]) ||
                        (char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1]))) {
                        chars[j++] = ' ';
                        chars[j++] = text[i];
                        continue;
                    }
                }

                chars[j++] = text[i];
            }
#if NETSTANDARD2_0
            return new string(chars);
#else
            return new string(chars);
#endif
        }

        private static int HowManyCapitalizedChars(char[] state) {
            var count = 0;
            for (var i = 0; i < state.Length; i++) {
                if (char.IsUpper(state[i]))
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Replace everything to ***, except the first and last char
        /// </summary>
        /// <returns></returns>
        public static string TruncateSensitiveInformation(this string part) {
            bool IsPresent = !(string.IsNullOrEmpty(part) || string.IsNullOrEmpty(part.Trim()));
            if (IsPresent) {
                var truncatedString = new char[part.Length];
                truncatedString[0] = part[0];


                for (var i = 1; i < part.Length - 1; i++) {
                    truncatedString[i] = sensitive;
                }
                truncatedString[part.Length - 1] = part[part.Length - 1];

                return new string(truncatedString);
            }

            return string.Empty;
        }
        public static string ToSha256(this string value) {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(value));
            foreach (byte theByte in crypto) {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
        public static string OnlyNumbers(this string str) {
            var onlyNumbers = new char[str.Length];
            var lastIndex = 0;

            foreach (var c in str) {
                if (c < '0' || c > '9') continue;

                onlyNumbers[lastIndex++] = c;
            }
            Array.Resize(ref onlyNumbers, lastIndex);
            return new string(onlyNumbers);
        }

        public static byte[] FromPlainHexDumpStyleToByteArray(this string hex) {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i) {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }
        private static int GetHexVal(char hex) {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
        #endregion
    }
}

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Common string utilities.
	/// </summary>
	internal static class StringUtils
	{
		/// <summary>
		///   Compute hash code for the specified string value as string.
		/// </summary>
		/// <param name="value">string value to compute hash code from</param>
		/// <param name="hashType">hash algorithm to use</param>
		/// <returns>hash code for the specified string value</returns>
		public static string ComputeStringHash(string value, HashType hashType) {
			return StringUtils.GetHexString(StringUtils.ComputeByteHash(value, hashType));
		}

		/// <summary>
		///   Compute numerically represented hash code for the specified string value.
		/// </summary>
		/// <param name="value">string value to compute hash code from</param>
		/// <returns>hash code for the specified string value</returns>
		public static long ComputeSignedNumericHash(string value) {
			Checks.CheckNotNull(value, "value");
			return BitConverter.ToInt64(StringUtils.ComputeByteHash(value, HashType.Md5), 0);
		}

		/// <summary>
		///   Compute numerically represented hash code for the specified string value.
		/// </summary>
		/// <param name="value">string value to compute hash code from</param>
		/// <returns>hash code for the specified string value</returns>
		/// <remarks>
		///   Return value can be converted to ulong, but returned decimal for CLS compliance.
		/// </remarks>
		public static decimal ComputeUnsignedNumericHash(string value) {
			Checks.CheckNotNull(value, "value");
			return BitConverter.ToUInt64(StringUtils.ComputeByteHash(value, HashType.Md5), 0);
		}

		/// <summary>
		///   Compute hash code for the specified string value as array of bytes.
		/// </summary>
		/// <param name="value">string value to compute hash code from</param>
		/// <param name="hashType">hash algorithm to use</param>
		/// <returns>hash code for the specified string value</returns>
		public static byte[] ComputeByteHash(string value, HashType hashType) {
			HashAlgorithm hashAlgorithm = StringUtils.GetHashAlgorithm(hashType);
			byte[] result = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(value));
			hashAlgorithm.Clear();
			return result;
		}

		/// <summary>
		///   Get string of hexadecimal numbers representing the specified byte array.
		/// </summary>
		/// <param name="bytes">input byte array</param>
		/// <returns>string of hexadecimal numbers representing the specified byte array</returns>
		public static string GetHexString(byte[] bytes) {
			Checks.CheckCollection(bytes, "bytes");
			return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
		}

		/// <summary>
		///   Get unique name (string that does not include one of the strings specified in names collection).
		/// </summary>
		/// <param name="names">strings that should be different of the unique string</param>
		/// <param name="prefix">unique name prefix</param>
		/// <param name="suffix">unique name suffix</param>
		/// <param name="caseSensitive">use case-sensitive comparison</param>
		/// <returns>string that does not include one of the strings specified in names collection</returns>
		public static string GetUniqueName(ReadOnlyCollection<string> names, string prefix, string suffix, bool caseSensitive) {
			bool isUnique = false;
			return StringUtils.GetUniqueName(names, prefix, suffix, caseSensitive, ref isUnique);
		}

		/// <summary>
		///   Get unique name (string that does not include one of the strings specified in names collection).
		/// </summary>
		/// <param name="names">strings that should be different of the unique string</param>
		/// <param name="prefix">unique name prefix</param>
		/// <param name="suffix">unique name suffix</param>
		/// <param name="caseSensitive">use case-sensitive comparison</param>
		/// <param name="isUnique">the specified name is already unique</param>
		/// <returns>string that does not include one of the strings specified in names collection</returns>
		public static string GetUniqueName(ReadOnlyCollection<string> names, string prefix, string suffix, bool caseSensitive, ref bool isUnique) {
			Checks.CheckNotNull(names, "names");
			Checks.CheckNotNull(prefix, "prefix");
			Checks.CheckNotNull(suffix, "suffix");
			Set<string> set = new Set<string>();
			string text = null;
			string text2 = null;
			if (caseSensitive) {
				foreach (string name in names) {
					set.Add(name);
				}
				text = prefix;
				text2 = suffix;
			}
			else {
				foreach (string name2 in names) {
					set.Add(name2.ToUpperInvariant());
				}
				text = prefix.ToUpperInvariant();
				text2 = suffix.ToUpperInvariant();
			}
			string empty = string.Empty;
			empty = text + text2;
			if (!set.Contains(empty)) {
				isUnique = true;
				return prefix + suffix;
			}
			isUnique = false;
			int num = 1;
			while (true) {
				empty = string.Format("{0}{1}{2}", new object[3] { text, num, text2 });
				if (!set.Contains(empty)) {
					break;
				}
				num++;
			}
			return string.Format("{0}{1}{2}", new object[3] { prefix, num, suffix });
		}

		/// <summary>
		///   Normalize string in a standard way.
		/// </summary>
		/// <param name="value">string value to normalize</param>
		/// <returns>normalized string</returns>
		public static string Normalize(string value) {
			Checks.CheckNotNull(value, "value");
			return value.Trim().ToUpperInvariant();
		}

		/// <summary>
		///   Remove accent marks from the string.
		/// </summary>
		/// <param name="value">string value</param>
		/// <returns>string value with accent marks removed</returns>
		public static string RemoveAccents(string value) {
			Checks.CheckNotNull(value, "value");
			string text = value.Normalize(NormalizationForm.FormD);
			StringBuilder stringBuilder = new StringBuilder();
			string text2 = text;
			foreach (char c in text2) {
				if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) {
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		///   Remove white space characters from string.
		/// </summary>
		/// <param name="value">string to remove white space characters from</param>
		/// <returns>string without white space characters</returns>
		public static string RemoveWhitespace(string value) {
			Checks.CheckNotNull(value, "value");
			for (int i = 0; i < value.Length; i++) {
				if (char.IsWhiteSpace(value, i)) {
					value = value.Remove(i, 1);
				}
			}
			return value;
		}

		/// <summary>
		///   Get initialized HashAlgorithm instance.
		/// </summary>
		/// <param name="hashType">hash algorithm to use</param>
		/// <returns>HashAlgorithm instance</returns>
		internal static HashAlgorithm GetHashAlgorithm(HashType hashType) {
			//HashAlgorithm hashAlgorithm = hashType switch {
			//	HashType.Md5 => MD5.Create(),
			//	HashType.Sha1 => SHA1.Create(),
			//	_ => throw new ApplicationException("Unknown hash algorithm."),
			//};
			HashAlgorithm hashAlgorithm;
			switch (hashType) {
				case HashType.Md5:
					hashAlgorithm = MD5.Create();
					break;
				case HashType.Sha1:
					hashAlgorithm= SHA1.Create();
					break;
				default:throw new ApplicationException("Unknown hash algorithm.");
					break;
			}
            hashAlgorithm.Initialize();
			return hashAlgorithm;
		}

		/// <summary>
		///   Get length of a hash value for the specified hash algorithm (in bytes).
		/// </summary>
		/// <param name="hashType">hash algorithm</param>
		/// <returns>hash value length</returns>
		internal static int GetHashSize(HashType hashType) {
			//return hashType switch {
			//	HashType.Md5 => 16,
			//	HashType.Sha1 => 20,
			//	_ => throw new ApplicationException("Unknown hash algorithm."),
			//};
			switch (hashType) {
				case HashType.Md5:
					return 16;
					break;
				case HashType.Sha1:
					return 20;
					break;
				default:
					throw new ApplicationException("Unknown hash algorithm.");
					break;
			}
		}
	}
}
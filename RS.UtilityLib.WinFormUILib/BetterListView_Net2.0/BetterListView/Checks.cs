using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Custom assertions.
	/// </summary>
	/// <summary>
	///   Extended assertions.
	/// </summary>
	internal static class Checks
	{
		/// <summary>
		///   additional memory amount for keeping some free memory reserve
		/// </summary>
		private const long SafeMemoryOverhead = 16777216L;

		private const char Base64Padding = '=';

		private const string RegexPath = "^(?<1>.*[\\\\/])(?<2>.+)\\.(?<3>.+)?$|^(?<1>.*[\\\\/])(?<2>.+)$|^(?<2>.+)\\.(?<3>.+)?$|^(?<2>.+)$";

		private const string RegexUrl = "(([a-zA-Z][0-9a-zA-Z+\\-\\.]*:)?/{0,2}[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?(#[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?";

		private const string RegexEMail = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

		private static readonly Set<char> invalidFilenameChars;

		private static readonly Set<char> Base64Characters;

		/// <summary>
		///   Check if the boolean expression has right value.
		/// </summary>
		/// <param name="value">value of the boolean expression</param>
		/// <param name="rightValue">right value of the boolean expression</param>
		/// <param name="expression">string representation of the expression</param>
		public static void CheckBool(bool value, bool rightValue, string expression) {
			Checks.CheckBool(value, rightValue, expression, string.Empty);
		}

		/// <summary>
		///   Check if the boolean expression has right value.
		/// </summary>
		/// <param name="value">value of the boolean expression</param>
		/// <param name="rightValue">right value of the boolean expression</param>
		/// <param name="expression">string representation of the expression</param>
		/// <param name="message">exception message</param>
		public static void CheckBool(bool value, bool rightValue, string expression, string message) {
			if (value != rightValue) {
				StringBuilder stringBuilder = new StringBuilder();
				if (message.Length != 0) {
					stringBuilder.AppendLine(message);
				}
				stringBuilder.AppendFormat("'{0}' should be {1}.", expression, rightValue);
				throw new ArgumentException(stringBuilder.ToString());
			}
		}

		/// <summary>
		///   Check if the boolean expression is False.
		/// </summary>
		/// <param name="value">value of the boolean expression</param>
		/// <param name="expression">string representation of the expression</param>
		public static void CheckFalse(bool value, string expression) {
			Checks.CheckFalse(value, expression, string.Empty);
		}

		/// <summary>
		///   Check if the boolean expression is False.
		/// </summary>
		/// <param name="value">value of the boolean expression</param>
		/// <param name="expression">string representation of the expression</param>
		/// <param name="message">exception message</param>
		public static void CheckFalse(bool value, string expression, string message) {
			Checks.CheckBool(value, rightValue: false, expression, message);
		}

		/// <summary>
		///   Check if the boolean expression is True.
		/// </summary>
		/// <param name="value">value of the boolean expression</param>
		/// <param name="expression">string representation of the expression</param>
		public static void CheckTrue(bool value, string expression) {
			Checks.CheckTrue(value, expression, string.Empty);
		}

		/// <summary>
		///   Check if the boolean expression is True.
		/// </summary>
		/// <param name="value">value of the boolean expression</param>
		/// <param name="expression">string representation of the expression</param>
		/// <param name="message">exception message</param>
		public static void CheckTrue(bool value, string expression, string message) {
			Checks.CheckBool(value, rightValue: true, expression, message);
		}

		/// <summary>
		///   Check whether the array dimension lengths lay between minimum and maximum values.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="dimensions">number of dimensions</param>
		/// <param name="minValues">minimum dimension lengths</param>
		/// <param name="maxValues">maximum dimension lengths</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckArray(Array param, int dimensions, int[] minValues, int[] maxValues, string paramName) {
			Checks.CheckNotNull(param, paramName);
			if (param.Rank != dimensions) {
				throw new ArgumentException("Array '" + paramName + "' should have required number of dimensions.");
			}
			int num = 0;
			while (true) {
				if (num < dimensions) {
					int length = param.GetLength(num);
					if (length < minValues[num] || length > maxValues[num]) {
						break;
					}
					num++;
					continue;
				}
				return;
			}
			throw new ArgumentException("Size of some dimension in array '" + paramName + "' does not lay within allowed range.");
		}

		/// <summary>
		///   Check whether the parameter is not null, of type ICollection{T} and is not empty.
		/// </summary>
		/// <typeparam name="T">type of the collection items</typeparam>
		/// <param name="param">paramater</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckCollection<T>(object param, string paramName) {
			Checks.CheckType(param, typeof(ICollection<T>), paramName);
			if (((ICollection<T>)param).Count == 0) {
				throw new ArgumentException("Collection '" + paramName + "' should not be empty.");
			}
		}

		/// <summary>
		///   Check whether the parameter is not null, of type ICollection and is not empty.
		/// </summary>
		/// <param name="param">paramater</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckCollection(object param, string paramName) {
			Checks.CheckType(param, typeof(ICollection), paramName);
			if (((ICollection)param).Count == 0) {
				throw new ArgumentException("Collection '" + paramName + "' should not be empty.");
			}
		}

		/// <summary>
		///   Check whether the parameter is not null, of type ICollection, is not empty and has restricted number of elements.
		/// </summary>
		/// <param name="param">paramater</param>
		/// <param name="minCount">minimum number of elements in collection</param>
		/// <param name="maxCount">maximum number of elements in collection</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckCollection(object param, int minCount, int maxCount, string paramName) {
			Checks.CheckType(param, typeof(ICollection), paramName);
			int count = ((ICollection)param).Count;
			if (count < minCount || count > maxCount) {
				throw new ArgumentException($"Collection '{paramName}' has {count} elements, but should have the number of elements between {minCount} and {maxCount}");
			}
		}

		/// <summary>
		///   Check whether the collection contains specified element.
		/// </summary>
		/// <typeparam name="T">type of collection element</typeparam>
		/// <param name="param">collection that should contain specified element</param>
		/// <param name="value">element, that should be contained within collection</param>
		/// <param name="paramName">name of a collection</param>
		/// <param name="valueName">name of element</param>
		public static void CheckCollectionContains<T>(ICollection<T> param, T value, string paramName, string valueName) {
			Checks.CheckNotNull(param, paramName);
			Checks.CheckNotNull(value, valueName);
			if (!param.Contains(value)) {
				throw new ArgumentException("Collection '" + paramName + "' does not contain element '" + valueName + "'.");
			}
		}

		static Checks() {
			Checks.Base64Characters = new Set<char>
			{
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
			'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
			'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
			'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
			'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
			'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
			'8', '9', '+', '/'
		};
			Checks.invalidFilenameChars = new Set<char>();
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			char[] array = invalidFileNameChars;
			foreach (char item in array) {
				Checks.invalidFilenameChars.Add(item);
			}
		}

		/// <summary>
		///   Check whether the specified Padding parameter has non-negative values.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckPadding(Padding param, string paramName) {
			if (param.Left < 0 || param.Top < 0 || param.Right < 0 || param.Bottom < 0) {
				throw new ArgumentException($"Parameter '{paramName}' (value: '{param}') contains negative padding.");
			}
		}

		/// <summary>
		///   Check whether the specified Size parameter has non-negative values.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckSize(Size param, string paramName) {
			if (param.Width < 0 || param.Height < 0) {
				throw new ArgumentException($"Parameter '{paramName}' (value: '{param}') contains negative size.");
			}
		}

		/// <summary>
		///   Check whether the specified Rectangle parameter has non-negative values.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckRectangle(Rectangle param, string paramName) {
			if (param.Left < 0 || param.Top < 0 || param.Width < 0 || param.Height < 0) {
				throw new ArgumentException($"Parameter '{paramName}' (value: '{param}') contains rectangle with negative coordinates");
			}
		}

		/// <summary>
		///   Check whether the given parameter is equal to a specified value.
		/// </summary>
		/// <typeparam name="T">parameter type</typeparam>
		/// <param name="param">parameter</param>
		/// <param name="paramValue">parameter value</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="paramValueName">parameter value name</param>
		public static void CheckEqual<T>(T param, T paramValue, string paramName, string paramValueName) {
			Checks.CheckNotNull(param, paramName);
			if (!param.Equals(paramValue)) {
				throw new ArgumentException("Parameter '" + paramName + "' should be equal to '" + paramValueName + "'.");
			}
		}

		/// <summary>
		///   Check whether the given parameter is not equal to a specified value.
		/// </summary>
		/// <typeparam name="T">parameter type</typeparam>
		/// <param name="param">parameter</param>
		/// <param name="paramValue">parameter value</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="paramValueName">parameter value name</param>
		public static void CheckNotEqual<T>(T param, T paramValue, string paramName, string paramValueName) {
			Checks.CheckNotNull(param, paramName);
			if (param.Equals(paramValue)) {
				throw new ArgumentException("Parameter '" + paramName + "' should not be equal to '" + paramValueName + "'.");
			}
		}

		/// <summary>
		///   Check whether the directory path specified by string parameter does exist.
		/// </summary>
		/// <param name="path">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckDirectoryExists(string path, string paramName) {
			Checks.CheckPath(path, paramName);
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Attributes != (FileAttributes)(-1) && (fileInfo.Attributes & FileAttributes.Directory) != FileAttributes.Directory) {
				throw new ArgumentException(string.Format("Path specified in '{0}' is not a directory."));
			}
			if (!Directory.Exists(path)) {
				throw new FileNotFoundException("Path specified in '" + paramName + "' does not exist.");
			}
		}

		/// <summary>
		///   Check whether the directory path specified by string parameter does exist.
		/// </summary>
		/// <param name="path">parameter</param>
		/// <returns>the directory path specified by string parameter does exist</returns>
		public static bool CheckDirectoryExistsSafe(string path) {
			if (!Checks.CheckPathSafe(path)) {
				return false;
			}
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Attributes != (FileAttributes)(-1) && (fileInfo.Attributes & FileAttributes.Directory) != FileAttributes.Directory) {
				return false;
			}
			return Directory.Exists(path);
		}

		/// <summary>
		///   Check whether the file path specified by string parameter does exist.
		/// </summary>
		/// <param name="path">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckFileExists(string path, string paramName) {
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Attributes != (FileAttributes)(-1) && (fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
				throw new ArgumentException("Path specified in '" + paramName + "' is a directory.");
			}
			if (!File.Exists(path)) {
				throw new FileNotFoundException("Path specified in '" + paramName + "' does not exist.");
			}
		}

		/// <summary>
		///   Check whether MD5 hash code of the specified file is the same as the provided hash code.
		/// </summary>
		/// <param name="path">path to the input file</param>
		/// <param name="hash">suggested hash code of the file</param>
		public static void CheckFileHash(string path, string hash) {
			Checks.CheckFileExists(path, "path");
			Checks.CheckHashString(hash, HashType.Md5, "hash");
			string hashCode = null;
			if (!IOUtils.ComputeFileContentHash(path, ref hashCode)) {
				throw new InvalidOperationException("Hash code of the specified file '" + path + "' could not be obtained.");
			}
			if (!hashCode.Equals(hash, StringComparison.Ordinal)) {
				throw new ArgumentException("The hash code of the specified file '" + path + "' does not correspond to the provided hash code '" + hash + "'.");
			}
		}

		/// <summary>
		///   Check whether the string parameter is a valid filename.
		/// </summary>
		/// <param name="filename">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckFilename(string filename, string paramName) {
			string message = null;
			if (!Checks.CheckFilenameSafe(filename, paramName, ref message)) {
				throw new ArgumentException(message);
			}
		}

		/// <summary>
		///   Check whether the string parameter is a valid filename.
		/// </summary>
		/// <param name="filename">parameter</param>
		/// <returns>success</returns>
		public static bool CheckFilenameSafe(string filename) {
			string message = null;
			return Checks.CheckFilenameSafe(filename, "filename", ref message);
		}

		/// <summary>
		///   Check whether the string parameter is a valid filename.
		/// </summary>
		/// <param name="filename">parameter</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="message">output message</param>
		/// <returns>success</returns>
		public static bool CheckFilenameSafe(string filename, string paramName, ref string message) {
			try {
				Checks.CheckString(filename, paramName);
			}
			catch (ArgumentException ex) {
				message = ex.Message;
				return false;
			}
			for (int i = 0; i < filename.Length; i++) {
				if (Checks.invalidFilenameChars.Contains(filename[i])) {
					message = $"Parameter '{paramName}' contains invalid filename character '{filename[i]}'.";
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Check whether the string parameter is a correct filesystem path.
		/// </summary>
		/// <param name="path">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckPath(string path, string paramName) {
			Checks.CheckString(path, paramName);
			if (!Checks.CheckPathSafe(path)) {
				throw new ArgumentException("Parameter '" + paramName + "' is not a valid path.");
			}
		}

		/// <summary>
		///   Check whether the file path specified by string parameter does exist.
		/// </summary>
		/// <param name="path">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckPathExists(string path, string paramName) {
			Checks.CheckPath(path, paramName);
			FileInfo fileInfo = new FileInfo(path);
			if (!(((fileInfo.Attributes & FileAttributes.Directory) != FileAttributes.Directory) ? File.Exists(path) : Directory.Exists(path))) {
				throw new FileNotFoundException("Path specified in '" + paramName + "' does not exist.");
			}
		}

		/// <summary>
		///   Check whether the file path specified by string parameter does exist.
		/// </summary>
		/// <param name="path">parameter</param>
		/// <param name="paramName">parameter name</param>
		/// <returns>the file path specified by string parameter does exist</returns>
		public static bool CheckPathExistsSafe(string path, string paramName) {
			if (!Checks.CheckPathSafe(path)) {
				return false;
			}
			FileInfo fileInfo = new FileInfo(path);
			if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
				return Directory.Exists(path);
			}
			return File.Exists(path);
		}

		/// <summary>
		///   Check whether the string parameter is a correct filesystem path without throwing an exception.
		/// </summary>
		/// <param name="path">path to check</param>
		/// <returns>path is valid</returns>
		public static bool CheckPathSafe(string path) {
			string message = string.Empty;
			if (Checks.CheckStringSafe(path, "^(?<1>.*[\\\\/])(?<2>.+)\\.(?<3>.+)?$|^(?<1>.*[\\\\/])(?<2>.+)$|^(?<2>.+)\\.(?<3>.+)?$|^(?<2>.+)$", "path", ref message)) {
				return true;
			}
			return false;
		}

		/// <summary>
		///   Check whether there is enough available memory.
		/// </summary>
		/// <param name="neededBytes">amount of needed memory (in bytes)</param>
		public static void CheckEnoughMemory(long neededBytes) {
			string message = null;
			if (!Checks.CheckEnoughMemorySafe(neededBytes, ref message)) {
				throw new ApplicationException(message);
			}
		}

		/// <summary>
		///   Safely check whether there is enough available memory.
		/// </summary>
		/// <param name="neededBytes">amount of needed memory (in bytes)</param>
		/// <returns>result</returns>
		public static bool CheckEnoughMemorySafe(long neededBytes) {
			string message = null;
			return Checks.CheckEnoughMemorySafe(neededBytes, ref message);
		}

		/// <summary>
		///   Safely check whether there is enough available memory.
		/// </summary>
		/// <param name="neededBytes">amount of needed memory (in bytes)</param>
		/// <param name="message">result message</param>
		/// <returns>result</returns>
		public static bool CheckEnoughMemorySafe(long neededBytes, ref string message) {
			Checks.CheckTrue(neededBytes >= 0, "neededBytes >= 0");
			if (neededBytes + 16777216 > IOUtils.GetAvailableMemory()) {
				message = "Insufficient available memory.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		/// <summary>
		///   Checks whether the specified string parameter is a Base64 string.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckBase64String(string param, string paramName) {
			if (!Checks.CheckBase64StringSafe(param)) {
				throw new ArgumentException("Parameter '" + paramName + "' is not a valid Base64 string.");
			}
		}

		/// <summary>
		///   Checks whether the specified string parameter is a Base64 string without throwing an exception.
		/// </summary>
		/// <param name="param">parameter</param>
		public static bool CheckBase64StringSafe(string param) {
			if (param == null) {
				return false;
			}
			param = param.Replace("\r", string.Empty).Replace("\n", string.Empty);
			if (param.Length == 0 || param.Length % 4 != 0) {
				return false;
			}
			int length = param.Length;
			param = param.TrimEnd('=');
			int length2 = param.Length;
			if (length - length2 > 2) {
				return false;
			}
			string text = param;
			string text2 = text;
			foreach (char item in text2) {
				if (!Checks.Base64Characters.Contains(item)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Check whether the string parameter is a valid e-mail address.
		/// </summary>
		/// <param name="email">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckEMail(string email, string paramName) {
			Checks.CheckString(email, paramName);
			if (!Checks.CheckEMailSafe(email)) {
				throw new ArgumentException("Parameter '" + paramName + "' is not a valid e-mail address.");
			}
		}

		/// <summary>
		///   Check whether the string parameter is a valid e-mail address without throwing an exception.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public static bool CheckEMailSafe(string email) {
			string message = string.Empty;
			if (Checks.CheckStringSafe(email, "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", "email", ref message)) {
				return true;
			}
			return false;
		}

		/// <summary>
		///   Check whether the specified string represents a hash code of the specified hash type.
		/// </summary>
		/// <param name="param">string parameter to check</param>
		/// <param name="hashType">hash algorithm</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckHashString(string param, HashType hashType, string paramName) {
			string message = null;
			if (!Checks.CheckHexStringSafe(param, StringUtils.GetHashSize(hashType), "paramName", ref message)) {
				throw new ArgumentException(message);
			}
		}

		/// <summary>
		///   Check whether the specified string represents a hash code of the specified hash type.
		/// </summary>
		/// <param name="param">string parameter to check</param>
		/// <param name="hashType">hash algorithm</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="message">output message</param>
		public static bool CheckHashStringSafe(string param, HashType hashType, string paramName, ref string message) {
			return Checks.CheckHexStringSafe(param, StringUtils.GetHashSize(hashType), "paramName", ref message);
		}

		/// <summary>
		///   Check whether the given string is a hexadecimal representation of byte array.
		/// </summary>
		/// <param name="param">string parameter to check</param>
		/// <param name="length">byte array length</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckHexString(string param, int length, string paramName) {
			string message = null;
			if (!Checks.CheckHexStringSafe(param, length, paramName, ref message)) {
				throw new ArgumentException(message);
			}
		}

		/// <summary>
		///   Check whether the given string is a hexadecimal representation of byte array without throwing an exception.
		/// </summary>
		/// <param name="param">string parameter to check</param>
		/// <param name="length">byte array length</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="message">output message</param>
		public static bool CheckHexStringSafe(string param, int length, string paramName, ref string message) {
			try {
				Checks.CheckNotNull(param, "param");
				Checks.CheckTrue(length >= 0, "length >= 0");
			}
			catch (ArgumentException ex) {
				message = ex.Message;
				return false;
			}
			int num = length << 1;
			if (param.Length != num) {
				message = $"Length of the hex string should be '{num}' but was '{param.Length}'.";
				return false;
			}
			for (int i = 0; i < param.Length; i++) {
				char c = param[i];
				if ((c < '0' || c > '9') && (c < 'a' || c > 'f')) {
					message = $"Parameter '{paramName}' (value: '{param}') contains wrong character '{c}' at position '{i}'.";
					return false;
				}
			}
			message = string.Empty;
			return true;
		}

		/// <summary>
		///   Check whether the string parameter matches given regular expression.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="pattern">regular expression pattern</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckString(string param, string pattern, string paramName) {
			string message = string.Empty;
			if (!Checks.CheckStringSafe(param, pattern, paramName, ref message)) {
				throw new ArgumentException(message);
			}
		}

		/// <summary>
		///   Check whether the string parameter is not null and is not empty.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckString(string param, string paramName) {
			Checks.CheckNotNull(param, paramName);
			if (param.Length == 0) {
				throw new ArgumentException("String '" + paramName + "' should not be empty.");
			}
		}

		/// <summary>
		///   Check whether the string parameter is not null and with length in the specified interval00.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="minimumLength">minimum allowed string parameter length</param>
		/// <param name="maximumLength">maximum allowed string parameter length</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckString(string param, int minimumLength, int maximumLength, string paramName) {
			Checks.CheckNotNull(param, paramName);
			Checks.CheckBounds(minimumLength, 0, maximumLength, "minimumLength");
			Checks.CheckTrue(maximumLength >= minimumLength, "maximumLength >= minimumLength");
			if (param.Length < minimumLength || param.Length > maximumLength) {
				throw new ArgumentException($"String '{paramName}' have length '{param.Length}', but should have length between '{minimumLength}' and '{maximumLength}'.");
			}
		}

		/// <summary>
		///   Check whether the string parameter matches given regular expression.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="pattern">regular expression pattern</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="message">error message</param>
		/// <returns>success</returns>
		public static bool CheckStringSafe(string param, string pattern, string paramName, ref string message) {
			try {
				Checks.CheckNotNull(param, paramName);
				Checks.CheckString(pattern, "pattern");
			}
			catch (ArgumentException ex) {
				message = ex.Message;
				return false;
			}
			try {
				Regex.Match(string.Empty, pattern);
			}
			catch (ArgumentException) {
				message = "The specified regular expression '" + pattern + "' is not valid.";
				return false;
			}
			Match match = Regex.Match(param, pattern);
			if (!match.Success) {
				message = "Given string '" + param + "' (paramter '" + paramName + "') does not match specified regular expression: '" + pattern + "'";
				return false;
			}
			message = string.Empty;
			return true;
		}

		/// <summary>
		///   Check whether the string parameter is a valid URL.
		/// </summary>
		/// <param name="url">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckUrl(string url, string paramName) {
			Checks.CheckString(url, paramName);
			if (!Checks.CheckUrlSafe(url)) {
				throw new ArgumentException("Parameter '" + paramName + "' is not a valid URL.");
			}
		}

		/// <summary>
		///   Check whether the string parameter is a valid URL without throwing an exception.
		/// </summary>
		/// <param name="url">URL to check</param>
		/// <returns>URL is valid</returns>
		public static bool CheckUrlSafe(string url) {
			string message = string.Empty;
			if (Checks.CheckStringSafe(url, "(([a-zA-Z][0-9a-zA-Z+\\-\\.]*:)?/{0,2}[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?(#[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?", "path", ref message)) {
				return true;
			}
			return false;
		}

		/// <summary>
		///   Check whether the specified parameter is not null.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckNotNull(object param, string paramName) {
			Checks.CheckNotNull(param, paramName, null);
		}

		/// <summary>
		///   Check whether the specified parameter is not null.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="message">exception message</param>
		public static void CheckNotNull(object param, string paramName, string message) {
			if (param == null) {
				string paramName2 = (string.IsNullOrEmpty(message) ? paramName : (message + Environment.NewLine + "Parameter name: " + paramName));
				throw new ArgumentNullException(paramName2);
			}
		}

		/// <summary>
		///   Check type. Also check for null argument.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="type">parameter type</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckType(object param, Type type, string paramName) {
			Checks.CheckType(param, type, paramName, allowNull: false);
		}

		/// <summary>
		///   Check type. Also check for null argument.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="type">parameter type</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="allowNull">parameter can be null</param>
		public static void CheckType(object param, Type type, string paramName, bool allowNull) {
			Checks.CheckType(param, new Type[1] { type }, paramName, allowNull);
		}

		/// <summary>
		///   Check type. Also check for null argument.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="allowedTypes">allowed parameter types</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckType(object param, ICollection<Type> allowedTypes, string paramName) {
			Checks.CheckType(param, allowedTypes, paramName, alowNull: false);
		}

		/// <summary>
		///   Check type. Also check for null argument.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="allowedTypes">allowed parameter types</param>
		/// <param name="paramName">parameter name</param>
		/// <param name="alowNull">parameter can be null</param>
		public static void CheckType(object param, ICollection<Type> allowedTypes, string paramName, bool alowNull) {
			if (alowNull && param == null) {
				return;
			}
			Checks.CheckNotNull(param, paramName);
			Checks.CheckNotNull(allowedTypes, "allowedTypes");
			Checks.CheckTrue(allowedTypes.Count != 0, "allowedTypes.Count != 0");
			foreach (Type allowedType in allowedTypes) {
				if (allowedType.IsInstanceOfType(param)) {
					return;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Type allowedType2 in allowedTypes) {
				stringBuilder.AppendFormat("'{0}', ", allowedType2.FullName);
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			throw new ArgumentException($"Parameter '{paramName}' was {param.GetType().FullName}, but should be one of the allowed types: {stringBuilder}.");
		}

		/// <summary>
		///   Check whether the parameter value lays between the given boundaries.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="minValue">minimum allowed parameter value</param>
		/// <param name="maxValue">maximum allowed parameter value</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckBounds<T>(T param, T minValue, T maxValue, string paramName) where T : IComparable {
			Checks.CheckNotNull(param, paramName);
			Checks.CheckBounds(param, minValue, maxValue, exclusive: false, paramName);
		}

		/// <summary>
		///   Check whether the parameter value lays between the given boundaries.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="minValue">minimum allowed parameter value</param>
		/// <param name="maxValue">maximum allowed parameter value</param>
		/// <param name="exclusive">parameter boundaries are exclusive</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckBounds<T>(T param, T minValue, T maxValue, bool exclusive, string paramName) where T : IComparable {
			Checks.CheckNotNull(param, paramName);
			bool num;
			if (exclusive) {
				if (param.CompareTo(minValue) > 0) {
					num = param.CompareTo(maxValue) >= 0;
					goto IL_0073;
				}
			}
			else if (param.CompareTo(minValue) >= 0) {
				num = param.CompareTo(maxValue) > 0;
				goto IL_0073;
			}
			goto IL_0075;
			IL_0075:
			throw new ArgumentOutOfRangeException($"Parameter '{paramName}' was {param}, but should lay between {minValue} and {maxValue}.");
			IL_0073:
			if (!num) {
				return;
			}
			goto IL_0075;
		}

		/// <summary>
		///   Check whether the parameter value is reasonable and lays between the given boundaries.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="minValue">minimum allowed parameter value</param>
		/// <param name="maxValue">maximum allowed parameter value</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckDouble(double param, double minValue, double maxValue, string paramName) {
			Checks.CheckDouble(param, minValue, maxValue, exclusive: false, paramName);
		}

		/// <summary>
		///   Check whether the parameter value is reasonable and lays between the given boundaries.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="minValue">minimum allowed parameter value</param>
		/// <param name="maxValue">maximum allowed parameter value</param>
		/// <param name="exclusive">parameter boundaries are exclusive</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckDouble(double param, double minValue, double maxValue, bool exclusive, string paramName) {
			Checks.CheckDouble(param, paramName);
			Checks.CheckBounds(param, minValue, maxValue, exclusive, paramName);
		}

		/// <summary>
		///   Check whether the number has a reasonable value.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckDouble(double param, string paramName) {
			if (double.IsNaN(param)) {
				throw new ArgumentException("Value of parameter '" + paramName + "' should be a number.");
			}
			if (double.IsInfinity(param)) {
				throw new ArgumentException("Value of parameter '" + paramName + "' should be a finite number.");
			}
		}

		/// <summary>
		///   Check whether the parameter value is reasonable and lays between the given boundaries.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="minValue">minimum allowed parameter value</param>
		/// <param name="maxValue">maximum allowed parameter value</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckSingle(float param, float minValue, float maxValue, string paramName) {
			Checks.CheckSingle(param, minValue, maxValue, exclusive: false, paramName);
		}

		/// <summary>
		///   Check whether the parameter value is reasonable and lays between the given boundaries.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="minValue">minimum allowed parameter value</param>
		/// <param name="maxValue">maximum allowed parameter value</param>
		/// <param name="exclusive">parameter boundaries are exclusive</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckSingle(float param, float minValue, float maxValue, bool exclusive, string paramName) {
			Checks.CheckSingle(param, paramName);
			Checks.CheckBounds(param, minValue, maxValue, exclusive: false, paramName);
		}

		/// <summary>
		///   Check whether the number has a reasonable value.
		/// </summary>
		/// <param name="param">parameter</param>
		/// <param name="paramName">parameter name</param>
		public static void CheckSingle(float param, string paramName) {
			if (float.IsNaN(param)) {
				throw new ArgumentException("Value of parameter '" + paramName + "' should be a number.");
			}
			if (float.IsInfinity(param)) {
				throw new ArgumentException("Value of parameter '" + paramName + "' should be a finite number.");
			}
		}
	}
}
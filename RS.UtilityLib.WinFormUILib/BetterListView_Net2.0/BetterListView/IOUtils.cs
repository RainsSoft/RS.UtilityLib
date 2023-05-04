using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common filesystem utilities.
	/// </summary>
	internal static class IOUtils
	{
		internal static class NativeMethods
		{
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
			public class MEMORYSTATUSEX
			{
				public uint dwLength;

				public uint dwMemoryLoad;

				public ulong ullTotalPhys;

				public ulong ullAvailPhys;

				public ulong ullTotalPageFile;

				public ulong ullAvailPageFile;

				public ulong ullTotalVirtual;

				public ulong ullAvailVirtual;

				public ulong ullAvailExtendedVirtual;

				public MEMORYSTATUSEX() {
					this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
				}
			}

			[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GlobalMemoryStatusEx([In][Out] MEMORYSTATUSEX lpBuffer);
		}

		private const int BufferSize = 32768;

		private const string DefaultBoundary = "----";

		private const char PairSeparator = '\n';

		private const string ValueSeparator = "=";

		private const int DefaultTimeoutWebRequest = 10000;

		private static NativeMethods.MEMORYSTATUSEX memStatus;

		/// <summary>
		///   Check whether the two paths are the same.
		/// </summary>
		/// <param name="pathA">first path comparison</param>
		/// <param name="pathB">second path comparison</param>
		/// <returns>the two paths are the same</returns>
		public static bool AreSamePaths(string pathA, string pathB) {
			return IOUtils.AreSamePaths(pathA, pathB, getFullPaths: false);
		}

		/// <summary>
		///   Check whether the two paths are the same.
		/// </summary>
		/// <param name="pathA">first path comparison</param>
		/// <param name="pathB">second path comparison</param>
		/// <param name="getFullPaths">get full paths before normalization</param>
		/// <returns>the two paths are the same</returns>
		public static bool AreSamePaths(string pathA, string pathB, bool getFullPaths) {
			Checks.CheckPath(pathA, "pathA");
			Checks.CheckPath(pathB, "pathB");
			return IOUtils.NormalizePath(pathA, getFullPaths, caseSensitive: false).Equals(IOUtils.NormalizePath(pathB, getFullPaths, caseSensitive: false), StringComparison.Ordinal);
		}

		/// <summary>
		///   Check whether the source path can be copied/moved to target path.
		/// </summary>
		/// <param name="sourcePath">source path</param>
		/// <param name="targetPath">target path</param>
		/// <returns>the source path can be copied/moved in target path</returns>
		/// <remarks>
		///   Both paths should be normalized.
		/// </remarks>
		public static bool CheckCopyMove(string sourcePath, string targetPath) {
			Checks.CheckPath(sourcePath, "sourcePath");
			Checks.CheckPath(targetPath, "targetPath");
			if (sourcePath.Equals(targetPath, StringComparison.Ordinal)) {
				return false;
			}
			bool flag = IOUtils.CheckDirectory(sourcePath);
			bool flag2 = IOUtils.CheckDirectory(targetPath);
			if (flag && targetPath.IndexOf(sourcePath) == 0) {
				return false;
			}
			string text = Path.GetDirectoryName(sourcePath);
			if (text == null) {
				text = sourcePath;
			}
			if (flag2 && text.Equals(targetPath, StringComparison.Ordinal)) {
				return false;
			}
			return !sourcePath.Equals(targetPath, StringComparison.Ordinal);
		}

		/// <summary>
		///   Check whether the specified existing file is a directory.
		/// </summary>
		/// <param name="path">path of file to check</param>
		/// <returns>the specified file is a directory</returns>
		public static bool CheckDirectory(string path) {
			Checks.CheckPath(path, "path");
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists) {
				return (fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
			}
			DriveInfo driveInfo = new DriveInfo(path);
			if (driveInfo.DriveType == DriveType.NoRootDirectory) {
				return false;
			}
			return true;
		}

		/// <summary>
		///   Get MD5 hash code for a given path.
		/// </summary>
		/// <param name="path">path to compute hash from</param>
		/// <param name="getFullPath">get full path before normalization</param>
		/// <returns>hash code</returns>
		public static string ComputePathHash(string path, bool getFullPath) {
			Checks.CheckPath(path, "path");
			return StringUtils.ComputeStringHash(IOUtils.NormalizePath(path, getFullPath, caseSensitive: false), HashType.Md5);
		}

		/// <summary>
		///   Compute MD5 hash code from the file content.
		/// </summary>
		/// <param name="path">path of the file to compute hash from</param>
		/// <param name="hashCode">MD5 hash code of the file content</param>
		/// <returns>success</returns>
		public static bool ComputeFileContentHash(string path, ref string hashCode) {
			Checks.CheckFileExists(path, "path");
			HashAlgorithm hashAlgorithm = StringUtils.GetHashAlgorithm(HashType.Md5);
			byte[] array;
			try {
				using (FileStream inputStream = File.OpenRead(path)) {
					array = hashAlgorithm.ComputeHash(inputStream);
					hashAlgorithm.Clear();
				}
			}
			catch (UnauthorizedAccessException) {
				return false;
			}
			catch (NotSupportedException) {
				return false;
			}
			catch (CryptographicException) {
				return false;
			}
			if (array == null || array.Length == 0) {
				return false;
			}
			hashCode = StringUtils.GetHexString(array);
			return true;
		}

		/// <summary>
		///   Copy Stream to another Stream.
		/// </summary>
		/// <param name="sourceStream">source stream</param>
		/// <param name="targetStream">target stream</param>
		public static void CopyStream(Stream sourceStream, Stream targetStream) {
			Checks.CheckNotNull(sourceStream, "sourceStream");
			Checks.CheckNotNull(targetStream, "targetStream");
			Checks.CheckTrue(sourceStream.CanRead, "sourceStream.CanRead");
			Checks.CheckTrue(targetStream.CanWrite, "targetStream.CanWrite");
			byte[] array = new byte[32768];
			while (true) {
				int num = sourceStream.Read(array, 0, array.Length);
				if (num > 0) {
					targetStream.Write(array, 0, num);
					continue;
				}
				break;
			}
		}

		/// <summary>
		///   Get amount of available physical memory.
		/// </summary>
		/// <returns>amount of available physical memory (in bytes)</returns>
		public static long GetAvailableMemory() {
			if (IOUtils.memStatus == null) {
				IOUtils.memStatus = new NativeMethods.MEMORYSTATUSEX();
			}
			if (NativeMethods.GlobalMemoryStatusEx(IOUtils.memStatus)) {
				return (long)IOUtils.memStatus.ullAvailPhys;
			}
			return 0L;
		}

		/// <summary>
		///   Get string representation of raw pixel data size (in bytes).
		/// </summary>
		public static string GetSizeString(long size) {
			Checks.CheckTrue(size >= 0, "size >= 0");
			long num = 1L;
			string text = "";
			if (size < 1024) {
				num = 1L;
				text = "B";
			}
			else if (size < 1048576) {
				num = 1024L;
				text = "KB";
			}
			else if (size < 1073741824) {
				num = 1048676L;
				text = "MB";
			}
			else {
				num = 1073741824L;
				text = "GB";
			}
			if (size % num == 0L) {
				return $"{Math.Round((double)size / (double)num, 0):N2} {text}";
			}
			return $"{Math.Round((double)size / (double)num, 2):N2} {text}";
		}

		/// <summary>
		///   Get substitute path for the specified path, that is unique.
		/// </summary>
		/// <param name="path">file path (either existing or unique)</param>
		/// <returns>unique substitute path</returns>
		public static string GetSubstitutePath(string path) {
			Checks.CheckPath(path, "path");
			if (!File.Exists(path)) {
				return path;
			}
			string directoryName = Path.GetDirectoryName(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			string extension = Path.GetExtension(path);
			int num = 0;
			string text = null;
			do {
				num++;
				text = $"{directoryName}{Path.DirectorySeparatorChar}{fileNameWithoutExtension}({num}){extension}";
			}
			while (File.Exists(text));
			return text;
		}

		/// <summary>
		///   Get unique file name in a specified directory.
		/// </summary>
		/// <param name="dirInfo">directory to find unique file in</param>
		/// <returns>unique file name</returns>
		public static string GetUniquePath(DirectoryInfo dirInfo) {
			Checks.CheckNotNull(dirInfo, "dirInfo");
			Checks.CheckTrue(dirInfo.Exists, "dirInfo.Exists");
			string text = null;
			do {
				text = dirInfo.FullName + Path.DirectorySeparatorChar + Path.GetRandomFileName() + ".tmp";
			}
			while (File.Exists(text));
			return text;
		}

		public static WebRequestResult GetWebResponse(string hostName, string subPath, Dictionary<string, string> request, NetworkCredential credentials, ref Dictionary<string, string> response, ref string message) {
			return IOUtils.GetWebResponse(hostName, subPath, request, credentials, xmlOutput: false, useMultipartPost: true, ref response, ref message);
		}

		public static WebRequestResult GetWebResponse(string hostName, string subPath, Dictionary<string, string> request, NetworkCredential credentials, bool xmlOutput, bool useMultipartPost, ref Dictionary<string, string> response, ref string message) {
			Checks.CheckString(hostName, "hostName");
			Checks.CheckString(subPath, "subPath");
			string url = "http://" + hostName + "/" + subPath;
			Checks.CheckUrl(url, "url");
			Checks.CheckCollection(request, "request");
			foreach (string key in request.Keys) {
				Checks.CheckFalse(key.Contains("\""), "parameterName.Contains(\"\\\"\")");
			}
			if (!NetworkInterface.GetIsNetworkAvailable()) {
				message = "No internet connection.";
				return WebRequestResult.NoInternetConnection;
			}
			response = new Dictionary<string, string>();
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			if (useMultipartPost) {
				Set<string> set = new Set<string>();
				foreach (KeyValuePair<string, string> item in request) {
					set.Add(item.Key);
					set.Add(item.Value);
				}
				Random random = new Random();
				do {
					text = "----" + (char)random.Next(48, 57) + (char)random.Next(97, 122) + (char)random.Next(48, 57) + (char)random.Next(97, 122);
				}
				while (set.Contains(text));
				foreach (KeyValuePair<string, string> item2 in request) {
					stringBuilder.AppendLine("--" + text);
					stringBuilder.AppendLine("Content-Disposition: form-data; name=\"" + item2.Key + "\"");
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(item2.Value);
				}
				stringBuilder.AppendLine("--" + text);
			}
			else {
				bool flag = true;
				foreach (KeyValuePair<string, string> item3 in request) {
					if (flag) {
						flag = false;
					}
					else {
						stringBuilder.Append('&');
					}
					stringBuilder.AppendFormat("{0}={1}", Uri.EscapeDataString(item3.Key), Uri.EscapeDataString(item3.Value));
				}
			}
			byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			Stream stream = null;
			StreamReader streamReader = null;
			try {
				WebRequest webRequest = IOUtils.CreateWebRequest(url, bytes.Length, useMultipartPost, text, credentials, xmlOutput, useProxy: false);
				stream = webRequest.GetRequestStream();
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
				Stream responseStream;
				try {
					WebResponse response3 = webRequest.GetResponse();
					responseStream = response3.GetResponseStream();
				}
				catch (WebException) {
					webRequest = IOUtils.CreateWebRequest(url, bytes.Length, useMultipartPost, text, credentials, xmlOutput, useProxy: true);
					stream = webRequest.GetRequestStream();
					stream.Write(bytes, 0, bytes.Length);
					stream.Close();
					WebResponse response2 = webRequest.GetResponse();
					responseStream = response2.GetResponseStream();
				}
				streamReader = new StreamReader(responseStream);
				string text2 = streamReader.ReadToEnd();
				if (string.IsNullOrEmpty(text2)) {
					message = "Empty response.";
					return WebRequestResult.Other;
				}
				if (!xmlOutput) {
					string[] array = text2.Split('\n');
					string[] separator = new string[1] { "=" };
					string[] array3 = array;
					foreach (string text3 in array3) {
						if (!text3.Contains("=")) {
							message = "Invalid response data (separator not present: '" + text3 + "').";
							return WebRequestResult.Other;
						}
						string[] array2 = text3.Split(separator, StringSplitOptions.None);
						if (array2.Length != 2) {
							message = "Invalid response data (badly separated parameter: '" + text3 + "').";
							return WebRequestResult.Other;
						}
						response.Add(array2[0], array2[1]);
					}
				}
				else {
					try {
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(text2);
						XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("activation");
						if (elementsByTagName.Count == 1) {
							XmlNode xmlNode = elementsByTagName.Item(0);
							foreach (XmlNode childNode in xmlNode.ChildNodes) {
								response.Add(childNode.Name, childNode.InnerText);
							}
						}
					}
					catch (XmlException ex2) {
						message = "Failed to parse XML response from server (Message: " + ex2.Message + ")";
						return WebRequestResult.Other;
					}
				}
			}
			catch (ArgumentException ex3) {
				message = ex3.Message;
				return WebRequestResult.Other;
			}
			catch (IOException ex4) {
				message = ex4.Message;
				return WebRequestResult.Other;
			}
			catch (NotSupportedException ex5) {
				message = ex5.Message;
				return WebRequestResult.Other;
			}
			catch (ObjectDisposedException ex6) {
				message = ex6.Message;
				return WebRequestResult.Other;
			}
			catch (SecurityException ex7) {
				message = ex7.Message;
				return WebRequestResult.Other;
			}
			catch (UriFormatException ex8) {
				message = ex8.Message;
				return WebRequestResult.Other;
			}
			catch (WebException ex9) {
				message = ex9.Message;
				return WebRequestResult.Other;
			}
			finally {
				stream?.Dispose();
				if (streamReader != null) {
					streamReader.Close();
					streamReader.Dispose();
				}
			}
			message = string.Empty;
			return WebRequestResult.Ok;
		}

		/// <summary>
		///   Normalize file path (so it allows direct string comparison).
		/// </summary>
		/// <param name="path">path to be normalized</param>
		/// <param name="getFullPath">get full path before normalization</param>
		/// <param name="caseSensitive">keep case sensitivity</param>
		/// <returns>normalized path</returns>
		public static string NormalizePath(string path, bool getFullPath, bool caseSensitive) {
			Checks.CheckPath(path, "path");
			path = path.Replace('/', Path.DirectorySeparatorChar);
			if (getFullPath) {
				path = Path.GetFullPath(path);
			}
			path = path.Trim();
			if (path[path.Length - 1] == Path.DirectorySeparatorChar) {
				path = path.Remove(path.Length - 1, 1);
			}
			string text = new string(Path.DirectorySeparatorChar, 2);
			string newValue = new string(Path.DirectorySeparatorChar, 1);
			while (path.Contains(text)) {
				path = path.Replace(text, newValue);
			}
			if (!caseSensitive) {
				path = path.ToUpperInvariant();
			}
			return path;
		}

		/// <summary>
		///   Allow access to read-only files by removing the ReadOnly attribute.
		/// </summary>
		/// <param name="paths">paths of files to unlock</param>
		/// <returns>success</returns>
		public static bool UnlockReadOnlyFiles(ReadOnlyCollection<string> paths) {
			Checks.CheckCollection(paths, "paths");
			foreach (string path in paths) {
				Checks.CheckFileExists(path, "path");
			}
			try {
				foreach (string path2 in paths) {
					FileAttributes attributes = File.GetAttributes(path2);
					if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
						File.SetAttributes(path2, attributes & ~FileAttributes.ReadOnly);
					}
				}
			}
			catch (UnauthorizedAccessException) {
				return false;
			}
			return true;
		}

		private static WebRequest CreateWebRequest(string url, long contentLength, bool useMultipartPost, string boundary, NetworkCredential credentials, bool xmlOutput, bool useProxy) {
			Uri requestUri = new Uri(url, UriKind.Absolute);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.ContentLength = contentLength;
			if (useMultipartPost) {
				httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
			}
			if (credentials != null) {
				httpWebRequest.Credentials = credentials;
			}
			httpWebRequest.KeepAlive = false;
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = 10000;
			if (xmlOutput) {
				httpWebRequest.Accept = "application/xml";
			}
			if (!useProxy) {
				httpWebRequest.Proxy = null;
			}
			return httpWebRequest;
		}
	}
}
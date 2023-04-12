//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2018 Solutions Design. All rights reserved.
// https://github.com/SolutionsDesign/Algorithmia
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2018 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//		- Frans  Bouma [FB]
//////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;

using SD.System_Linq;

using System.Text;
using System.ComponentModel;
using SD.Tools.BCLExtensions.CollectionsRelated;

namespace SD.Tools.Algorithmia.GeneralDataStructures
{
	/// <summary>
	/// 简单类，用作 IDataErrorInfo 实现的错误信息的容器。
	/// Simple class which is used as a container for error information for IDataErrorInfo implementations. 
	/// </summary>
	/// <remarks>与其在实现 IDataErrorInformation 的每个类中对错误信息进行内务处理，不如使用此类的实例来为您执行此操作。
	/// 只需在此类中设置错误，并在您自己的类的 IDataErrorInfo 实现中检索错误信息。<br/>Instead of doing housekeeping of error info in every class which implements IDataErrorInfo, you can use an instance of
	/// this class to do it for you. Simply set the errors in this class and retrieve the error info in the IDataErrorInfo implementation of your own class.
	/// <br/><br/>
	/// This class is thread safe.
	/// </remarks>
	public class ErrorContainer : IDataErrorInfo
	{
		#region Class Member Declarations
		private string _dataErrorString;
		private Dictionary<string, Pair<string, bool>> _errorPerProperty;		// value: Pair.Value1 = errorcode, Pair.Value2 = flag if error is soft (true) or not (false). Soft errors are removed after they're read.
		private readonly string _defaultError;
		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorContainer"/> class.
		/// </summary>
		/// <param name="defaultError">The default error message to return by IDataErrorInfo.Error.</param>
		public ErrorContainer(string defaultError)
		{
			_dataErrorString = string.Empty;
			_defaultError = defaultError;
			this.SyncRoot = new Object();
		}


		/// <summary>
		/// 清除此容器中包含的错误
		/// Clears the errors contained in this container
		/// </summary>
		public void ClearErrors()
		{
			_dataErrorString = string.Empty;
			lock(this.SyncRoot)
			{
				this.ErrorPerProperty.Clear();
			}
		}


		/// <summary>
		/// 获取此错误容器中存储有错误的所有属性名称。使用名称索引到此容器中，以获取此特定属性的错误。将忽略将空字符串作为错误的属性。<br/>
		/// Gets all property names with errors stored in this errorcontainer. Use the names to index into this container to obtain the error for this particular
		/// property. Properties with an empty string as error are ignored.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This method traverses the inner structures with a filter without locking. To make sure a call to this method is thread safe, lock on 
		/// <see cref="SyncRoot"/> </remarks>
		public IEnumerable<string> GetAllPropertyNamesWithErrors()
		{
			foreach(KeyValuePair<string, Pair<string, bool>> pair in this.ErrorPerProperty)
			{
				if(!string.IsNullOrEmpty(pair.Value.Value1))
				{
					yield return pair.Key;
				}
			}
		}


		/// <summary>
		/// 将所有属性错误对转换为换行符分隔的字符串。
		/// Converts all property-error pairs to a new-line delimited string .
		/// </summary>
		/// <param name="prefixEachLineWithDash">if set to <see langword="true"/> each line is prefixed with a '-'</param>
		/// <returns></returns>
		public string ConvertToNewLineDelimitedList(bool prefixEachLineWithDash)
		{
			StringBuilder builder = new StringBuilder();
			bool first = true;
			List<string> namesToTraverse = null;
			lock(this.SyncRoot)
			{
				namesToTraverse = GetAllPropertyNamesWithErrors().ToList();
			}
			foreach(string propertyName in namesToTraverse)
			{
				string error = this[propertyName];
				if(!first)
				{
					builder.Append(Environment.NewLine);
				}
				if(prefixEachLineWithDash)
				{
					builder.Append(" - ");
				}
				builder.AppendFormat("{0}: {1}", propertyName, error);
				first = false;
			}
			return builder.ToString();
		}


		/// <summary>
		/// 设置属性错误。如果传入空错误描述，则清除错误信息。始终记录硬错误。
		/// Sets the property error. If an empty errorDescription is passed in, the error information is cleared. Always logs a hard error.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="errorDescription">The error description.</param>
		public void SetPropertyError(string propertyName, string errorDescription)
		{
			SetPropertyError(propertyName, errorDescription, false);
		}


		/// <summary>
		/// 设置属性错误。如果传入空错误描述，则清除错误信息。
		/// Sets the property error. If an empty errorDescription is passed in, the error information is cleared.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="errorDescription">The error description.</param>
		/// <param name="isSoftError">if set to <see langword="true"/> the error is considered 'soft', which means it's cleared after it's been read.</param>
		public void SetPropertyError(string propertyName, string errorDescription, bool isSoftError)
		{
			lock(this.SyncRoot)
			{
				if(errorDescription.Length <= 0)
				{
					this.ErrorPerProperty.Remove(propertyName);
				}
				else
				{
					this.ErrorPerProperty[propertyName] = new Pair<string, bool>(errorDescription, isSoftError);
					_dataErrorString = _defaultError;
				}
				if(this.ErrorPerProperty.Count <= 0)
				{
					_dataErrorString = string.Empty;
				}
			}
		}


		/// <summary>
		/// 将属性错误追加到该属性的现有错误。首先附加换行符。
		/// Appends the property error to an existing error for that property. Appends a newline first.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="errorDescription">The error description.</param>
		/// <remarks>若要删除属性的错误消息，请使用空字符串作为错误消息调用 SetPropertyError。始终记录硬错误<br/>
		/// To remove an error message for a property, call SetPropertyError with an empty string as error message.
		/// Always logs a hard error</remarks>
		public void AppendPropertyError(string propertyName, string errorDescription)
		{
			AppendPropertyError(propertyName, errorDescription, Environment.NewLine, false);
		}


		/// <summary>
		/// 将属性错误追加到该属性的现有错误。首先附加换行符。
		/// Appends the property error to an existing error for that property. Appends a newline first.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="errorDescription">The error description.</param>
		/// <param name="isSoftError">if set to <see langword="true"/> the error is considered 'soft', which means it's cleared after it's been read.</param>
		/// <remarks>若要删除属性的错误消息，请使用空字符串作为错误消息调用 SetPropertyError。
		/// 使用 isSoftError 中指定的值覆盖属性的现有错误的现有 isSoftError 标志值.
		/// To remove an error message for a property, call SetPropertyError with an empty string as error message.
		/// Overwrites the existing isSoftError flag value of the existing error of the property with the value specified in isSoftError</remarks>
		public void AppendPropertyError(string propertyName, string errorDescription, bool isSoftError)
		{
			AppendPropertyError(propertyName, errorDescription, Environment.NewLine, isSoftError);
		}


		/// <summary>
		/// 将属性错误追加到该属性的现有错误。首先将行分隔符追加到现有错误。
		/// Appends the property error to an existing error for that property. Appends the lineSeparator to the existing error first. 
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="errorDescription">The error description.</param>
		/// <param name="lineSeparator">要追加到现有错误消息的行分隔符。The line separator to append to the existing error message.</param>
		/// <remarks>若要删除属性的错误消息，请使用空字符串作为错误消息调用 SetPropertyError。始终记录硬错误.
		/// To remove an error message for a property, call SetPropertyError with an empty string as error message.
		/// Always logs a hard error</remarks>
		public void AppendPropertyError(string propertyName, string errorDescription, string lineSeparator)
		{
			AppendPropertyError(propertyName, errorDescription, lineSeparator, false);
		}


		/// <summary>
		/// 将属性错误追加到该属性的现有错误。首先将行分隔符追加到现有错误。
		/// Appends the property error to an existing error for that property. Appends the lineSeparator to the existing error first. 
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="errorDescription">The error description.</param>
		/// <param name="lineSeparator">要追加到现有错误消息的行分隔符。The line separator to append to the existing error message.</param>
		/// <param name="isSoftError">如果设置为 <see langword="true"/>则错误被视为'软'，这意味着它在读取后被清除。if set to <see langword="true"/> the error is considered 'soft', which means it's cleared after it's been read.</param>
		/// <remarks>若要删除属性的错误消息，请使用空字符串作为错误消息调用 SetPropertyError。
		/// 使用 isSoftError 中指定的值覆盖属性的现有错误的现有 isSoftError 标志值.
		/// To remove an error message for a property, call SetPropertyError with an empty string as error message.
		/// Overwrites the existing isSoftError flag value of the existing error of the property with the value specified in isSoftError</remarks>
		public void AppendPropertyError(string propertyName, string errorDescription, string lineSeparator, bool isSoftError)
		{
			if(string.IsNullOrEmpty(errorDescription))
			{
				return;
			}
			if(string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentException("propertyName is empty or null");
			}
			string existingError = this[propertyName];
			if(existingError.Length > 0)
			{
				existingError = existingError + lineSeparator;
			}
			SetPropertyError(propertyName, existingError + errorDescription, isSoftError);
		}


		#region IDataErrorInfo Members
		/// <summary>
		/// 获取一条错误消息，指示此对象出了什么问题。
		/// Gets an error message indicating what is wrong with this object.
		/// </summary>
		/// <returns>
		/// 指示此对象出现问题的错误消息。默认值为空字符串 （“”）。
		/// An error message indicating what is wrong with this object. The default is an empty string ("").
		/// </returns>
		public string Error
		{
			get { return _dataErrorString; }
		}

		/// <summary>
		/// 获取具有指定列名的字符串。
		/// Gets the <see cref="System.String"/> with the specified column name.
		/// </summary>
		public string this[string columnName]
		{
			get
			{
				lock(this.SyncRoot)
				{
					Pair<string, bool> loggedErrorInfo = this.ErrorPerProperty.GetValue(columnName);
					if(loggedErrorInfo == null)
					{
						return string.Empty;
					}
					if(loggedErrorInfo.Value2)
					{
						// soft error, so reset it
						SetPropertyError(columnName, string.Empty);
					}
					return loggedErrorInfo.Value1;
				}
			}
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="System.Collections.ICollection" />. It's the same object used in locks inside this object. 
		/// </summary>
		public object SyncRoot { get; }

		private Dictionary<string, Pair<string, bool>> ErrorPerProperty
		{
			get { return _errorPerProperty ?? (_errorPerProperty = new Dictionary<string, Pair<string, bool>>()); }
		}
		#endregion
	}
}

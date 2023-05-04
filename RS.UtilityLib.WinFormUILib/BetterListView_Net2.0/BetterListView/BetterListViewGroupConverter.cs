using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type converter for BetterListViewGroup.
	/// </summary>
	public sealed class BetterListViewGroupConverter : TypeConverter
	{
		/// <summary>
		///   Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <returns>
		///   true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		/// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from. 
		/// </param>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (context == null || context.Instance == null || !(context.Instance is BetterListViewItem)) {
				return false;
			}
			if (sourceType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		///   Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <returns>
		///   true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		/// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to. 
		/// </param>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (context == null || context.Instance == null) {
				return false;
			}
			if (!(context.Instance is BetterListViewItem betterListViewItem) || betterListViewItem.Group == null) {
				return false;
			}
			if (destinationType == typeof(string)) {
				return true;
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		///   Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Object" /> that represents the converted value.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture. 
		/// </param>
		/// <param name="value">The <see cref="T:System.Object" /> to convert. 
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. 
		/// </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string && context.Instance != null) {
				string value2 = ((string)value).Trim();
				if (context.Instance is BetterListViewItem betterListViewItem && betterListViewItem.ListView != null) {
					foreach (BetterListViewGroup group in betterListViewItem.ListView.Groups) {
						if (group.Header.Equals(value2, StringComparison.Ordinal)) {
							return group;
						}
					}
				}
				return null;
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		///   Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Object" /> that represents the converted value.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed. 
		/// </param>
		/// <param name="value">The <see cref="T:System.Object" /> to convert. 
		/// </param>
		/// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to. 
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType" /> parameter is null. 
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. 
		/// </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			BetterListViewGroup betterListViewGroup = (BetterListViewGroup)value;
			if (destinationType == typeof(string)) {
				if (betterListViewGroup == null) {
					return string.Empty;
				}
				return betterListViewGroup.Header.Trim();
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				return new InstanceDescriptor(typeof(BetterListViewGroup).GetConstructor(new Type[2]
				{
				typeof(string),
				typeof(TextAlignmentHorizontal)
				}), new object[2] { betterListViewGroup.Header, betterListViewGroup.HeaderAlignmentHorizontal }, isComplete: false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		///   Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" /> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null. 
		/// </param>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if (context.Instance == null) {
				return null;
			}
			if (!(context.Instance is BetterListViewItem betterListViewItem) || betterListViewItem.ListView == null) {
				return null;
			}
			ArrayList arrayList = new ArrayList();
			foreach (BetterListViewGroup group in betterListViewItem.ListView.Groups) {
				if (group.Header.Length != 0) {
					arrayList.Add(group);
				}
			}
			arrayList.Add(null);
			return new StandardValuesCollection(arrayList);
		}

		/// <summary>
		///   Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an exclusive list of possible values, using the specified context.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" /> returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an exhaustive list of possible values; false if other values are possible.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}

		/// <summary>
		///   Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
		/// </summary>
		/// <returns>
		///   true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> should be called to find a common set of values the object supports; otherwise, false.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
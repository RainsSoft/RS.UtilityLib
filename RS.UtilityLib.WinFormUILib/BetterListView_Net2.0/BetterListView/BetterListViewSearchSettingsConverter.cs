using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type converter for BetterListViewSearchSettings.
	/// </summary>
	internal sealed class BetterListViewSearchSettingsConverter : ExpandableObjectConverter
	{
		/// <summary>
		///   Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
		/// <returns>
		///   true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		///   Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
		/// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
		/// <returns>
		///   An <see cref="T:System.Object" /> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   The <paramref name="destinationType" /> parameter is null.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		///   The conversion cannot be performed.
		/// </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			int[] enumerable = new int[1];
			BetterListViewSearchSettings betterListViewSearchSettings = new BetterListViewSearchSettings(BetterListViewSearchMode.PrefixOrSubstring, BetterListViewSearchOptions.FirstWordOnly | BetterListViewSearchOptions.PrefixPreference | BetterListViewSearchOptions.WordSearch, enumerable);
			if (value is BetterListViewSearchSettings) {
				betterListViewSearchSettings = (BetterListViewSearchSettings)value;
			}
			if (destinationType == typeof(string)) {
				return betterListViewSearchSettings.Mode.ToString();
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				Type[] array = new Type[3];
				object[] array2 = new object[3];
				array[0] = typeof(BetterListViewSearchMode);
				array2[0] = betterListViewSearchSettings.Mode;
				array[1] = typeof(BetterListViewSearchOptions);
				array2[1] = betterListViewSearchSettings.Options;
				array[2] = typeof(int[]);
				array2[2] = betterListViewSearchSettings.SubItemIndices.ToArray();
				return new InstanceDescriptor(typeof(BetterListViewSearchSettings).GetConstructor(array), array2);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		///   Creates an instance of the type that this <see cref="T:System.ComponentModel.TypeConverter" /> is associated with, using the specified context, given a set of property values for the object.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary" /> of new property values.</param>
		/// <returns>
		///   An <see cref="T:System.Object" /> representing the given <see cref="T:System.Collections.IDictionary" />, or null if the object cannot be created. This method always returns null.
		/// </returns>
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			return new BetterListViewSearchSettings((BetterListViewSearchMode)propertyValues["Mode"], (BetterListViewSearchOptions)propertyValues["Options"], (IEnumerable<int>)propertyValues["SubItemIndices"]);
		}

		/// <summary>
		///   Returns whether changing a value on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> to create a new value, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <returns>
		///   true if changing a property on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> to create a new value; otherwise, false.
		/// </returns>
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
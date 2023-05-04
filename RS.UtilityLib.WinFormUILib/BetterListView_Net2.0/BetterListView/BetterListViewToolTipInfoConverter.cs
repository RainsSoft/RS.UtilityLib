using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type converter for BetterListViewToolTipInfo.
	/// </summary>
	public sealed class BetterListViewToolTipInfoConverter : ExpandableObjectConverter
	{
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
			BetterListViewToolTipInfo betterListViewToolTipInfo = (BetterListViewToolTipInfo)value;
			if (destinationType == typeof(string)) {
				return betterListViewToolTipInfo.Text;
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				Type[] array = new Type[11];
				object[] array2 = new object[11];
				array[0] = typeof(BetterListViewToolTipLocation);
				array2[0] = betterListViewToolTipInfo.Location;
				array[1] = typeof(Rectangle);
				array2[1] = betterListViewToolTipInfo.Bounds;
				array[2] = typeof(string);
				array2[2] = betterListViewToolTipInfo.Text;
				array[3] = typeof(bool);
				array2[3] = betterListViewToolTipInfo.ShowOnPartialTextVisibility;
				array[4] = typeof(Color);
				array2[4] = betterListViewToolTipInfo.ToolTipBackColor;
				array[5] = typeof(Color);
				array2[5] = betterListViewToolTipInfo.ToolTipForeColor;
				array[6] = typeof(bool);
				array2[6] = betterListViewToolTipInfo.ToolTipIsBalloon;
				array[7] = typeof(bool);
				array2[7] = betterListViewToolTipInfo.ToolTipOwnerDraw;
				array[8] = typeof(bool);
				array2[8] = betterListViewToolTipInfo.ToolTipStripAmpersands;
				array[9] = typeof(ToolTipIcon);
				array2[9] = betterListViewToolTipInfo.ToolTipIcon;
				array[10] = typeof(string);
				array2[10] = betterListViewToolTipInfo.ToolTipTitle;
				return new InstanceDescriptor(typeof(BetterListViewToolTipInfo).GetConstructor(array), array2);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		///   Creates an instance of the type that this <see cref="T:System.ComponentModel.TypeConverter" /> is associated with, using the specified context, given a set of property values for the object.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Object" /> representing the given <see cref="T:System.Collections.IDictionary" />, or null if the object cannot be created. This method always returns null.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		/// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary" /> of new property values. 
		/// </param>
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			if (context == null || !(context.Instance is BetterListView)) {
				return null;
			}
			return new BetterListViewToolTipInfo((BetterListViewToolTipLocation)propertyValues["Location"], (Rectangle)propertyValues["Bounds"], (string)(propertyValues["Text"] ?? string.Empty), (bool)propertyValues["ShowOnPartialTextVisibility"], (Color)propertyValues["ToolTipBackColor"], (Color)propertyValues["ToolTipForeColor"], (bool)propertyValues["ToolTipIsBalloon"], (bool)propertyValues["ToolTipOwnerDraw"], (bool)propertyValues["ToolTipStripAmpersands"], (ToolTipIcon)propertyValues["ToolTipIcon"], (string)(propertyValues["ToolTipTitle"] ?? string.Empty));
		}

		/// <summary>
		///   Returns whether changing a value on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> to create a new value, using the specified context.
		/// </summary>
		/// <returns>
		///   true if changing a property on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> to create a new value; otherwise, false.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
		/// </param>
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			if (context != null) {
				return context.Instance is BetterListView;
			}
			return false;
		}
	}
}
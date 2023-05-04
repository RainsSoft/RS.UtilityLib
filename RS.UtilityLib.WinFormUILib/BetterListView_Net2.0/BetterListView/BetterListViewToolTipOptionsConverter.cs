using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type converter for BetterListViewToolTipOptions.
	/// </summary>
	public sealed class BetterListViewToolTipOptionsConverter : ExpandableObjectConverter
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
			BetterListViewToolTipOptions betterListViewToolTipOptions = (BetterListViewToolTipOptions)value;
			if (destinationType == typeof(string)) {
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0} ms; ", betterListViewToolTipOptions.AutomaticDelay);
				if (betterListViewToolTipOptions.ShowAlways) {
					stringBuilder.Append("ShowAlways; ");
				}
				if (betterListViewToolTipOptions.UseAnimation) {
					stringBuilder.Append("UseAnimation; ");
				}
				if (betterListViewToolTipOptions.UseFading) {
					stringBuilder.Append("UseFading; ");
				}
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
				return stringBuilder.ToString();
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				Type[] array = new Type[7];
				object[] array2 = new object[7];
				array[0] = typeof(int);
				array2[0] = betterListViewToolTipOptions.AutomaticDelay;
				array[1] = typeof(int);
				array2[1] = betterListViewToolTipOptions.AutoPopDelay;
				array[2] = typeof(int);
				array2[2] = betterListViewToolTipOptions.InitialDelay;
				array[3] = typeof(int);
				array2[3] = betterListViewToolTipOptions.ReshowDelay;
				array[4] = typeof(bool);
				array2[4] = betterListViewToolTipOptions.ShowAlways;
				array[5] = typeof(bool);
				array2[5] = betterListViewToolTipOptions.UseAnimation;
				array[6] = typeof(bool);
				array2[6] = betterListViewToolTipOptions.UseFading;
				return new InstanceDescriptor(typeof(BetterListViewToolTipOptions).GetConstructor(array), array2);
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
			return new BetterListViewToolTipOptions((int)propertyValues["AutomaticDelay"], (int)propertyValues["AutoPopDelay"], (int)propertyValues["InitialDelay"], (int)propertyValues["ReshowDelay"], (bool)propertyValues["ShowAlways"], (bool)propertyValues["UseAnimation"], (bool)propertyValues["UseFading"]);
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
			return true;
		}
	}
}
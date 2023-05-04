using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type converter for ReadOnlySet{int} (BetterListViewSearchSettings.SubItemIndices).
	/// </summary>
	internal sealed class SubItemIndicesConverter : TypeConverter
	{
		private const char Separator = ';';

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string text) {
				string[] array = text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				Set<int> set = new Set<int>();
				string[] array2 = array;
				string[] array3 = array2;
				foreach (string text2 in array3) {
					if (int.TryParse(text2, NumberStyles.Integer, culture, out var result) && result >= 0) {
						set.Add(result);
						continue;
					}
					throw new FormatException("Invalid number format: '" + text2 + "'.");
				}
				return set.AsReadOnly();
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			ReadOnlySet<int> readOnlySet = (ReadOnlySet<int>)value;
			if (destinationType == typeof(string)) {
				StringBuilder stringBuilder = new StringBuilder();
				foreach (int item in readOnlySet) {
					stringBuilder.AppendFormat("{0}{1} ", item.ToString(culture), ';');
				}
				if (readOnlySet.Count != 0) {
					stringBuilder.Remove(stringBuilder.Length - 2, 2);
				}
				return stringBuilder.ToString();
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				return new InstanceDescriptor(typeof(ReadOnlySet<int>).GetConstructor(new Type[1] { typeof(int[]) }), new int[1][] { readOnlySet.ToArray() });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
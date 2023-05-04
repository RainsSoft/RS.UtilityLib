using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ComponentOwl.BetterListView.Design
{

	/// <summary>
	///   UI type editor for flag enum.
	/// </summary>
	internal sealed class FlagEnumUITypeEditor : UITypeEditor
	{
		private readonly FlagCheckedListBox flagCheckedListBox;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.Design.FlagEnumUITypeEditor" /> class.
		/// </summary>
		public FlagEnumUITypeEditor() {
			this.flagCheckedListBox = new FlagCheckedListBox();
		}

		/// <summary>
		/// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle" /> method.
		/// </summary>
		/// <returns>
		/// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to gain additional context information. 
		///                 </param><param name="provider">An <see cref="T:System.IServiceProvider" /> that this editor can use to obtain services. 
		///                 </param><param name="value">The object to edit. 
		///                 </param>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context.Instance != null) {
				IWindowsFormsEditorService windowsFormsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (windowsFormsEditorService != null) {
					Enum currentValueEnum = (Enum)Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);
					this.flagCheckedListBox.CurrentValueEnum = currentValueEnum;
					windowsFormsEditorService.DropDownControl(this.flagCheckedListBox);
					return this.flagCheckedListBox.CurrentValueEnum;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)" /> method.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle" /> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)" /> method. If the <see cref="T:System.Drawing.Design.UITypeEditor" /> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle" /> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None" />.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to gain additional context information. 
		///                 </param>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
	}
}
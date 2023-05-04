using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView.Design
{

	/// <summary>
	///   CheckedListBox for editing flag enums.
	/// </summary>
	[ToolboxItem(false)]
	internal sealed class FlagCheckedListBox : CheckedListBox
	{
		private Type enumType;

		private bool isUpdatingCheckStates;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Enum CurrentValueEnum {
			get {
				return (Enum)Enum.ToObject(this.enumType, this.CurrentValueInt);
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.enumType = value.GetType();
				this.FillEnumMembers();
				this.UpdateCheckedItems((int)Convert.ChangeType(value, typeof(int)));
			}
		}

		private int CurrentValueInt {
			get {
				int num = 0;
				foreach (FlagCheckedListBoxItem checkedItem in base.CheckedItems) {
					num |= checkedItem.Value;
				}
				return num;
			}
		}

		/// <summary>
		///   Initialize a new FlagCheckedListBox instance.
		/// </summary>
		public FlagCheckedListBox() {
			base.BorderStyle = BorderStyle.None;
			base.CheckOnClick = true;
		}

		protected override void OnItemCheck(ItemCheckEventArgs eventArgs) {
			base.OnItemCheck(eventArgs);
			if (!this.isUpdatingCheckStates) {
				this.UpdateCheckedItems((FlagCheckedListBoxItem)base.Items[eventArgs.Index], eventArgs.NewValue);
			}
		}

		private void FillEnumMembers() {
			try {
				base.BeginUpdate();
				base.Items.Clear();
				string[] names = Enum.GetNames(this.enumType);
				string[] array = names;
				foreach (string text in array) {
					object value = Enum.Parse(this.enumType, text);
					int value2 = (int)Convert.ChangeType(value, typeof(int));
					base.Items.Add(new FlagCheckedListBoxItem(text, value2));
				}
			}
			finally {
				base.EndUpdate();
			}
		}

		private void UpdateCheckedItems(int intValue) {
			this.isUpdatingCheckStates = true;
			for (int i = 0; i < base.Items.Count; i++) {
				FlagCheckedListBoxItem flagCheckedListBoxItem = (FlagCheckedListBoxItem)base.Items[i];
				if (flagCheckedListBoxItem.Value == 0) {
					base.SetItemChecked(i, intValue == 0);
				}
				else {
					base.SetItemChecked(i, (flagCheckedListBoxItem.Value & intValue) == flagCheckedListBoxItem.Value && flagCheckedListBoxItem.Value != 0);
				}
			}
			this.isUpdatingCheckStates = false;
		}

		private void UpdateCheckedItems(FlagCheckedListBoxItem item, CheckState checkState) {
			if (item.Value == 0) {
				this.UpdateCheckedItems(0);
			}
			int currentValueInt = this.CurrentValueInt;
			currentValueInt = ((checkState != 0) ? (currentValueInt | item.Value) : (currentValueInt & ~item.Value));
			this.UpdateCheckedItems(currentValueInt);
		}
	}
}
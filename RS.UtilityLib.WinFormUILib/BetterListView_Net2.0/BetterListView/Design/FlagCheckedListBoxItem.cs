namespace ComponentOwl.BetterListView.Design
{

	/// <summary>
	///   Item for FlagCheckedListBox.
	/// </summary>
	internal sealed class FlagCheckedListBoxItem
	{
		private readonly string caption;

		private readonly int value;

		/// <summary>
		///   this value corresponds to a single bit being set
		/// </summary>
		public bool IsFlag => (this.value & (this.value - 1)) == 0;

		/// <summary>
		///   enum value
		/// </summary>
		public int Value => this.value;

		/// <summary>
		///   Initialize a new FlagCheckedListBoxItem instance.
		/// </summary>
		/// <param name="caption">enum name</param>
		/// <param name="value">enum value</param>
		public FlagCheckedListBoxItem(string caption, int value) {
			Checks.CheckString(caption, "caption");
			this.caption = caption;
			this.value = value;
		}

		/// <summary>
		///   Checks whether This value is a member of the composite bit value.
		/// </summary>
		/// <param name="item">another flags enum member</param>
		/// <returns>this value is a member of the composite bit value</returns>
		public bool IsMemberFlag(FlagCheckedListBoxItem item) {
			Checks.CheckFalse(this == item, "ReferenceEquals(this, item)");
			if (this.IsFlag) {
				return (this.value & item.Value) == this.value;
			}
			return false;
		}

		public override string ToString() {
			return this.caption;
		}
	}
}
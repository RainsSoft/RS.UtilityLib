namespace ComponentOwl.BetterListView
{

	internal sealed class TextBreak
	{
		private int width;

		private TextSize textSize;

		private TextBreak nextBreak;

		public int Width => this.width;

		public TextSize TextSize => this.textSize;

		/// <summary>
		///   reference to nearest text break with greater bounding width
		/// </summary>
		public TextBreak NextBreak {
			get {
				return this.nextBreak;
			}
			set {
				if (this.nextBreak != null) {
					Checks.CheckTrue(this.nextBreak.width > this.width, "this.nextBreak.width > width");
					Checks.CheckTrue(this.nextBreak.textSize.Height <= this.textSize.Height, "this.nextBreak.textSize.Height <= this.textSize.Height");
				}
				this.nextBreak = value;
			}
		}

		public TextBreak(int width, TextSize textSize, TextBreak nextBreak) {
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckFalse(textSize.IsEmpty, "textSize.IsEmpty");
			if (nextBreak != null) {
				Checks.CheckTrue(nextBreak.width > width, "nextBreak.width > width");
				Checks.CheckTrue(nextBreak.textSize.Height <= textSize.Height, "nextBreak.textSize.Height <= textSize.Height");
			}
			this.width = width;
			this.textSize = textSize;
			this.nextBreak = nextBreak;
		}
	}
}
using System;
using System.Text;

namespace ComponentOwl.BetterListView
{

	internal struct AdjustedText
	{
		public static readonly AdjustedText Empty = new AdjustedText(new string[1] { "" });

		private string[] textLines;

		public string[] TextLines => this.textLines;

		public bool IsEmpty => this.Equals(AdjustedText.Empty);

		public AdjustedText(string[] textLines) {
			Checks.CheckNotNull(textLines, "textLines");
			foreach (string param in textLines) {
				Checks.CheckNotNull(param, "line");
			}
			this.textLines = textLines;
		}

		public override bool Equals(object obj) {
			if (!(obj is AdjustedText adjustedText)) {
				return false;
			}
			if (this.textLines.Length != adjustedText.textLines.Length) {
				return false;
			}
			for (int i = 0; i < this.textLines.Length; i++) {
				if (!this.textLines[i].Equals(adjustedText.textLines[i], StringComparison.Ordinal)) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode() {
			int num = this.textLines[0].GetHashCode();
			for (int i = 1; i < this.textLines.Length; i++) {
				num ^= this.textLines[i].GetHashCode();
			}
			return num;
		}

		public override string ToString() {
			if (this.textLines.Length == 1) {
				return this.textLines[0];
			}
			int num = this.textLines.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < num; i++) {
				stringBuilder.AppendLine(this.textLines[i]);
			}
			stringBuilder.Append(this.textLines[num]);
			return stringBuilder.ToString();
		}
	}
}
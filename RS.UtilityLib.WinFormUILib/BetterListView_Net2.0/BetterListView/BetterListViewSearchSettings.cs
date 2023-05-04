using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using ComponentOwl.BetterListView.Collections;
using ComponentOwl.BetterListView.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Keyboard search settings.
	/// </summary>
	[TypeConverter(typeof(BetterListViewSearchSettingsConverter))]
	public struct BetterListViewSearchSettings
	{
		/// <summary>
		///   represents an empty BetterListViewSearchSettings structure
		/// </summary>
		public static readonly BetterListViewSearchSettings Empty = new BetterListViewSearchSettings(BetterListViewSearchMode.Disabled, BetterListViewSearchOptions.None, new int[0]);

		private readonly BetterListViewSearchMode mode;

		private readonly ReadOnlySet<int> subItemIndices;

		private BetterListViewSearchOptions options;

		/// <summary>
		///   search mode
		/// </summary>
		public BetterListViewSearchMode Mode => this.mode;

		/// <summary>
		///   search options
		/// </summary>
		[Editor(typeof(FlagEnumUITypeEditor), typeof(UITypeEditor))]
		public BetterListViewSearchOptions Options {
			get {
				return this.options;
			}
			set {
				this.options = value;
			}
		}

		/// <summary>
		///   indices of the sub-items to be searched; if empty, all sub-items are searched
		/// </summary>
		[TypeConverter(typeof(SubItemIndicesConverter))]
		public ReadOnlySet<int> SubItemIndices => this.subItemIndices;

		/// <summary>
		///   this BetterListViewSearchSettings structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewSearchSettings.Empty);

		/// <summary>
		///   Initialize a new BetterListViewSearchSettings instance.
		/// </summary>
		/// <param name="mode">search mode</param>
		public BetterListViewSearchSettings(BetterListViewSearchMode mode)
			: this(mode, BetterListViewSearchOptions.None) {
		}

		/// <summary>
		///   Initialize a new BetterListViewSearchSettings instance.
		/// </summary>
		/// <param name="mode">search mode</param>
		/// <param name="options">search options</param>
		public BetterListViewSearchSettings(BetterListViewSearchMode mode, BetterListViewSearchOptions options)
			: this(mode, options, new int[0]) {
		}

		/// <summary>
		///   Initialize a new BetterListViewSearchSettings instance.
		/// </summary>
		/// <param name="mode">search mode</param>
		/// <param name="options">search options</param>
		/// <param name="subItemIndices">indices of the sub-items to be searched; if empty, all sub-items are searched</param>
		public BetterListViewSearchSettings(BetterListViewSearchMode mode, BetterListViewSearchOptions options, IEnumerable<int> subItemIndices) {
			Checks.CheckNotNull(subItemIndices, "subItemIndices");
			foreach (int subItemIndex in subItemIndices) {
				Checks.CheckTrue(subItemIndex >= 0, "index >= 0");
			}
			this.mode = mode;
			this.options = options;
			this.subItemIndices = new ReadOnlySet<int>(subItemIndices);
		}

		/// <summary>
		///   Test whether the two BetterListViewSearchSettings objects are identical.
		/// </summary>
		/// <param name="searchSettingsA">first BetterListViewSearchSettings object</param>
		/// <param name="searchSettingsB">second BetterListViewSearchSettings object</param>
		/// <returns>
		///   the two BetterListViewSearchSettings objects are identical
		/// </returns>
		public static bool operator ==(BetterListViewSearchSettings searchSettingsA, BetterListViewSearchSettings searchSettingsB) {
			return searchSettingsA.Equals(searchSettingsB);
		}

		/// <summary>
		///   Test whether the two BetterListViewSearchSettings objects are different.
		/// </summary>
		/// <param name="searchSettingsA">first BetterListViewSearchSettings object</param>
		/// <param name="searchSettingsB">second BetterListViewSearchSettings object</param>
		/// <returns>
		///   the two BetterListViewSearchSettings objects are different
		/// </returns>
		public static bool operator !=(BetterListViewSearchSettings searchSettingsA, BetterListViewSearchSettings searchSettingsB) {
			return !searchSettingsA.Equals(searchSettingsB);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewSearchSettings betterListViewSearchSettings)) {
				return false;
			}
			if (this.mode == betterListViewSearchSettings.mode && this.options == betterListViewSearchSettings.options) {
				return this.subItemIndices.Equals(betterListViewSearchSettings.subItemIndices);
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.mode.GetHashCode() ^ this.options.GetHashCode() ^ this.subItemIndices.Count.GetHashCode();
		}
	}
}
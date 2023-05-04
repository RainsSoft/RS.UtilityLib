using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Information about level of invalidation and the invalidated area.
	/// </summary>
	public struct BetterListViewInvalidationInfo
	{
		/// <summary>
		///   empty BetterListViewInvalidationInfo structure
		/// </summary>
		public static readonly BetterListViewInvalidationInfo Empty = new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.None, Rectangle.Empty);

		private readonly BetterListViewInvalidationLevel level;

		private readonly BetterListViewInvalidationFlags flags;

		private readonly Rectangle region;

		/// <summary>
		///   level of control invalidation
		/// </summary>
		public BetterListViewInvalidationLevel Level => this.level;

		/// <summary>
		///   control invalidation options
		/// </summary>
		public BetterListViewInvalidationFlags Flags => this.flags;

		/// <summary>
		///   invalidated region
		/// </summary>
		public Rectangle Region => this.region;

		/// <summary>
		///   this BetterListViewInvalidationInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewInvalidationInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewInvalidationInfo instance.
		/// </summary>
		/// <param name="level">level of control invalidation</param>
		/// <param name="flags">control invalidation options</param>
		/// <param name="region">invalidated region</param>
		public BetterListViewInvalidationInfo(BetterListViewInvalidationLevel level, BetterListViewInvalidationFlags flags, Rectangle region) {
			this.level = level;
			this.flags = flags;
			this.region = region;
		}

		/// <summary>
		///   Test whether the two BetterListViewInvalidationInfo object are identical.
		/// </summary>
		/// <param name="invalidationInfoA">first BetterListViewInvalidationInfo object</param>
		/// <param name="invalidationInfoB">second BetterListViewInvalidationInfo object</param>
		/// <returns>
		///   the two BetterListViewInvalidationInfo object are identical
		/// </returns>
		public static bool operator ==(BetterListViewInvalidationInfo invalidationInfoA, BetterListViewInvalidationInfo invalidationInfoB) {
			return invalidationInfoA.Equals(invalidationInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewInvalidationInfo object are different.
		/// </summary>
		/// <param name="invalidationInfoA">first BetterListViewInvalidationInfo object</param>
		/// <param name="invalidationInfoB">second BetterListViewInvalidationInfo object</param>
		/// <returns>
		///   the two BetterListViewInvalidationInfo object are different
		/// </returns>
		public static bool operator !=(BetterListViewInvalidationInfo invalidationInfoA, BetterListViewInvalidationInfo invalidationInfoB) {
			return !invalidationInfoA.Equals(invalidationInfoB);
		}

		/// <summary>
		///   Union this invalidation info with the specified invalidation info.
		/// </summary>
		/// <param name="invalidationInfo">invalidation info to union with</param>
		/// <returns>combined invalidation info</returns>
		public BetterListViewInvalidationInfo UnionWith(BetterListViewInvalidationInfo invalidationInfo) {
			BetterListViewInvalidationLevel betterListViewInvalidationLevel = (BetterListViewInvalidationLevel)Math.Max((int)this.level, (int)invalidationInfo.Level);
			BetterListViewInvalidationFlags betterListViewInvalidationFlags = this.flags | invalidationInfo.Flags;
			Rectangle rectangle = (this.region.IsEmpty ? invalidationInfo.Region : Rectangle.Union(this.region, invalidationInfo.Region));
			return new BetterListViewInvalidationInfo(betterListViewInvalidationLevel, betterListViewInvalidationFlags, rectangle);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewInvalidationInfo betterListViewInvalidationInfo)) {
				return false;
			}
			if (this.level == betterListViewInvalidationInfo.level && this.flags == betterListViewInvalidationInfo.flags) {
				return this.region == betterListViewInvalidationInfo.region;
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
			int num = this.level.GetHashCode() ^ this.flags.GetHashCode();
			Rectangle rectangle = this.region;
			return num ^ rectangle.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			return $"{base.GetType().Name}: {{Level = '{this.level}', Flags = '{this.flags}', Region = '{this.region}'}}";
		}
	}
}
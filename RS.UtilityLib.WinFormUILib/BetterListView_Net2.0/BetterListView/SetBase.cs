using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Base class for hash sets.
	/// </summary>
	/// <typeparam name="TItem">item type</typeparam>
	public abstract class SetBase<TItem> : IEnumerable<TItem>, IEnumerable
	{
		private readonly Dictionary<TItem, object> innerSet;

		/// <summary>
		///   number of items within the set
		/// </summary>
		public int Count => this.innerSet.Count;

		/// <summary>
		///   Underlying set structure.
		/// </summary>
		protected Dictionary<TItem, object> InnerSet => this.innerSet;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.SetBase`1" /> class.
		/// </summary>
		protected SetBase() {
			this.innerSet = new Dictionary<TItem, object>();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.SetBase`1" /> class.
		/// </summary>
		/// <param name="enumerable">enumerable to create this set from</param>
		protected SetBase(IEnumerable<TItem> enumerable) {
			Checks.CheckNotNull(enumerable, "enumerable");
			this.innerSet = new Dictionary<TItem, object>();
			foreach (TItem item in enumerable) {
				this.innerSet.Add(item, null);
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.SetBase`1" /> class.
		/// </summary>
		/// <param name="comparer">value comparer</param>
		protected SetBase(IEqualityComparer<TItem> comparer) {
			Checks.CheckNotNull(comparer, "comparer");
			this.innerSet = new Dictionary<TItem, object>(comparer);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.SetBase`1" /> class.
		/// </summary>
		/// <param name="enumerable">enumerable to create this set from</param>
		/// <param name="comparer">value comparer</param>
		protected SetBase(IEnumerable<TItem> enumerable, IEqualityComparer<TItem> comparer) {
			Checks.CheckNotNull(enumerable, "enumerable");
			Checks.CheckNotNull(comparer, "comparer");
			this.innerSet = new Dictionary<TItem, object>(comparer);
			foreach (TItem item in enumerable) {
				this.innerSet.Add(item, null);
			}
		}

		/// <summary>
		///   Determines whether [contains] [the specified item].
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>
		///   <c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(TItem item) {
			return this.innerSet.ContainsKey(item);
		}

		/// <summary>
		///   Copy this set to the specified array.
		/// </summary>
		/// <param name="array">array to copy this set to</param>
		public void CopyTo(TItem[] array) {
			Checks.CheckNotNull(array, "array");
			this.innerSet.Keys.CopyTo(array, 0);
		}

		/// <summary>
		///   Copy this set to the specified array.
		/// </summary>
		/// <param name="array">array to copy this set to</param>
		/// <param name="arrayIndex">start index in the target array</param>
		public void CopyTo(TItem[] array, int arrayIndex) {
			Checks.CheckNotNull(array, "array");
			if (array.Length == 0) {
				Checks.CheckEqual(arrayIndex, 0, "arrayIndex", "0");
			}
			else {
				Checks.CheckBounds(arrayIndex, 0, array.Length - 1, "arrayIndex");
			}
			this.innerSet.Keys.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///   Get array containing items from this set.
		/// </summary>
		/// <returns>an array containing items from this set</returns>
		public TItem[] ToArray() {
			TItem[] array = new TItem[this.Count];
			this.CopyTo(array);
			return array;
		}

		/// <summary>
		/// Check whether contents of this set equals contents of the specified set.
		/// </summary>
		/// <param name="other">Set to check.</param>
		/// <returns>Contents of this set equals contents of the specified set.</returns>
		public bool EqualsContent(SetBase<TItem> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			Dictionary<TItem, object> dictionary = this.innerSet;
			Dictionary<TItem, object> dictionary2 = other.innerSet;
			if (dictionary.Count != dictionary2.Count) {
				return false;
			}
			foreach (TItem key in dictionary.Keys) {
				if (!dictionary2.ContainsKey(key)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			return this.ToString(writeContent: false);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <param name="writeContent">output content of the collection</param>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public string ToString(bool writeContent) {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetType().Name);
			if (writeContent) {
				stringBuilder.Append(": {");
				foreach (TItem key in this.innerSet.Keys) {
					stringBuilder.AppendFormat("'{0}', ", key);
				}
				if (this.innerSet.Count != 0) {
					stringBuilder.Remove(stringBuilder.Length - 2, 2);
				}
				stringBuilder.Append("}");
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator() {
			return this.innerSet.Keys.GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public IEnumerator GetEnumerator() {
			return this.innerSet.Keys.GetEnumerator();
		}
	}
}
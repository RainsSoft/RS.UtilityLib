using System.ComponentModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Forces AllowDrop property to use our ShouldSerialize and Reset methods.
	/// </summary>
	internal class BetterListViewAllowDropPropertyDescriptor : BetterListViewPropertyDescriptor
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewAllowDropPropertyDescriptor" /> class.
		/// </summary>
		/// <param name="propertyDescriptor">PropertyDescriptor to create this PropertyDescriptor from.</param>
		public BetterListViewAllowDropPropertyDescriptor(PropertyDescriptor propertyDescriptor)
			: base(propertyDescriptor) {
		}

		/// <summary>
		///   When overridden in a derived class, returns whether resetting an object changes its value.
		/// </summary>
		/// <returns>
		///   true if resetting the component changes its value; otherwise, false.
		/// </returns>
		/// <param name="component">The component to test for reset capability. 
		/// </param>
		public override bool CanResetValue(object component) {
			return this.ShouldSerializeValue(component);
		}

		/// <summary>
		///   When overridden in a derived class, resets the value for this property of the component to the default value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value. 
		/// </param>
		public override void ResetValue(object component) {
			((BetterListView)component).ResetAllowDrop();
		}

		/// <summary>
		///   When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
		/// </summary>
		/// <returns>
		///   true if the property should be persisted; otherwise, false.
		/// </returns>
		/// <param name="component">The component with the property to be examined for persistence. 
		/// </param>
		public override bool ShouldSerializeValue(object component) {
			return ((BetterListView)component).ShouldSerializeAllowDrop();
		}
	}
}
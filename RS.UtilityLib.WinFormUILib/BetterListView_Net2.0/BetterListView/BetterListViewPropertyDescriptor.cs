using System;
using System.ComponentModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   PropertyDescriptor wrapper for BetterListView.
	/// </summary>
	internal class BetterListViewPropertyDescriptor : PropertyDescriptor
	{
		private readonly PropertyDescriptor propertyDescriptor;

		/// <summary>
		///   When overridden in a derived class, gets the type of the component this property is bound to.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Type" /> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)" /> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)" /> methods are invoked, the object specified might be an instance of this type.
		/// </returns>
		public override Type ComponentType => this.propertyDescriptor.ComponentType;

		/// <summary>
		///   Gets the name that can be displayed in a window, such as a Properties window.
		/// </summary>
		/// <returns>
		///   The name to display for the member.
		/// </returns>
		public override string DisplayName => this.propertyDescriptor.DisplayName;

		/// <summary>
		///   When overridden in a derived class, gets a value indicating whether this property is read-only.
		/// </summary>
		/// <returns>
		///   true if the property is read-only; otherwise, false.
		/// </returns>
		public override bool IsReadOnly => this.propertyDescriptor.IsReadOnly;

		/// <summary>
		///   When overridden in a derived class, gets the type of the property.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Type" /> that represents the type of the property.
		/// </returns>
		public override Type PropertyType => this.propertyDescriptor.PropertyType;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewPropertyDescriptor" /> class.
		/// </summary>
		/// <param name="propertyDescriptor">PropertyDescriptor to create this PropertyDescriptor from</param>
		public BetterListViewPropertyDescriptor(PropertyDescriptor propertyDescriptor)
			: base(propertyDescriptor) {
			this.propertyDescriptor = propertyDescriptor;
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
			return this.propertyDescriptor.CanResetValue(component);
		}

		/// <summary>
		///   When overridden in a derived class, gets the current value of the property on a component.
		/// </summary>
		/// <returns>
		///   The value of a property for a given component.
		/// </returns>
		/// <param name="component">The component with the property for which to retrieve the value. 
		/// </param>
		public override object GetValue(object component) {
			return this.propertyDescriptor.GetValue(component);
		}

		/// <summary>
		///   When overridden in a derived class, resets the value for this property of the component to the default value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value. 
		/// </param>
		public override void ResetValue(object component) {
			this.propertyDescriptor.ResetValue(component);
		}

		/// <summary>
		///   When overridden in a derived class, sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set. 
		/// </param>
		/// <param name="value">The new value. 
		/// </param>
		public override void SetValue(object component, object value) {
			this.propertyDescriptor.SetValue(component, value);
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
			return this.propertyDescriptor.ShouldSerializeValue(component);
		}
	}
}
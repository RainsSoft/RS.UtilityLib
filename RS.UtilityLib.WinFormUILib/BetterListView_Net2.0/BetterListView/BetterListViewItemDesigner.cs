using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Custom component designer for BetterListViewItem.
	/// </summary>
	internal sealed class BetterListViewItemDesigner : BetterListViewElementDesigner
	{
		/// <summary>
		///   Gets the collection of components associated with the component managed by the designer.
		/// </summary>
		/// <returns>
		///   The components that are associated with the component managed by the designer.
		/// </returns>
		public override ICollection AssociatedComponents {
			get {
				if (base.Component is BetterListViewItem betterListViewItem) {
					return betterListViewItem.SubItems;
				}
				return base.AssociatedComponents;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemDesigner" /> class.
		/// </summary>
		public BetterListViewItemDesigner()
			: base(generateMemberDefault: false) {
		}

		/// <summary>
		///   Prepares the designer to view, edit, and design the specified component.
		/// </summary>
		/// <param name="component">The component for this designer. 
		/// </param>
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if (this.GetService(typeof(IComponentChangeService)) is IComponentChangeService componentChangeService) {
				componentChangeService.ComponentRemoving += ComponentChangeServiceComponentRemoving;
			}
		}

		/// <summary>
		///   Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Design.ComponentDesigner" /> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. 
		/// </param>
		protected override void Dispose(bool disposing) {
			if (disposing && this.GetService(typeof(IComponentChangeService)) is IComponentChangeService componentChangeService) {
				componentChangeService.ComponentRemoving -= ComponentChangeServiceComponentRemoving;
			}
			base.Dispose(disposing);
		}

		private void ComponentChangeServiceComponentRemoving(object sender, ComponentEventArgs eventArgs) {
			IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			BetterListViewItem betterListViewItem = (BetterListViewItem)base.Component;
			if (betterListViewItem == null) {
				return;
			}
			if (eventArgs.Component is BetterListViewSubItem) {
				BetterListViewSubItem item = (BetterListViewSubItem)eventArgs.Component;
				if (betterListViewItem.SubItems.Contains(item) && betterListViewItem.SubItems.Count != 1) {
					componentChangeService?.OnComponentChanging(betterListViewItem, null);
					betterListViewItem.SubItems.Remove(item);
					componentChangeService?.OnComponentChanged(betterListViewItem, null, null, null);
					return;
				}
			}
			if (eventArgs.Component != base.Component) {
				return;
			}
			for (int num = betterListViewItem.SubItems.Count - 1; num >= 0; num--) {
				BetterListViewSubItem betterListViewSubItem = betterListViewItem.SubItems[num];
				componentChangeService?.OnComponentChanging(betterListViewItem, null);
				if (num != 0) {
					betterListViewItem.SubItems.Remove(betterListViewSubItem);
				}
				designerHost?.DestroyComponent(betterListViewSubItem);
				componentChangeService?.OnComponentChanged(betterListViewItem, null, null, null);
			}
		}
	}
}
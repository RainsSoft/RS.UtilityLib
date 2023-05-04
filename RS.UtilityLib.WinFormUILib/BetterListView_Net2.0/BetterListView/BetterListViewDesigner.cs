using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Custom designer for BetterListView.
	/// </summary>
	internal sealed class BetterListViewDesigner : ControlDesigner
	{
		private DesignerActionListCollection actionLists;

		public override DesignerActionListCollection ActionLists {
			get {
				if (this.actionLists == null) {
					this.actionLists = new DesignerActionListCollection
					{
					new BetterListViewActionList(this)
				};
				}
				return this.actionLists;
			}
		}

		public override ICollection AssociatedComponents {
			get {
				if (this.Control is BetterListView betterListView) {
					List<object> list = new List<object>(betterListView.Columns.Count + betterListView.Groups.Count + betterListView.Items.Count);
					foreach (BetterListViewColumnHeader column in betterListView.Columns) {
						list.Add(column);
					}
					foreach (BetterListViewGroup group in betterListView.Groups) {
						list.Add(group);
					}
					foreach (BetterListViewItem item in betterListView.Items) {
						list.Add(item);
					}
					return list.ToArray();
				}
				return base.AssociatedComponents;
			}
		}

		public override void Initialize(IComponent component) {
			base.Initialize(component);
			BetterListView betterListView = (BetterListView)component;
			betterListView.ColumnWidthChanged += ControlOnColumnWidthChanged;
			IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentRemoving += OnComponentRemoving;
		}

		protected override void Dispose(bool disposing) {
			if (disposing && this.Control != null) {
				BetterListView betterListView = (BetterListView)this.Control;
				betterListView.ColumnWidthChanged -= ControlOnColumnWidthChanged;
				IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
				if (componentChangeService != null) {
					componentChangeService.ComponentRemoving -= OnComponentRemoving;
				}
			}
			base.Dispose(disposing);
		}

		protected override bool GetHitTest(Point point) {
			BetterListView betterListView = (BetterListView)base.Component;
			BetterListViewHitTestInfo betterListViewHitTestInfo = betterListView.HitTest(betterListView.PointToClient(point));
			if ((betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderBorder) == BetterListViewHitTestLocations.ColumnHeaderBorder) {
				return betterListViewHitTestInfo.ColumnHeader.AllowResize;
			}
			return false;
		}

		protected override void PostFilterProperties(IDictionary properties) {
			PropertyDescriptor propertyDescriptor = (PropertyDescriptor)properties["AllowDrop"];
			if (propertyDescriptor != null) {
				propertyDescriptor = TypeDescriptor.CreateProperty(typeof(BetterListView), propertyDescriptor);
				properties["AllowDrop"] = new BetterListViewAllowDropPropertyDescriptor(propertyDescriptor);
			}
			base.PostFilterProperties(properties);
		}

		private void OnComponentRemoving(object sender, ComponentEventArgs eventArgs) {
			IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			BetterListView betterListView = (BetterListView)this.Control;
			if (betterListView == null) {
				return;
			}
			bool suppressRefresh = false;
			try {
				betterListView.BeginUpdate();
				if (eventArgs.Component is BetterListViewColumnHeader) {
					suppressRefresh = false;
					BetterListViewColumnHeader item = (BetterListViewColumnHeader)eventArgs.Component;
					if (betterListView.Columns.Contains(item)) {
						componentChangeService?.OnComponentChanging(betterListView, null);
						betterListView.Columns.Remove(item);
						componentChangeService?.OnComponentChanged(betterListView, null, null, null);
					}
				}
				else if (eventArgs.Component is BetterListViewItem) {
					suppressRefresh = false;
					BetterListViewItem item2 = (BetterListViewItem)eventArgs.Component;
					if (betterListView.Items.Contains(item2)) {
						componentChangeService?.OnComponentChanging(betterListView, null);
						betterListView.Items.Remove(item2);
						componentChangeService?.OnComponentChanged(betterListView, null, null, null);
					}
				}
				else if (eventArgs.Component is BetterListViewGroup) {
					suppressRefresh = false;
					BetterListViewGroup item3 = (BetterListViewGroup)eventArgs.Component;
					if (betterListView.Groups.Contains(item3)) {
						componentChangeService?.OnComponentChanging(betterListView, null);
						betterListView.Groups.Remove(item3);
						componentChangeService?.OnComponentChanged(betterListView, null, null, null);
					}
				}
				else if (eventArgs.Component == base.Component) {
					suppressRefresh = true;
					for (int num = betterListView.Columns.Count - 1; num >= 0; num--) {
						BetterListViewColumnHeader betterListViewColumnHeader = betterListView.Columns[num];
						componentChangeService?.OnComponentChanging(betterListView, null);
						betterListView.Columns.Remove(betterListViewColumnHeader);
						designerHost?.DestroyComponent(betterListViewColumnHeader);
						componentChangeService?.OnComponentChanged(betterListView, null, null, null);
					}
					for (int num2 = betterListView.Items.Count - 1; num2 >= 0; num2--) {
						BetterListViewItem betterListViewItem = betterListView.Items[num2];
						componentChangeService?.OnComponentChanging(betterListView, null);
						betterListView.Items.Remove(betterListViewItem);
						designerHost?.DestroyComponent(betterListViewItem);
						componentChangeService?.OnComponentChanged(betterListView, null, null, null);
					}
					for (int num3 = betterListView.Groups.Count - 1; num3 >= 0; num3--) {
						BetterListViewGroup betterListViewGroup = betterListView.Groups[num3];
						componentChangeService?.OnComponentChanging(betterListView, null);
						betterListView.Groups.Remove(betterListViewGroup);
						designerHost?.DestroyComponent(betterListViewGroup);
						componentChangeService?.OnComponentChanged(betterListView, null, null, null);
					}
				}
			}
			finally {
				betterListView.EndUpdate(suppressRefresh);
			}
		}

		private void ControlOnColumnWidthChanged(object sender, BetterListViewColumnWidthChangedEventArgs eventArgs) {
			((IComponentChangeService)this.GetService(typeof(IComponentChangeService)))?.OnComponentChanged(eventArgs.ColumnHeader, TypeDescriptor.GetProperties(base.Component)["Width"], null, null);
		}
	}
}
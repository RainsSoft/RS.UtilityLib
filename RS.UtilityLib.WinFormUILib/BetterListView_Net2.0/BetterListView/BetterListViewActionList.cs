using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Reprents BetterListView ActionList for Component Designer.
	/// </summary>
	internal sealed class BetterListViewActionList : DesignerActionList
	{
		private readonly ComponentDesigner designer;

		/// <summary>
		///   Gets or sets the value represeting ContextMenuStrip property of the Better ListView component.
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public ContextMenuStrip ContextMenuStrip {
			get {
				return ((BetterListView)base.Component).ContextMenuStrip;
			}
			set {
				TypeDescriptor.GetProperties(base.Component)["ContextMenuStrip"].SetValue(base.Component, value);
			}
		}

		/// <summary>
		///   Gets or sets the value represeting ContextMenuStripColumns property of the Better ListView component.
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public ContextMenuStrip ContextMenuStripColumns {
			get {
				return ((BetterListView)base.Component).ContextMenuStripColumns;
			}
			set {
				TypeDescriptor.GetProperties(base.Component)["ContextMenuStripColumns"].SetValue(base.Component, value);
			}
		}

		/// <summary>
		///   Gets or sets the value represeting ContextMenuStripItems property of the Better ListView component.
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public ContextMenuStrip ContextMenuStripItems {
			get {
				return ((BetterListView)base.Component).ContextMenuStripItems;
			}
			set {
				TypeDescriptor.GetProperties(base.Component)["ContextMenuStripItems"].SetValue(base.Component, value);
			}
		}

		/// <summary>
		///   Gets or sets the value represeting MultiSelect property of the Better ListView component
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public bool MultiSelect {
			get {
				return ((BetterListView)base.Component).MultiSelect;
			}
			set {
				TypeDescriptor.GetProperties(base.Component)["MultiSelect"].SetValue(base.Component, value);
			}
		}

		/// <summary>
		///   Gets or sets the value represeting View property of the Better ListView component
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public BetterListViewView View {
			get {
				return ((BetterListView)base.Component).View;
			}
			set {
				TypeDescriptor.GetProperties(base.Component)["View"].SetValue(base.Component, value);
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewActionList" /> class.
		/// </summary>
		/// <param name="designer">a component related to the System.ComponentModel.Design.DesignerActionList</param>
		public BetterListViewActionList(ComponentDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}

		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection designerActionItemCollection = new DesignerActionItemCollection();
			designerActionItemCollection.Add(new DesignerActionMethodItem(this, "InvokeItemsDialog", "Edit Items", "Properties", "Opens the Items collection editor", includeAsDesignerVerb: true));
			designerActionItemCollection.Add(new DesignerActionMethodItem(this, "InvokeColumnsDialog", "Edit Columns", "Properties", "Opens the Columns collection editor", includeAsDesignerVerb: true));
			designerActionItemCollection.Add(new DesignerActionMethodItem(this, "InvokeGroupsDialog", "Edit Groups", "Properties", "Opens the Groups collection editor", includeAsDesignerVerb: true));
			designerActionItemCollection.Add(new DesignerActionHeaderItem("Properties", "Properties"));
			designerActionItemCollection.Add(new DesignerActionPropertyItem("View", "View", "Properties", "Changes the type of View this ListView shows"));
			designerActionItemCollection.Add(new DesignerActionPropertyItem("MultiSelect", "Multi Select", "Properties", "Allows multiple items to be selected"));
			designerActionItemCollection.Add(new DesignerActionHeaderItem("Context Menus", "Properties"));
			designerActionItemCollection.Add(new DesignerActionPropertyItem("ContextMenuStrip", "Default", "Properties", "Selects the general context menu of the control"));
			designerActionItemCollection.Add(new DesignerActionPropertyItem("ContextMenuStripItems", "Items", "Properties", "Selects context menu for items"));
			designerActionItemCollection.Add(new DesignerActionPropertyItem("ContextMenuStripColumns", "Columns", "Properties", "Selects context menu for column headers"));
			return designerActionItemCollection;
		}

		/// <summary>
		///   Show BetterListViewColumnHeaderCollection collection editor.
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public void InvokeColumnsDialog() {
			EditorServiceContext.EditValue(this.designer, base.Component, "Columns");
		}

		/// <summary>
		///   Show BetterListViewItemCollection collection editor.
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public void InvokeItemsDialog() {
			EditorServiceContext.EditValue(this.designer, base.Component, "Items");
		}

		/// <summary>
		///   Show BetterListViewGroupCollection collection editor.
		/// </summary>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		public void InvokeGroupsDialog() {
			EditorServiceContext.EditValue(this.designer, base.Component, "Groups");
		}
	}
}
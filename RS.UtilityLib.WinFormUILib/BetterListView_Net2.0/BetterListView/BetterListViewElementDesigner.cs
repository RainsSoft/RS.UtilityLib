using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Custom component designer for BetterListViewElement.
	/// </summary>
	internal class BetterListViewElementDesigner : ComponentDesigner
	{
		private readonly bool generateMemberDefault;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementDesigner" /> class.
		/// </summary>
		public BetterListViewElementDesigner()
			: this(generateMemberDefault: true) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementDesigner" /> class.
		/// </summary>
		/// <param name="generateMemberDefault">The GenerateMember property should be set to <c>true</c> by default.</param>
		protected BetterListViewElementDesigner(bool generateMemberDefault) {
			this.generateMemberDefault = generateMemberDefault;
		}

		/// <summary>
		///   Prepares the designer to view, edit, and design the specified component.
		/// </summary>
		/// <param name="component">The component for this designer.</param>
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if (this.GetService(typeof(IComponentChangeService)) is IComponentChangeService componentChangeService) {
				componentChangeService.ComponentRename += ComponentChangeServiceComponentRename;
			}
		}

		/// <summary>
		///   Initializes a newly created component.
		/// </summary>
		/// <param name="defaultValues">A name/value dictionary of default values to apply to properties. May be null if no default values are specified.</param>
		public override void InitializeNewComponent(IDictionary defaultValues) {
			if (!this.generateMemberDefault && this.GetService(typeof(IExtenderListService)) is IExtenderListService extenderListService) {
				IExtenderProvider[] extenderProviders = extenderListService.GetExtenderProviders();
				IExtenderProvider[] array = extenderProviders;
				foreach (IExtenderProvider extenderProvider in array) {
					if (extenderProvider.GetType().FullName.Equals("System.ComponentModel.Design.Serialization.CodeDomDesignerLoader+ModifiersExtenderProvider", StringComparison.Ordinal)) {
						extenderProvider.GetType().GetMethod("SetGenerateMember", BindingFlags.Instance | BindingFlags.Public)?.Invoke(extenderProvider, new object[2] { base.Component, false });
						break;
					}
				}
			}
			base.InitializeNewComponent(defaultValues);
			BetterListViewElementBase betterListViewElementBase = (BetterListViewElementBase)base.Component;
			if (betterListViewElementBase.Site != null) {
				betterListViewElementBase.Name = betterListViewElementBase.Site.Name;
			}
		}

		/// <summary>
		///   Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Design.ComponentDesigner" /> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing) {
			if (this.GetService(typeof(IComponentChangeService)) is IComponentChangeService componentChangeService) {
				componentChangeService.ComponentRename -= ComponentChangeServiceComponentRename;
			}
		}

		private void ComponentChangeServiceComponentRename(object sender, ComponentRenameEventArgs eventArgs) {
			if (eventArgs.Component == base.Component) {
				((BetterListViewElementBase)base.Component).Name = eventArgs.NewName;
			}
		}
	}
}
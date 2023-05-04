using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Contextual information about the Better ListView component.
	/// </summary>
	internal sealed class EditorServiceContext : IWindowsFormsEditorService, ITypeDescriptorContext, IServiceProvider
	{
		private IComponentChangeService componentChangeService;

		private readonly ComponentDesigner designer;

		private readonly PropertyDescriptor targetPropertyDescriptor;

		IContainer ITypeDescriptorContext.Container {
			get {
				if (this.designer.Component.Site != null) {
					return this.designer.Component.Site.Container;
				}
				return null;
			}
		}

		object ITypeDescriptorContext.Instance => this.designer.Component;

		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor => this.targetPropertyDescriptor;

		private IComponentChangeService ChangeService {
			get {
				if (this.componentChangeService == null) {
					this.componentChangeService = (IComponentChangeService)((IServiceProvider)this).GetService(typeof(IComponentChangeService));
				}
				return this.componentChangeService;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.EditorServiceContext" /> class.
		/// </summary>
		/// <param name="designer">ComponentDesigner instance</param>
		public EditorServiceContext(ComponentDesigner designer) {
			Checks.CheckNotNull(designer, "designer");
			this.designer = designer;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.EditorServiceContext" /> class.
		/// </summary>
		/// <param name="designer">ComponentDesigner instance</param>
		/// <param name="propertyDescriptor">PropertyDescriptor instance for the current property</param>
		public EditorServiceContext(ComponentDesigner designer, PropertyDescriptor propertyDescriptor) {
			Checks.CheckNotNull(designer, "designer");
			Checks.CheckNotNull(propertyDescriptor, "propertyDescriptor");
			this.designer = designer;
			this.targetPropertyDescriptor = propertyDescriptor;
			if (propertyDescriptor == null) {
				propertyDescriptor = TypeDescriptor.GetDefaultProperty(designer.Component);
				if (propertyDescriptor != null && typeof(ICollection).IsAssignableFrom(propertyDescriptor.PropertyType)) {
					this.targetPropertyDescriptor = propertyDescriptor;
				}
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.EditorServiceContext" /> class.
		/// </summary>
		/// <param name="designer">ComponentDesigner instance.</param>
		/// <param name="propertyDescriptor">PropertyDescriptor instance for the current property.</param>
		/// <param name="newVerbText">New verb text.</param>
		public EditorServiceContext(ComponentDesigner designer, PropertyDescriptor propertyDescriptor, string newVerbText)
			: this(designer, propertyDescriptor) {
			Checks.CheckNotNull(designer, "designer");
			Checks.CheckNotNull(propertyDescriptor, "propertyDescriptor");
			Checks.CheckString(newVerbText, "newVerbText");
			this.designer.Verbs.Add(new DesignerVerb(newVerbText, OnEditItems));
		}

		/// <summary>
		///   Edit property value.
		/// </summary>
		/// <param name="designer">ComponentDesigner instance</param>
		/// <param name="objectToChange">object to be edited</param>
		/// <param name="propertyName">property name</param>
		/// <returns>edited value</returns>
		public static object EditValue(ComponentDesigner designer, object objectToChange, string propertyName) {
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectToChange)[propertyName];
			EditorServiceContext editorServiceContext = new EditorServiceContext(designer, propertyDescriptor);
			UITypeEditor uITypeEditor = propertyDescriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			object value = propertyDescriptor.GetValue(objectToChange);
			object obj = uITypeEditor.EditValue(editorServiceContext, editorServiceContext, value);
			if (obj != value) {
				try {
					propertyDescriptor.SetValue(objectToChange, obj);
					return obj;
				}
				catch (CheckoutException) {
					return obj;
				}
			}
			return obj;
		}

		private void OnEditItems(object sender, EventArgs eventArgs) {
			object value = this.targetPropertyDescriptor.GetValue(this.designer.Component);
			if (value != null) {
				(TypeDescriptor.GetEditor(value, typeof(UITypeEditor)) as CollectionEditor)?.EditValue(this, this, value);
			}
		}

		/// <summary>
		///   Closes any previously opened drop down control area.
		/// </summary>
		void IWindowsFormsEditorService.CloseDropDown() {
		}

		/// <summary>
		///   Displays the specified control in a drop down area below a value field of the property grid that provides this service.
		/// </summary>
		/// <param name="control">The drop down list <see cref="T:System.Windows.Forms.Control" /> to open. 
		/// </param>
		void IWindowsFormsEditorService.DropDownControl(Control control) {
		}

		/// <summary>
		///   Shows the specified <see cref="T:System.Windows.Forms.Form" />.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Windows.Forms.DialogResult" /> indicating the result code returned by the <see cref="T:System.Windows.Forms.Form" />.
		/// </returns>
		/// <param name="dialog">The <see cref="T:System.Windows.Forms.Form" /> to display. 
		/// </param>
		DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog) {
			return ((IUIService)((IServiceProvider)this).GetService(typeof(IUIService)))?.ShowDialog(dialog) ?? dialog.ShowDialog(this.designer.Component as IWin32Window);
		}

		/// <summary>
		///   Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanged" /> event.
		/// </summary>
		void ITypeDescriptorContext.OnComponentChanged() {
			this.ChangeService.OnComponentChanged(this.designer.Component, this.targetPropertyDescriptor, null, null);
		}

		/// <summary>
		///   Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanging" /> event.
		/// </summary>
		/// <returns>
		///   true if this object can be changed; otherwise, false.
		/// </returns>
		bool ITypeDescriptorContext.OnComponentChanging() {
			try {
				this.ChangeService.OnComponentChanging(this.designer.Component, this.targetPropertyDescriptor);
			}
			catch (CheckoutException ex) {
				if (ex != CheckoutException.Canceled) {
					throw;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		///   Gets the service object of the specified type.
		/// </summary>
		/// <returns>
		///   A service object of type <paramref name="serviceType" />.
		///   -or- 
		///   null if there is no service object of type <paramref name="serviceType" />.
		/// </returns>
		/// <param name="serviceType">An object that specifies the type of service object to get. 
		/// </param>
		/// <filterpriority>2</filterpriority>
		object IServiceProvider.GetService(Type serviceType) {
			if (serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(IWindowsFormsEditorService)) {
				return this;
			}
			if (this.designer.Component.Site != null) {
				return this.designer.Component.Site.GetService(serviceType);
			}
			return null;
		}
	}
}
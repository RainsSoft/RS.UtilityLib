namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type of element property.
	/// </summary>
	internal enum BetterListViewElementPropertyType
	{
		/// <summary>
		///   property affects look of the control
		/// </summary>
		Appearance = 0,
		/// <summary>
		///   property affects look of the control (invoked by change in check state of the element)
		/// </summary>
		Check = 1,
		/// <summary>
		///   property affects setup of the control elements by its collapse
		/// </summary>
		Collapse = 2,
		/// <summary>
		///   property affects bound data source
		/// </summary>
		DataBinding = 3,
		/// <summary>
		///   property affects setup of the control elements by its expansion
		/// </summary>
		Expand = 4,
		/// <summary>
		///   property affects setup of the control by changing item grouping
		/// </summary>
		Grouping = 5,
		/// <summary>
		///   property affects whether element is selectable
		/// </summary>
		Selectability = 6,
		/// <summary>
		///   property affects element selection
		/// </summary>
		Selection = 7,
		/// <summary>
		///   property affects layout of the control
		/// </summary>
		Layout = 8,
		/// <summary>
		///   property affects layout of the control (invoked by change of element image)
		/// </summary>
		LayoutImage = 9,
		/// <summary>
		///   property affects layout of the control (invoked by change of element text)
		/// </summary>
		LayoutText = 10,
		/// <summary>
		///   property affects layout elements setup in the control
		/// </summary>
		LayoutSetup = 11,
		/// <summary>
		///   property affects item sorting
		/// </summary>
		Sorting = 12
	}
}
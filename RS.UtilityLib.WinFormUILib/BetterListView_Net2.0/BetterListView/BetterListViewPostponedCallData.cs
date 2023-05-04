using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	/// Holds information about a postponed method.
	/// </summary>
	internal struct BetterListViewPostponedCallData
	{
		private readonly Delegate method;

		private readonly object[] parameters;

		/// <summary>
		/// The postponed method.
		/// </summary>
		public Delegate Method => this.method;

		/// <summary>
		/// Parameters of the postponed method.
		/// </summary>
		public object[] Parameters => this.parameters;

		public BetterListViewPostponedCallData(Delegate method, object[] parameters) {
			Checks.CheckNotNull(method, "method");
			Checks.CheckNotNull(parameters, "parameters");
			this.method = method;
			this.parameters = parameters;
		}
	}
}
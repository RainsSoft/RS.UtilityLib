using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common conversion routines.
	/// </summary>
	internal static class Conversions
	{
		/// <summary>
		///   base DateTime used for timestamp conversion
		/// </summary>
		public static readonly DateTime DateTimeBase = new DateTime(1970, 1, 1);

		public static long TimestampFromDateTime(DateTime dateTime) {
			Checks.CheckTrue(dateTime >= Conversions.DateTimeBase, "dateTime >= DateTimeBase");
			return (long)Math.Floor(dateTime.ToUniversalTime().Subtract(Conversions.DateTimeBase).TotalSeconds);
		}

		public static DateTime TimestampToDateTime(long timestamp) {
			Checks.CheckTrue(timestamp > 0, "timestamp > 0");
			return Conversions.DateTimeBase.Add(new TimeSpan(timestamp * 10000000)).ToLocalTime();
		}
	}
}
namespace ComponentOwl.BetterListView
{

	internal enum ActivationResponse
	{
		Ok = 0,
		SerialNumberNotFound = 1,
		ActivationLimit = 2,
		InvalidData = 3,
		DatabaseError = 4,
		ServerOther = 5,
		NoInternetConnection = 6,
		LocalOther = 7
	}
}
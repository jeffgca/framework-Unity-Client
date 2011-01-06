// Copyright 2010 OverInteractive Media Inc. All rights reserved.

/// <summary>
/// A basic class for storing API data.
/// </summary>
static class drAPI
{
	/// <summary>
	/// A structure to hold information about an API call. 
	/// </summary>
	public struct API
	{
		public readonly int          version;
		public readonly string       path;
		public readonly drWWW.Method method;
		public readonly bool		    sign; 
		public readonly bool			sendToken;

		/// <summary>
		/// Constructs a new API struct.
		/// </summary>
		/// <param name="version">The version number.</param>
		/// <param name="path">The path.</param>
		/// <param name="method">The request type.</param>
		/// <param name="sign">Does the call require a sha1 signature</param> 
		/// <param name="sendToken">Does the call require a request token</param>
		public API (int version, string path, drWWW.Method method, bool sign, bool sendToken)
		{
			this.version 		= version;
			this.path    		= path;
			this.method  		= method;
			this.sign 	   		= sign; 
			this.sendToken		= sendToken;
		}
	}
	
	// Counters
	public static readonly API counterCurrent       = new API(1, "counter/cur",            drWWW.Method.GET,	false,	false);
	public static readonly API counterIncrement     = new API(1, "counter/inc",            drWWW.Method.POST,	true,		true);
	public static readonly API counterDecrement     = new API(1, "counter/dec",            drWWW.Method.POST,	true,		true);
	
	// Data Store
	public static readonly API dataGet              = new API(1, "data/get",               drWWW.Method.GET,	false,	false);
	public static readonly API dataSet              = new API(1, "data/set",               drWWW.Method.POST,	true,		true);	
	public static readonly API dataUnset            = new API(1, "data/unset",             drWWW.Method.POST,	true,		true);
	public static readonly API dataReplace          = new API(1, "data/replace-with",      drWWW.Method.POST,	true, 	true);
	
	// Leaderboards
	public static readonly API leaderboardList      = new API(1, "leaderboard/list",       drWWW.Method.GET,	false,	false);
	public static readonly API leaderboardPostScore = new API(1, "leaderboard/post-score", drWWW.Method.POST,	true,		true);
	public static readonly API leaderboardGet       = new API(1, "leaderboard/get",        drWWW.Method.GET,	false,	false);
	
	// Wallets
	public static readonly API walletBalance        = new API(1, "wallet/balance",         drWWW.Method.GET,	false,	false);
	public static readonly API walletCredit         = new API(1, "wallet/credit",          drWWW.Method.POST,	true,		true);
	public static readonly API walletDebit          = new API(1, "wallet/debit",           drWWW.Method.POST,	true,		true);
	
	// Miscellaneous
	public static readonly API tokenGenerate        = new API(1, "token/generate",         drWWW.Method.GET,	false,	false);
}

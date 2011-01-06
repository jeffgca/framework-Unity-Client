// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functionality for tokens.
/// </summary>
class drTokens
{
	static Queue<string> tokens = new Queue<string>();
	
	public static bool hasTokens {
		get { return tokens.Count != 0; }
	}
	
	drTokens () {}
	
	/// <summary>
	/// Fetches tokens.
	/// </summary>
	public static Coroutine FetchTokens ()
	{
		drWWW www = new drWWW(drAPI.tokenGenerate);
		www.AddField("amount", 25);
		
		www.OnSuccess += delegate {
			ArrayList result = www.result as ArrayList;
			
			foreach (string token in result) {
				tokens.Enqueue(token);
			}
			
			drDebug.Log("Fetched tokens");
		};
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error fetching tokens: " + errorMessage);
		};
		
		return www.Fetch();
	}
	
	public static string GetToken ()
	{
		if (tokens.Count == 0) {
			return null;
		}
		
		return tokens.Dequeue();
	}
}
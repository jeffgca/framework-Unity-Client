// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functionality for high score leaderboards.
/// </summary>
public class drLeaderboards
{
	/// <summary>
	/// A basic class for storing leaderboard information.
	/// </summary>
	public class Leaderboard
	{
		/// <summary>
		/// The leaderboard's name.
		/// </summary>
		public readonly string name;
		
		SortedDictionary<int, Entry> _entries = new SortedDictionary<int, Entry>();
		/// <summary>
		/// The entries in the leaderboard sorted by rank.
		/// </summary>
		public SortedDictionary<int, Entry> entries {
			get { return _entries; }
			private set { _entries = value; }
		}
		
		/// <summary>
		/// Initializes a new Leaderboard instance.
		/// </summary>
		/// <param name="name">The name of the leaderboard.</param>
		internal Leaderboard (string name)
		{
			this.name = name;
		}
		
		/// <summary>
		/// Adds a variable number of entries to the leaderboard.
		/// </summary>
		/// <param name="startRank">The rank at which the entries start.</param>
		/// <param name="lbEntries">The entries to add.</param>
		internal void AddEntries (int startRank, params Entry[] lbEntries)
		{
			int rank = startRank;
			
			foreach (Entry entry in lbEntries) {
				if (entries.ContainsKey(rank)) {
					entries[rank] = entry;
				} else {
					entries.Add(rank, entry);
				}
				
				rank++;
			}
			
		}
	}
	
	/// <summary>
	/// A basic class for storing leaderboard entry data.
	/// </summary>
	public class Entry
	{
		/// <summary>
		/// The ID of the user who submitted the score.
		/// </summary>
		public readonly int userId;
		
		/// <summary>
		/// The submitted score.
		/// </summary>
		public readonly int score;
		
		/// <summary>
		/// The time at which the score was submitted.
		/// </summary>
		public readonly DateTime time;
		
		/// <summary>
		/// Arbitrary data stored with the score.
		/// </summary>
		public readonly string[] values;
		
		/// <summary>
		/// Initializes a new Entry instance.
		/// </summary>
		/// <param name="data">The entry data.</param>
		internal Entry (Hashtable data)
		{
			int.TryParse(data["id"].ToString(), out userId);
			int.TryParse(data["score"].ToString(), out score);
			time = drUtil.ConvertFromUnixTimestamp((double)data["time"]);
			
			Hashtable valuesData = data["data"] as Hashtable;
			List<string> values = new List<string>();
			
			foreach (string val in valuesData.Values) {
				values.Add(val);
			}
			
			this.values = values.ToArray();
		}
		
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="drLeaderboards.Entry"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="drLeaderboards.Entry"/>.</returns>
		public override string ToString ()
		{
			string str = "user ID: " + userId + "; score: " + score + "; time: " + time + "; values: ";
			
			foreach (string value in values) {
				str += value + " ";
			}
			
			return str;
		}
	}
	
	static string[] _availableLeaderboards;
	/// <summary>
	/// Names of available leaderboards that have been set up.
	/// </summary>
	public static string[] availableLeaderboards {
		get { return _availableLeaderboards; }
		private set { _availableLeaderboards = value; }
	}
	
	/// <summary>
	/// Leaderboards that have been fetched from the server.
	/// </summary>
	static List<Leaderboard> fetchedLeaderboards = new List<Leaderboard>();
	
	drLeaderboards () {}
	
	/// <summary>
	/// Fetches the names of the available leaderboards, saving them in the drLeaderboards.available array.
	/// </summary>
	public static Coroutine FetchAvailableLeaderboards ()
	{
		return FetchAvailableLeaderboards(null, null);
	}
	
	/// <summary>
	/// Fetches the names of the available leaderboards, saving them in the drLeaderboards.available array.
	/// </summary>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine FetchAvailableLeaderboards (dimeRocker.SuccessHandler success)
	{
		return FetchAvailableLeaderboards(success, null);
	}
	
	/// <summary>
	/// Fetches the names of the available leaderboards, saving them in the drLeaderboards.available array.
	/// </summary>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine FetchAvailableLeaderboards (dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = new drWWW(drAPI.leaderboardList);
		
		www.OnSuccess += delegate {
			ArrayList result = www.result as ArrayList;
			
			if (result.Count == 0) {
				availableLeaderboards = new string[0];
				drDebug.LogWarning("No leaderboards have been set up");
		   		return;
			}
			
			availableLeaderboards = new string[result.Count];
			
			for (int i = 0; i < result.Count; i++) {
				availableLeaderboards[i] = result[i].ToString();
			}
			
			drDebug.Log("Fetched leaderboards available");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error listing available leaderboards: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Submits a score to the leaderboard.
	/// </summary>
	/// <param name="leaderboardName">The leaderboard to post the score to.</param>
	/// <param name="score">The user's score.</param>
	public static Coroutine PostScore (string leaderboardName, int score)
	{
		return PostScore(leaderboardName, score, null, null, null);
	}
	
	/// <summary>
	/// Submits a score to the leaderboard.
	/// </summary>
	/// <param name="leaderboardName">The leaderboard to post the score to.</param>
	/// <param name="score">The user's score.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine PostScore (string leaderboardName, int score, dimeRocker.SuccessHandler success)
	{
		return PostScore(leaderboardName, score, null, success, null);
	}
	
	/// <summary>
	/// Submits a score to the leaderboard.
	/// </summary>
	/// <param name="leaderboardName">The leaderboard to post the score to.</param>
	/// <param name="score">The user's score.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine PostScore (string leaderboardName, int score, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return PostScore(leaderboardName, score, null, success, error);
	}
	
	/// <summary>
	/// Submits a score to the leaderboard.
	/// </summary>
	/// <param name="leaderboardName">The leaderboard to post the score to.</param>
	/// <param name="score">The user's score.</param>
	/// <param name="values">Arbitrary values to attach to the score (maximum three).</param>
	public static Coroutine PostScore (string leaderboardName, int score, string[] values)
	{
		return PostScore(leaderboardName, score, values, null, null);
	}
	
	/// <summary>
	/// Submits a score to the leaderboard.
	/// </summary>
	/// <param name="leaderboardName">The leaderboard to post the score to.</param>
	/// <param name="score">The user's score.</param>
	/// <param name="values">Arbitrary values to attach to the score (maximum three).</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine PostScore (string leaderboardName, int score, string[] values, dimeRocker.SuccessHandler success)
	{
		return PostScore(leaderboardName, score, values, success, null);
	}
	
	/// <summary>
	/// Submits a score to the leaderboard.
	/// </summary>
	/// <param name="leaderboardName">The leaderboard to post the score to.</param>
	/// <param name="score">The user's score.</param>
	/// <param name="values">Arbitrary values to attach to the score (maximum three).</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine PostScore (string leaderboardName, int score, string[] values, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = new drWWW(drAPI.leaderboardPostScore);
		www.AddField("name", leaderboardName);
		www.AddField("score", score);
		
		if (values != null) {
			for (int i = 0; i < Mathf.Max(values.Length, 3); i++) {
				// Skip empty or null values
				if (values[i] == null || values[i] == "") {
					continue;
				}
				
				www.AddField("v" + (i + 1), values[i]);
			}
		}
		
		www.OnSuccess += delegate {
			int rank = 0;
			int.TryParse(www.result.ToString(), out rank);
			
			if (rank == 0) {
				drDebug.LogWarning("User did not rank");
		   		return;
			}
			
			drDebug.Log("Score posted to leaderboard " + leaderboardName + " with rank " + rank);
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error posting score to leaderboard " + leaderboardName + ": " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Fetches the leaderboard scores.
	/// </summary>
	/// <param name="leaderboardName">The name of the leaderboard.</param>
	public static Coroutine FetchEntries (string leaderboardName)
	{
		return FetchLeaderboardEntries(leaderboardName, null, null, null, null);
	}
	
	/// <summary>
	/// Fetches the leaderboard scores.
	/// </summary>
	/// <param name="leaderboardName">The name of the leaderboard.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine FetchEntries (string leaderboardName, dimeRocker.SuccessHandler success)
	{
		return FetchLeaderboardEntries(leaderboardName, null, null, success, null);
	}
	
	/// <summary>
	/// Fetches the leaderboard scores.
	/// </summary>
	/// <param name="leaderboardName">The name of the leaderboard.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine FetchEntries (string leaderboardName, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return FetchLeaderboardEntries(leaderboardName, null, null, success, error);
	}
	
	/// <summary>
	/// Fetches the leaderboard scores.
	/// </summary>
	/// <param name="leaderboardName">The name of the leaderboard.</param>
	/// <param name="offset">The entry to start from. A negative number will return from the end instead. Default is 0.</param>
	/// <param name="length">The number of entries to return. Default is 10.</param>
	public static Coroutine FetchEntries (string leaderboardName, int offset, int length)
	{
		return FetchLeaderboardEntries(leaderboardName, offset, length, null, null);
	}
	
	/// <summary>
	/// Fetches the leaderboard scores.
	/// </summary>
	/// <param name="leaderboardName">The name of the leaderboard.</param>
	/// <param name="offset">The entry to start from. A negative number will return from the end instead. Default is 0.</param>
	/// <param name="length">The number of entries to return. Default is 10.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine FetchEntries (string leaderboardName, int offset, int length, dimeRocker.SuccessHandler success)
	{
		return FetchLeaderboardEntries(leaderboardName, offset, length, success, null);
	}
	
	/// <summary>
	/// Fetches the leaderboard scores.
	/// </summary>
	/// <param name="leaderboardName">The name of the leaderboard.</param>
	/// <param name="offset">The entry to start from. A negative number will return from the end instead. Default is 0.</param>
	/// <param name="length">The number of entries to return. Default is 10.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine FetchEntries (string leaderboardName, int offset, int length, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return FetchLeaderboardEntries(leaderboardName, offset, length, success, error);
	}
	
	/// <summary>
	/// Fetches the leaderboard scores.
	/// </summary>
	/// <param name="leaderboardName">The name of the leaderboard.</param>
	/// <param name="offset">The entry to start from. A negative number will return from the end instead. Default is 0.</param>
	/// <param name="length">The number of entries to return. Default is 10.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	static Coroutine FetchLeaderboardEntries (string leaderboardName, int? offset, int? length, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = new drWWW(drAPI.leaderboardGet);
		www.AddField("name", leaderboardName);
		
		if (offset.HasValue) {
			www.AddField("offset", offset.Value);
		}
		
		if (length.HasValue) {
			www.AddField("length", length.Value);
		}
		
		www.OnSuccess += delegate {
			ArrayList result = www.result as ArrayList;
			List<Entry> entries = new List<Entry>();
			
			foreach (Hashtable entryData in result) {
				Entry entry = new Entry(entryData);
				entries.Add(entry);
			}
			
			Leaderboard lb = GetLeaderboard(leaderboardName);
			
			if (lb == null) {
				lb = new Leaderboard(leaderboardName);
				fetchedLeaderboards.Add(lb);
			}
			
			lb.AddEntries(offset.HasValue ? offset.Value + 1 : 1, entries.ToArray());
			drDebug.Log("Scores fetched from leaderboard " + leaderboardName);
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error fetching scores from leaderboard " + leaderboardName + ": " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Gets the leaderboard with the given name.
	/// </summary>
	/// <param name="name">The name of the leaderboard.</param>
	/// <returns>The leaderboard.</returns>
	public static Leaderboard GetLeaderboard (string name)
	{
		foreach (Leaderboard leaderboard in fetchedLeaderboards) {
			if (leaderboard.name == name) {
				return leaderboard;
			}
		}
		
		return null;
	}
}
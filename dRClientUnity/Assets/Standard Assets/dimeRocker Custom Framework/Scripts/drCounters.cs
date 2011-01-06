// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using Procurios.Public; // JSON parser
using UnityEngine;

/// <summary>
/// Functionality for counters.
/// </summary>
public class drCounters
{
	/// <summary>
	/// A basic class for storing counter information.
	/// </summary>
	public class Counter
	{
		/// <summary>
		/// The name of the counter.
		/// </summary>
		public readonly string name;
		
		int _value;
		/// <summary>
		/// The counter's value.
		/// </summary>
		public int value {
			get { return _value; }
			internal set { _value = value; }
		}
		
		/// <summary>
		/// The user's ID.
		/// </summary>
		public readonly int userId;
		
		/// <summary>
		/// Initializes a new instance of the Counter class.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		internal Counter (string name)
		{
			this.name = name;
		}
		
		/// <summary>
		/// Initializes a new instance of the Counter class.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="userId">The user's ID.</param>
		internal Counter (string name, int userId)
		{
			this.name = name;
			this.userId = userId;
		}
		
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="drCounters.Counter"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="drCounters.Counter"/>.</returns>
		public override string ToString ()
		{
			return "counter: " + name + "; value: " + value + "; user ID: " + userId;
		}
	}
	
	/// <summary>
	/// Counters that have been downloaded from the server.
	/// </summary>
	static List<Counter> fetchedCounters = new List<Counter>();
	
	drCounters () {}
	
	/// <summary>
	/// Fetches the current Counter value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	public static Coroutine FetchCurrent (string name)
	{
		return FetchCurrent(name, null, null, null);
	}
	
	/// <summary>
	/// Fetches the current Counter value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="success">Callback triggers on successful fetch.</param>
	public static Coroutine FetchCurrent (string name, dimeRocker.SuccessHandler success)
	{
		return FetchCurrent(name, null, success, null);
	}
	
	/// <summary>
	/// Fetches the current Counter value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="success">Callback triggers on successful fetch.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine FetchCurrent (string name, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return FetchCurrent(name, null, success, error);
	}
	
	/// <summary>
	/// Fetches the current Counter value for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="userIds">The user's IDs.</param>
	public static Coroutine FetchCurrent (string name, int[] userIds)
	{
		return FetchCurrent(name, userIds, null, null);
	}
	
	/// <summary>
	/// Fetches the current Counter value for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="userIds">The user's IDs.</param>
	/// <param name="success">Callback triggers on successful fetch.</param>
	public static Coroutine FetchCurrent (string name, int[] userIds, dimeRocker.SuccessHandler success)
	{
		return FetchCurrent(name, userIds, success, null);
	}
	
	/// <summary>
	/// Fetches the current Counter value for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="userIds">The user's IDs.</param>
	/// <param name="success">Callback triggers on successful fetch.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine FetchCurrent (string name, int[] userIds, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.counterCurrent, name, userIds);
		
		www.OnSuccess += delegate {
			Hashtable result = www.result as Hashtable;
			
			if (result.Count == 0) {
				drDebug.LogWarning("No counters exist with the specified name and users");
		   		return;
			}
			
			StoreReturn(name, result, userIds);
			drDebug.Log("Fetched current counters");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error fetching current counters: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Increments the counter's value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to increment.</param>
	public static Coroutine Increment (string name, int amount)
	{
		return Increment(name, amount, null, null, null);
	}
	
	/// <summary>
	/// Increments the counter's value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	public static Coroutine Increment (string name, int amount, dimeRocker.SuccessHandler success)
	{
		return Increment(name, amount, null, success, null);
	}
	
	/// <summary>
	/// Increments the counter's value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Increment (string name, int amount, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return Increment(name, amount, null, success, error);
	}
	
	/// <summary>
	/// Increments the Counter value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="userIds">The user's IDs.</param>
	public static Coroutine Increment (string name, int amount, int[] userIds)
	{
		return Increment(name, amount, userIds, null, null);
	}
	
	/// <summary>
	/// Increments the Counter value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="userIds">The user's IDs.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	public static Coroutine Increment (string name, int amount, int[] userIds, dimeRocker.SuccessHandler success)
	{
		return Increment(name, amount, userIds, success, null);
	}
	
	/// <summary>
	/// Increments the Counter value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="userIds">The user's IDs.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Increment (string name, int amount, int[] userIds, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.counterIncrement, name, userIds);
		www.AddField("amount", amount);
		
		www.OnSuccess += delegate {
			Hashtable result = www.result as Hashtable;
			
			if (result.Count == 0) {
				drDebug.LogWarning("No counters exist with the specified name and users");
		   		return;
			}
			
			StoreReturn(name, result, userIds);
			drDebug.Log("Incremented counters");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error incrementing counters: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Decrements the counter's value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to Decrement.</param>
	public static Coroutine Decrement (string name, int amount)
	{
		return Decrement(name, amount, null, null, null);
	}
	
	/// <summary>
	/// Decrements the counter's value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to Decrement.</param>
	/// <param name="success">Callback triggers on successful Decrement.</param>
	public static Coroutine Decrement (string name, int amount, dimeRocker.SuccessHandler success)
	{
		return Decrement(name, amount, null, success, null);
	}
	
	/// <summary>
	/// Decrements the counter's value.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to Decrement.</param>
	/// <param name="success">Callback triggers on successful Decrement.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Decrement (string name, int amount, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return Decrement(name, amount, null, success, error);
	}
	
	/// <summary>
	/// Decrements the Counter value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to Decrement.</param>
	/// <param name="userIds">The user's IDs.</param>
	public static Coroutine Decrement (string name, int amount, int[] userIds)
	{
		return Decrement(name, amount, userIds, null, null);
	}
	
	/// <summary>
	/// Decrements the Counter value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to Decrement.</param>
	/// <param name="userIds">The user's IDs.</param>
	/// <param name="success">Callback triggers on successful Decrement.</param>
	public static Coroutine Decrement (string name, int amount, int[] userIds, dimeRocker.SuccessHandler success)
	{
		return Decrement(name, amount, userIds, success, null);
	}
	
	/// <summary>
	/// Decrements the Counter value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="amount">The amount to Decrement.</param>
	/// <param name="userIds">The user's IDs.</param>
	/// <param name="success">Callback triggers on successful Decrement.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Decrement (string name, int amount, int[] userIds, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.counterDecrement, name, userIds);
		www.AddField("amount", amount);
		
		www.OnSuccess += delegate {
			Hashtable result = www.result as Hashtable;
			
			if (result.Count == 0) {
				drDebug.LogWarning("No counters exist with the specified name and users");
		   		return;
			}
			
			StoreReturn(name, result, userIds);
			drDebug.Log("Decremented counters");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error decrementing counters: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Gets a counter with the given name.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <returns>The counters.</returns>
	public static Counter GetCounter (string name)
	{
		foreach (Counter counter in fetchedCounters) {
			if (counter.name == name) {
				return counter;
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Gets a counters with the given name for a specific user.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="userId">The user's ID.</param>
	/// <returns>The counter.</returns>
	public static Counter GetCounter (string name, int userId)
	{
		foreach (Counter counter in fetchedCounters) {
			if (counter.name == name && counter.userId == userId) {
				return counter;
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Stores the returned counter information.
	/// </summary>
	/// <param name="name">The name of the counter.</param>
	/// <param name="data">The data to store.</param>
	/// <param name="userIds">The user IDs.</param>
	static void StoreReturn (string name, Hashtable data, int[] userIds)
	{
		foreach (DictionaryEntry entry in data) {
			int userId = int.Parse(entry.Key.ToString());
			Counter existing = GetCounter(name, userId);
			
			if (existing == null) {
				existing = new Counter(name, userId);
				fetchedCounters.Add(existing);
			}
			
			existing.value = int.Parse(entry.Value.ToString());
		}
	}
	
	/// <summary>
	/// Creates a WWW object prepopulated with fields common to all calls.
	/// </summary>
	/// <param name="api">The call's API.</param>
	/// <param name="name">The name of the counter.</param>
	/// <param name="userIds">The user IDs.</param>
	/// <returns>The WWW object.</returns>
	static drWWW GetWWW (drAPI.API api, string name, int[] userIds)
	{
		drWWW www = new drWWW(api);
		www.AddField("name", name);
		
		if (userIds != null && userIds.Length > 0) {
			string userIdsJson = JSON.JsonEncode(userIds);
			www.AddField("uids", userIdsJson);
		}
		
		return www;
	}
}
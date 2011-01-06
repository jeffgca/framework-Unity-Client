// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using Procurios.Public; // JSON parser
using UnityEngine;

/// <summary>
/// Functionality for data store.
/// </summary>
/// <remarks>A common use for the data store is player preferences that persist between play sessions and computers.</remarks>
/// <example>
/// var data = new Hashtable() {
/// 	{ "health", 30 },
/// 	{ "location", "Vancouver" }
/// };
/// 
/// drDataStore.Set("playerinfo", data);
/// </example>
/// <example>
/// drDataStore.Fetch("playerinfo", data, delegate {
/// 	var data = drDataStore.GetData("playerinfo").data;
/// 	Debug.Log("Player Health: " + data["health"]);
/// });
/// </example>
public class drDataStore
{
	/// <summary>
	/// A basic struct for storing data.
	/// </summary>
	public class Data
	{
		/// <summary>
		/// The namespace.
		/// </summary>
		public readonly string ns;
		
		Hashtable _data = new Hashtable();
		/// <summary>
		/// The data.
		/// </summary>
		public Hashtable data {
			get { return _data; }
			internal set { _data = value; }
		}
		
		/// <summary>
		/// The user's ID.
		/// </summary>
		public readonly int userId;
		
		/// <summary>
		/// Initializes a new instance of the Data class.
		/// </summary>
		/// <param name="ns">The namespace.</param>
		public Data (string ns)
		{
			this.ns = ns;
		}
		
		/// <summary>
		/// Initializes a new instance of the Data class.
		/// </summary>
		/// <param name="ns">The namespace.</param>
		/// <param name="userId">The user's ID.</param>
		public Data (string ns, int userId)
		{
			this.ns = ns;
			this.userId = userId;
		}
	}
	
	/// <summary>
	/// Data that has been downloaded from the server.
	/// </summary>
	static List<Data> fetchedData = new List<Data>();
	
	drDataStore () {}
	
	/// <summary>
	/// Loads global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to load.</param>
	public static Coroutine Fetch (string ns, string[] keys)
	{
		return FetchData(ns, keys, null, null, null);
	}
	
	/// <summary>
	/// Loads global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to load.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine Fetch (string ns, string[] keys, dimeRocker.SuccessHandler success)
	{
		return FetchData(ns, keys, null, success, null);
	}
	
	/// <summary>
	/// Loads global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to load.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Fetch (string ns, string[] keys, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return FetchData(ns, keys, null, success, error);
	}
	
	/// <summary>
	/// Loads user-specific data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to load.</param>
	/// <param name="userId">The user's ID.</param>
	public static Coroutine Fetch (string ns, string[] keys, int userId)
	{
		return FetchData(ns, keys, userId, null, null);
	}
	
	/// <summary>
	/// Loads user-specific data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to load.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine Fetch (string ns, string[] keys, int userId, dimeRocker.SuccessHandler success)
	{
		return FetchData(ns, keys, userId, success, null);
	}
	
	/// <summary>
	/// Loads data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to load.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Fetch (string ns, string[] keys, int userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return FetchData(ns, keys, userId, success, error);
	}
	
	/// <summary>
	/// Loads data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to load.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	static Coroutine FetchData (string ns, string[] keys, int? userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.dataGet, ns, userId);
		
		if (keys != null && keys.Length > 0) {
			string keysJson = JSON.JsonEncode(keys);
			www.AddField("keys", keysJson);
		}
		
		www.OnSuccess += delegate {
			Hashtable result = www.result as Hashtable;
			
			if (result.Count == 0) {
				drDebug.LogWarning("No data exists in the data store to load");
		   		return;
			}
			
			StoreData(ns, result, userId);
			drDebug.Log("Loaded data");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error loading data: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Saves namespaced global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	public static Coroutine Set (string ns, Hashtable data)
	{
		return SetData(ns, data, null, null, null);
	}
	
	/// <summary>
	/// Saves namespaced global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine Set (string ns, Hashtable data, dimeRocker.SuccessHandler success)
	{
		return SetData(ns, data, null, success, null);
	}
	
	/// <summary>
	/// Saves namespaced global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Set (string ns, Hashtable data, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return SetData(ns, data, null, success, error);
	}
	
	/// <summary>
	/// Saves user-specific data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	public static Coroutine Set (string ns, Hashtable data, int userId)
	{
		return SetData(ns, data, userId, null, null);
	}
	
	/// <summary>
	/// Saves user-specific data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine Set (string ns, Hashtable data, int userId, dimeRocker.SuccessHandler success)
	{
		return SetData(ns, data, userId, success, null);
	}
	
	/// <summary>
	/// Saves data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Set (string ns, Hashtable data, int userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{		
		return SetData(ns, data, userId, success, error);
	}
	
	/// <summary>
	/// Saves data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	static Coroutine SetData (string ns, Hashtable data, int? userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{		
		drWWW www = GetWWW(drAPI.dataSet, ns, userId);
		
		string json = JSON.JsonEncode(data);
		www.AddField("data", json);
		
		Debug.Log("Data JSON: " + json);
		
		www.OnSuccess += delegate {
			int result = 0;
			int.TryParse(www.result.ToString(), out result);
			
			// This shouldn't happen, but the check is here just in case
			if (result != 1) {
				drDebug.LogWarning("No data saved");
				return;
			}
			
			StoreData(ns, data, userId);
			drDebug.Log("Data saved");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error saving data: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Unsets global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to unset</param>
	public static Coroutine Unset (string ns, string[] keys)
	{
		return UnsetData(ns, keys, null, null, null);
	}
	
	/// <summary>
	/// Unsets global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to unset</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine Unset (string ns, string[] keys, dimeRocker.SuccessHandler success)
	{
		return UnsetData(ns, keys, null, success, null);
	}
	
	/// <summary>
	/// Unsets global data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to unset</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Unset (string ns, string[] keys, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return UnsetData(ns, keys, null, success, error);
	}
	
	/// <summary>
	/// Unsets user-specific data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to unset</param>
	/// <param name="userId">The user's ID.</param>
	public static Coroutine Unset (string ns, string[] keys, int userId)
	{
		return UnsetData(ns, keys, userId, null, null);
	}
	
	/// <summary>
	/// Unsets user-specific data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to unset</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	public static Coroutine Unset (string ns, string[] keys, int userId, dimeRocker.SuccessHandler success)
	{
		return UnsetData(ns, keys, userId, success, null);
	}
	
	/// <summary>
	/// Unsets data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to unset.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Unset (string ns, string[] keys, int userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return UnsetData(ns, keys, userId, success, error);
	}
	
	/// <summary>
	/// Unsets data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="keys">Keys of the data to unset.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data save.</param>
	/// <param name="error">Callback triggers on error.</param>
	static Coroutine UnsetData (string ns, string[] keys, int? userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		// Specifying the keys to unset is mandatory; to replace the entire document use ReplaceWith
		if (keys == null || keys.Length == 0) {
			drDebug.LogWarning("Keys must be specified when unsetting data");
			return null;
		}
		
		drWWW www = GetWWW(drAPI.dataUnset, ns, userId);
		
		string keysJson = JSON.JsonEncode(new ArrayList(keys));
		www.AddField("keys", keysJson);
		
		www.OnSuccess += delegate {
			int result = 0;
			int.TryParse(www.result.ToString(), out result);
			
			// This shouldn't happen, but the check is here just in case
			if (result != 1) {
				drDebug.LogWarning("Data not unset");
				return;
			}
			
			Data existing;
			
			if (userId.HasValue) {
				existing = GetData(ns, userId.Value);
			} else {
				existing = GetData(ns);
			}
			
			foreach (string key in keys) {
				existing.data.Remove(key);
			}
			
			drDebug.Log("Data unset successfully");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error unsetting data: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Saves global data, replacing all existing data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	public static Coroutine Replace (string ns, Hashtable data)
	{
		return ReplaceData(ns, data, null, null, null);
	}
	
	/// <summary>
	/// Saves global data, replacing all existing data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="success">Callback triggers on successful data replacement.</param>
	public static Coroutine Replace (string ns, Hashtable data, dimeRocker.SuccessHandler success)
	{
		return ReplaceData(ns, data, null, success, null);
	}
	
	/// <summary>
	/// Saves global data, replacing all existing data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="success">Callback triggers on successful data replacement.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Replace (string ns, Hashtable data, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return ReplaceData(ns, data, null, success, error);
	}
	
	/// <summary>
	/// Saves user-specific data, replacing all existing user data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	public static Coroutine Replace (string ns, Hashtable data, int userId)
	{
		return ReplaceData(ns, data, userId, null, null);
	}
	
	/// <summary>
	/// Saves user-specific data, replacing all existing user data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data replacement.</param>
	public static Coroutine Replace (string ns, Hashtable data, int userId, dimeRocker.SuccessHandler success)
	{
		return ReplaceData(ns, data, userId, success, null);
	}
	
	/// <summary>
	/// Saves data, replacing existing data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data replacement.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Replace (string ns, Hashtable data, int userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return ReplaceData(ns, data, userId, success, error);
	}
	
	/// <summary>
	/// Saves data, replacing existing data.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to save.</param>
	/// <param name="userId">The user's ID.</param>
	/// <param name="success">Callback triggers on successful data replacement.</param>
	/// <param name="error">Callback triggers on error.</param>
	static Coroutine ReplaceData (string ns, Hashtable data, int? userId, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.dataReplace, ns, userId);
		
		string json = JSON.JsonEncode(data);
		www.AddField("data", json);
		
		www.OnSuccess += delegate {
			int result = 0;
			int.TryParse(www.result.ToString(), out result);
			
			// This shouldn't happen, but the check is here just in case
			if (result != 1) {
				drDebug.LogWarning("No data saved");
				return;
			}
			
			Data existing;
			
			if (userId.HasValue) {
				existing = GetData(ns, userId.Value);
			} else {
				existing = GetData(ns);
			}
			
			fetchedData.Remove(existing);
			StoreData(ns, data, userId);
			drDebug.Log("Data saved");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error saving data: " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Gets data for the given namespace.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <returns>The namespaced user-specific data.</returns>
	public static Data GetData (string ns)
	{
		foreach (Data data in fetchedData) {
			if (data.ns == ns) {
				return data;
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Gets data for the given namespace and user.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="userId">The ID of the user.</param>
	/// <returns>The namespaced user-specific data.</returns>
	public static Data GetData (string ns, int userId)
	{
		foreach (Data data in fetchedData) {
			if (data.ns == ns && data.userId == userId) {
				return data;
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Stores the data returned / saved to the server.
	/// </summary>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="data">The data to store.</param>
	/// <param name="userId">The user's ID.</param>
	static void StoreData (string ns, Hashtable data, int? userId)
	{
		Data existing = GetData(ns, userId.HasValue ? userId.Value : 0);
		
		if (existing == null) {
			if (userId.HasValue) {
				existing = new Data(ns, userId.Value);
			} else {
				existing = new Data(ns);
			}
			
			fetchedData.Add(existing);
		}
		
		foreach (DictionaryEntry entry in data) {
			if (existing.data.ContainsKey(entry.Key)) {
				existing.data[entry.Key] = entry.Value;
			} else {
				existing.data.Add(entry.Key, entry.Value);
			}
		}
	}
	
	/// <summary>
	/// Creates a WWW object prepopulated with fields common to all calls.
	/// </summary>
	/// <param name="api">The call's API.</param>
	/// <param name="ns">The namespace of the data.</param>
	/// <param name="userId">The user's ID.</param>
	/// <returns>The WWW object.</returns>
	static drWWW GetWWW (drAPI.API api, string ns, int? userId)
	{
		drWWW www = new drWWW(api);
		www.AddField("namespace", ns);
		
		if (userId.HasValue) {
			www.AddField("uid", userId.Value);
		}
		
		return www;
	}
}

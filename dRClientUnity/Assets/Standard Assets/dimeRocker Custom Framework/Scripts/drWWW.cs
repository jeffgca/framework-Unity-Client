// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Procurios.Public; // JSON parser
using UnityEngine;

/// <summary>
/// Constructs WWW classes for sending requests to the dimeRocker servers.
/// </summary>
class drWWW
{
	public enum Method { GET, POST }
	
	public event dimeRocker.SuccessHandler OnSuccess;
	public event dimeRocker.ErrorHandler   OnError;
	
	static List<WWW> inProgress = new List<WWW>();
	/// <summary>
	/// Returns true if some WWW requests are still processing.
	/// </summary>
	public static bool processing {
		get { return inProgress.Count != 0; }
	}
	
	object _result;
	public object result {
		get { return _result; }
		private set { _result = value; }
	}

	drAPI.API api;
	WWW www;
	SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();
	
	static Queue<string> tokens = new Queue<string>();
	
	/// <summary>
	/// Returns a hash that the server uses to ensure the data sent is valid.
	/// </summary>
	string signature {
		get {
			StringBuilder sig = new StringBuilder(dimeRocker.instance.secretKey);

			foreach (KeyValuePair<string, object> kvp in parameters) {
				if (kvp.Key.StartsWith("_") || kvp.Value == null) {
					continue;
				}
				
				sig.Append(kvp.Key);
				sig.Append('=');
				sig.Append(kvp.Value.ToString());
				sig.Append('&');
			}
			
			return drUtil.SHA1Hash(sig.ToString());
		}
	}
	
	/// <summary>
	/// Creates a new drWWW object.
	/// </summary>
	/// <param name="api">The API call to use.</param>
	internal drWWW (drAPI.API api) {
		this.api = api;
	}
    
	/// <summary>
	/// Adds a parameter.
	/// </summary>
	/// <param name="key">The parameter's key.</param>
	/// <param name="value">The value sent.</param>
    public void AddField (string key, object value)
    {
        parameters.Add(key, value);
    }
	
	/// <summary>
	/// Fetches information from the server.
	/// </summary>
	public Coroutine Fetch ()
	{
		return dimeRocker.RunCoroutine(FetchCoroutine());
	}
	
	/// <summary>
	/// Fetches information from the server.
	/// </summary>
	IEnumerator FetchCoroutine ()
	{
		if (Application.isEditor) {
			AddField("_test", drUtil.SHA1Hash(dimeRocker.instance.secretKey));
		}
				
		// Tokens
		if (api.sendToken) {
    		if (tokens.Count == 0) {
    			yield return FetchTokens();
    		}
    		
    		AddField("token", tokens.Dequeue());			
		}

		if (api.sign) {
			AddField("_sig", signature);
		}
				
		if (api.method == drWWW.Method.POST) { // POST
			WWWForm form = drUtil.BuildPOSTParametersForm(parameters);
			string url = BuildUrl(api.path);
			www = new WWW(url, form);
		} else { // GET
			string parametersString = drUtil.BuildGETParametersString(parameters);
			string url = BuildUrl(api.path, parametersString);
			www = new WWW(url);
		}
		
		inProgress.Add(www);
		yield return www;
		inProgress.Remove(www);
		
		try {
			result = ParseResponse(www);
			
			if (OnSuccess != null) {
				OnSuccess();
			}
		} catch (Exception e) {
			if (OnError != null) {
				OnError(e.Message);
			}
		}
	}
	
	/// <summary>
	/// Fetches tokens.
	/// </summary>
	Coroutine FetchTokens ()
	{
		return dimeRocker.RunCoroutine(FetchTokensCoroutine());
	}
	
	/// <summary>
	/// Fetches tokens.
	/// </summary>
	IEnumerator FetchTokensCoroutine ()
	{
		string url = BuildUrl(drAPI.tokenGenerate.path);
		WWWForm form = new WWWForm();
		form.AddField("amount", 25);
		
		if (Application.isEditor) {
			form.AddField("_test", drUtil.SHA1Hash(dimeRocker.instance.secretKey));
		}
		
		WWW www = new WWW(url, form);
		yield return www;
		object result = ParseResponse(www);
		
		foreach (string token in result as ArrayList) {
			tokens.Enqueue(token);
		}
	}
	
	/// <summary>
	/// Parses the JSON response from the server.
	/// </summary>
	/// <param name="www">The WWW request.</param>
	/// <returns>The response object.</returns>
	static object ParseResponse (WWW www)
	{
#if UNITY_2_6
		string json = www.data;
#else
		string json = www.text;
#endif
			
		if (dimeRocker.instance.verboseOutputInConsole) {
			drDebug.Log("URL: " + www.url);
			drDebug.Log("Length: " + json.Length);
			drDebug.Log("Data: " + json);
		}
		
		if (dimeRocker.instance.verboseOutputInDebugLog) {
			Debug.Log("URL: " + www.url);
			Debug.Log("Length: " + json.Length);
			Debug.Log("Data: " + json);

		}
		
		if (www.error != null) {
			throw new Exception("WWW fetch error: " + www.error);
		}
		
		// Process request:
		Hashtable container = JSON.JsonDecode(json) as Hashtable;

		// Container is null
    	if (container == null) {
    		if (json.Length == 0) {
    			throw new Exception("No data returned from server");
    		} else {
    			// probably a json parse error or container format is wrong
    			throw new Exception("Return container (JSON parse error)");
    		}		
    	}
    	    	
		// Container isn't a Hashtable
		if (container.GetType() != typeof(Hashtable)) {
			throw new Exception("Container is not a Hashtable");
    	}

    	// Container's result field is null
		object result = container["results"];
    	if (result == null) {
			throw new Exception("No result object");
    	}
		
		string errorMessage = container["errorMessage"].ToString();
		// Server returned content in the errorMessage field
		if (errorMessage != "") {
			throw new Exception(errorMessage);
		}
		
		return result;
	}
	
	/// <summary>
	/// Builds the URL.
	/// </summary>
	/// <param name="method">The method to use.</param>
	/// <returns>The URL.</returns>
	static string BuildUrl (string method)
	{
		return BuildUrl(method, "");
	}

	/// <summary>
	/// Builds the URL.
	/// </summary>
	/// <param name="method">The method to use.</param>
	/// <param name="parametersString">The parameter string to use to use.</param>
	/// <returns>The URL.</returns>
	static string BuildUrl (string method, string parametersString)
	{
		return dimeRocker.instance.apiUrl + method + parametersString;;
	}
	
	/// <summary>
	/// Calls a function on the web page.
	/// </summary>
	/// <param name="functionName">The name of the function to execute.</param>
	/// <param name="args">The arguments to send to the function.</param>
	public static void ExternalCall (string functionName, params object[] args)
	{
		ArrayList parameters = new ArrayList(args);
		parameters.Insert(0, functionName);
		
		// All function calls pass through a global "DR_clientCall" function
		Application.ExternalCall("DR_clientCall", parameters.ToArray());
	}
}

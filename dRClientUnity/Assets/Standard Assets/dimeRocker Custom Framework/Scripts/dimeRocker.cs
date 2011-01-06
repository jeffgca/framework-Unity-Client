// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The main dimeRocker class.
/// </summary>
public class dimeRocker : MonoBehaviour
{
	/// <summary>
	/// The version of the client.
	/// </summary>
	public static readonly Version version = new Version(0, 1, 4);
	
	/// <summary>
	/// A delegate for handling successful responses from the server.
	/// </summary>
	public delegate void SuccessHandler();
	
	/// <summary>
	/// A delegate for handling failed calls to the server.
	/// </summary>
	/// <param name="message">The error message.</param>
	public delegate void ErrorHandler(string message);
	
	#region Inspector Settings
	
	/// <summary>
	/// The URL to the API repository.
	/// </summary>
	public string apiUrl = "";
	
	/// <summary>
	/// The game's secret key.
	/// </summary>
	public string secretKey = "";
	
	/// <summary>
	/// Whether or not to allow the console to appear.
	/// </summary>
	public bool devMode = true;
	
	/// <summary>
	/// Hotkey to toggle the visibility of the console window.
	/// </summary>
	public KeyCode consoleToggleKey = KeyCode.BackQuote;
	
	/// <summary>
	/// Whether or not verbose output is displayed in the dimeRocker console.
	/// </summary>
	public bool verboseOutputInConsole;
	
	/// <summary>
	/// Whether or not verbose output is displayed in Unity's debug log.
	/// </summary>
	public bool verboseOutputInDebugLog;
	
	#endregion
	
	static bool _isReady;
	/// <summary>
	/// Whether or not the information has been initially loaded from the server.
	/// </summary>
	public static bool isReady {
		get { return _isReady; }
		private set { _isReady = value; }
	}
	
	/// <summary>
	/// True if the game is deployed on dimeRocker's domain.
	/// </summary>
	public static bool deployedOnDimeRocker {
		get { return Application.absoluteURL.Contains("dimerocker.com"); }
	}
	
	/// <summary>
	/// Returns true if the developer has provided required dimeRocker information.
	/// dimeRocker Secret Key and Version ID must be set. If using Unity Editor, stackUsername and stackPassword must be set.
	/// </summary>
	static bool hasRequiredInformation {
		get {
			if (instance.secretKey == null || instance.secretKey.Length == 0) {
				return false;
		    }
			
			return true;
		}
	}
	
	static dimeRocker _instance;
	/// <summary>
	/// An instance of this class for accessing inspector variables.
	/// </summary>
	public static dimeRocker instance {
		get { return _instance; }
		private set { _instance = value; }
	}
	
	dimeRocker () {}
	
	/// <summary>
	/// Sets up the dimeRocker object.
	/// </summary>
	void Awake ()
	{
		DontDestroyOnLoad(this);
		instance = this;
		
		// Rename dimeRocker game object to prevent external JavaScript attacks
		gameObject.name = drUtil.SHA1Hash(secretKey + UnityEngine.Random.Range(0, 9999));
	}
	
	void Update ()
	{
		// Toggle the console
		if (Input.GetKeyDown(consoleToggleKey)) {
			drDebug.show = !drDebug.show;
		}
	}
	
	void OnGUI ()
	{
		drDebug.OnGUI();
	}

	/// <summary>
	/// Initialization.
	/// </summary>
	IEnumerator Start ()
	{
		yield return Init();
	}
	
	/// <summary>
	/// Initializes dimeRocker.
	/// </summary>
	public static Coroutine Init ()
	{
		return RunCoroutine(InitCoroutine());
	}
	
	/// <summary>
	/// Initializes dimeRocker.
	/// </summary>
	static IEnumerator InitCoroutine ()
	{
		if (!hasRequiredInformation) {
			yield break;
		}
		
		// Only fetch the session if we're in the editor; it's not needed otherwise
		if (Application.isEditor) {
			//yield return drSession.FetchSession();
		}
		
		// Wait for the coroutines to complete
		while (drWWW.processing) {
			yield return null;
		}
		
		// Once the coroutines have finished loading their data, dimeRocker is fully ready to use
		dimeRocker.isReady = true;
		drDebug.Log("dimeRocker Custom Framework ready using version " + version);
	}
	
	/// <summary>
	/// Calls a coroutine using the scene's dimeRocker game object. An error is thrown if the prefab hasn't been instantiated.
	/// </summary>
	/// <param name="routine">The routine to execute.</param>
	/// <returns>The coroutine object.</returns>
	public static Coroutine RunCoroutine (IEnumerator routine)
	{
		if (instance == null) {
			drDebug.LogError("The dimeRocker prefab must exist in the scene before calls can be executed");
			return null;
		}
		
		return instance.StartCoroutine(routine);
	}
}

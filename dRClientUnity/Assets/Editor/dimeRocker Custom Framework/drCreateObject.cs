// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using UnityEditor;
using UnityEngine;

/// <summary>
/// A wizard for instantiating a dimeRocker game object.
/// </summary>
class drCreateObject : ScriptableWizard
{
	const string prefabPath = "Assets/Standard Assets/dimeRocker Custom Framework/dimeRocker.prefab";
	public string apiDomain = "";
	public string secretKey = "";

	void OnWizardCreate ()
	{
		// Instantiate the dimeRocker object
		Object prefab = Resources.LoadAssetAtPath(prefabPath, typeof(GameObject));
		GameObject go = EditorUtility.InstantiatePrefab(prefab) as GameObject;
		dimeRocker dr = go.GetComponent<dimeRocker>();
		dr.apiUrl = apiDomain;
		dr.secretKey = secretKey;
	}

	void OnWizardUpdate ()
	{
		helpString = "Fill in your game's API domain and secret key below.";
	}

	/// <summary>
	/// Called when the "Cancel" button is pressed.
	/// </summary>
	void OnWizardOtherButton ()
	{
		Close();
	}
}

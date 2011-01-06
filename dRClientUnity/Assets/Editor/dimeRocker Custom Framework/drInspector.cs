// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A custom inspector for the dimeRocker component.
/// </summary>
[CustomEditor(typeof(dimeRocker))]
class drInspector : Editor
{
	#region Labels
	
	GUIContent apiUrlLabel         = new GUIContent("API URL",                     "The API's URL.");
	GUIContent secretKeyLabel      = new GUIContent("Secret Key",                  "The game's secret key.");
	GUIContent debugOptionsLabel   = new GUIContent("Debug Options",               "Options for displaying logged output and errors.");
	GUIContent devModeLabel        = new GUIContent("Development Mode",            "Whether or not to allow console to appear.");
	GUIContent consoleToggleLabel  = new GUIContent("Console Toggle Key",          "Hotkey for toggling console window.");
	GUIContent verboseConsoleLabel = new GUIContent("Verbose Output In Console",   "Whether or not to show verbose output in the dimeRocker console.");
	GUIContent verboseDebugLabel   = new GUIContent("Verbose Output In Debug Log", "Whether or not to show verbose output in Unity's debug log.");
	
	#endregion
	
	bool showDebugOptions;

	/// <summary>
	/// Displays the custom inspector.
	/// </summary>
	override public void OnInspectorGUI ()
	{
		dimeRocker dr = target as dimeRocker;

		// Set the indent level to match all other inspectors
		EditorGUI.indentLevel = 1;
		
		dr.apiUrl = EditorGUILayout.TextField(apiUrlLabel, dr.apiUrl).Trim();
		dr.secretKey = EditorGUILayout.TextField(secretKeyLabel, dr.secretKey).Trim();
		
		EditorGUI.indentLevel = 0;
		
		showDebugOptions = EditorGUILayout.Foldout(showDebugOptions, debugOptionsLabel);
		
		if (showDebugOptions) {
			EditorGUI.indentLevel = 2;

			dr.devMode                 = EditorGUILayout.Toggle(devModeLabel,                   dr.devMode);
			dr.consoleToggleKey        = (KeyCode)EditorGUILayout.EnumPopup(consoleToggleLabel, dr.consoleToggleKey);
			dr.verboseOutputInConsole  = EditorGUILayout.Toggle(verboseConsoleLabel,            dr.verboseOutputInConsole);
			dr.verboseOutputInDebugLog = EditorGUILayout.Toggle(verboseDebugLabel,              dr.verboseOutputInDebugLog);
		}
	}
}

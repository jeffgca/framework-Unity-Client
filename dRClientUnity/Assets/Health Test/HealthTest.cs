// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tests all the dimeRocker functions to ensure that they're working properly.
/// </summary>
public class HealthTest : MonoBehaviour
{
	public enum Status { Untested, Success, Failure }
	
	static readonly string[] statusLabels = { "Untested", "Success", "Failure" };
	static readonly Color[] statusColors = { Color.white, Color.green, Color.red };
	
	readonly string[] tabs = new string[] { 
		"Initialization", 
		"Data Store", 
		"Leaderboards", 
		"Wallets", 
		"Counters", 
		"JavaScript" 
	};
	
	int selectedTab;
	Vector2 consoleScrollPos;
	const int margin = 10;
	
	void OnGUI ()
	{
		Rect windowRect = new Rect(margin, margin, Screen.width - (2 * margin), Screen.height - (2 * margin));
		// Register the window
		GUILayout.Window(0, windowRect, DisplayWindow, "dimeRocker Health Test - v" + dimeRocker.version);
	}
	
	/// <summary>
	/// Displays the window.
	/// </summary>
	/// <param name="windowID">The window's ID.</param>
	void DisplayWindow (int windowID)
	{
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(GUI.skin.box);
				selectedTab = GUILayout.Toolbar(selectedTab, tabs);
				
				GUILayout.Space(10);
				
				switch (selectedTab) {
					case 0:
						InitializationGUI();
						break;
					case 1:
						DataStoreTest.OnGUI();
						break;
					case 2:
						LeaderboardsTest.OnGUI();
						break;
					case 3:
						WalletsTest.OnGUI();
						break;
					case 4:
						CountersTest.OnGUI();
						break;
					case 5: 
						JavascriptCommTest.OnGUI();
						break;
				}
			GUILayout.EndVertical();
		
			consoleScrollPos = GUILayout.BeginScrollView(consoleScrollPos, GUILayout.Width(300));
				drDebug.ShowMessages();
			GUILayout.EndScrollView();
		GUILayout.EndHorizontal();
	}
	
	void InitializationGUI ()
	{
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				
				GUILayout.Label("API URL", GUILayout.ExpandWidth(false));
				GUILayout.Label("Secret Key", GUILayout.ExpandWidth(false));
				
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				
				dimeRocker.instance.apiUrl = GUILayout.TextField(dimeRocker.instance.apiUrl);
				dimeRocker.instance.secretKey = GUILayout.TextField(dimeRocker.instance.secretKey);
				
				if (GUILayout.Button("Initialize")) {
					dimeRocker.Init();
				}
				
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	public static void StatusLabel (Status status)
	{
		GUI.color = statusColors[(int)status];
		GUILayout.Label(statusLabels[(int)status], GUILayout.Width(80));
		GUI.color = Color.white;
	}
}

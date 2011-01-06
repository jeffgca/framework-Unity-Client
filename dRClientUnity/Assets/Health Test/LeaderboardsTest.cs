using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardsTest
{
	static LeaderboardsTest _instance;
	static LeaderboardsTest instance {
		get {
			if (_instance == null) {
				_instance = new LeaderboardsTest();
			}
			
			return _instance;
		}
	}
	
	static Vector2 scrollPos;
	
	static string leaderboardName = "Default";
	static int score = 9000;
	static string[] values = new string[] { "", "", "" };
	static int offset;
	static int length = 10;
	
	public static void OnGUI ()
	{
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				GUILayout.Label("");
				GUILayout.Label("Leaderboard");
				GUILayout.Label("Score");
				
				for (int i = 1; i <= values.Length; i++) {
					GUILayout.Label("Value " + i);
				}
		
				GUILayout.Label("Offset");
				GUILayout.Label("Length");
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				if (GUILayout.Button("Get Available Leaderboards")) {
					drLeaderboards.FetchAvailableLeaderboards(delegate {
						drDebug.Log("Available leaderboards:");
						
						foreach (string name in drLeaderboards.availableLeaderboards) {
							drDebug.Log(name);
						}
					});
				}
				
				leaderboardName = GUILayout.TextField(leaderboardName);
		
				try {
					score = int.Parse(GUILayout.TextField(score.ToString()));
				} catch {
					score = 9000;
				}
				
				
				for (int i = 0; i < values.Length; i++) {
					values[i] = GUILayout.TextField(values[i]);
				}
				
				try {
					offset = int.Parse(GUILayout.TextField(offset.ToString()));
				} catch {
					offset = 0;
				}
		
				try {
					length = int.Parse(GUILayout.TextField(length.ToString()));
				} catch {
					length = 0;
				}
				
				if (GUILayout.Button("Post Score")) {
					drLeaderboards.PostScore(leaderboardName, score, values);
				}
		
				if (GUILayout.Button("Fetch Entries")) {
					drLeaderboards.FetchEntries(leaderboardName, offset, length, delegate {
						drDebug.Log("Leaderboard entries:");
						drLeaderboards.Leaderboard lb = drLeaderboards.GetLeaderboard(leaderboardName);
						
						if (lb == null) {
							Debug.LogWarning("Leaderboard object is null");
							return;
						}
						
						foreach (KeyValuePair<int, drLeaderboards.Entry> kvp in lb.entries) {
							drDebug.Log("Rank " + kvp.Key + ": " + kvp.Value);
						}
					});
				}
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountersTest
{
	static CountersTest _instance;
	static CountersTest instance {
		get {
			if (_instance == null) {
				_instance = new CountersTest();
			}
			
			return _instance;
		}
	}
	
	static Vector2 scrollPos;
	
	static string counterName = "Default";
	static int amount = 1;
	
	public static void OnGUI ()
	{
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				GUILayout.Label("Counter");
				GUILayout.Label("Amount");
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				counterName = GUILayout.TextField(counterName);
				
				try {
					amount = int.Parse(GUILayout.TextField(amount.ToString()));
				} catch {
					amount = 1;
				}
				
				if (GUILayout.Button("Increment")) {
					drCounters.Increment(counterName, amount, delegate {
						drCounters.Counter counter = drCounters.GetCounter(counterName);
						drDebug.Log(counter);
					});
				}
		
				if (GUILayout.Button("Decrement")) {
					drCounters.Decrement(counterName, amount, delegate {
						drCounters.Counter counter = drCounters.GetCounter(counterName);
						drDebug.Log(counter);
					});
				}
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}

using System.Collections;
using UnityEngine;

public class DataStoreTest
{
	static DataStoreTest _instance;
	static DataStoreTest instance {
		get {
			if (_instance == null) {
				_instance = new DataStoreTest();
			}
			
			return _instance;
		}
	}
	
	static Vector2 scrollPos;
	
	static HealthTest.Status
		dsSet,
		dsFetch,
		dsUnset,
		dsReplace;
	
	public static void OnGUI ()
	{
		if (GUILayout.Button("Run Tests")) {
			instance.RunTests();
		}
		
		scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.BeginHorizontal();
				// Labels
				GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
					
					GUILayout.Label("Set:",     GUILayout.ExpandWidth(false));
					GUILayout.Label("Get:",     GUILayout.ExpandWidth(false));
					GUILayout.Label("Unset:",   GUILayout.ExpandWidth(false));
					GUILayout.Label("Replace:", GUILayout.ExpandWidth(false));
			
				GUILayout.EndVertical();
				
				// Statuses
				GUILayout.BeginVertical();
					
					HealthTest.StatusLabel(dsSet);
					HealthTest.StatusLabel(dsFetch);
					HealthTest.StatusLabel(dsUnset);
					HealthTest.StatusLabel(dsReplace);
					
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
	}
	
	Coroutine RunTests()
	{
		return dimeRocker.RunCoroutine(RunTestsCoroutine());
	}
	
	IEnumerator RunTestsCoroutine ()
	{
		const string ns = "valuables";
		
		Hashtable sampleData1 = new Hashtable();
		sampleData1.Add("weapon", "bazooka");
		sampleData1.Add("health", 23);
		sampleData1.Add("loves", true);
		sampleData1.Add("beatHipShip", false);
		
		Hashtable sampleData2 = new Hashtable();
		sampleData2.Add("location", "Vancouver");
		sampleData2.Add("nothingimportant", null);
		
		yield return drDataStore.Set(ns, sampleData1,
			delegate { dsSet = HealthTest.Status.Success; },
			delegate { dsSet = HealthTest.Status.Failure; }
		);
		
		yield return drDataStore.Fetch(ns, new string[] { "health", "towns conquered", "beat Hip Ship" },
			delegate { 
				dsFetch = HealthTest.Status.Success; 		
				foreach(DictionaryEntry d in drDataStore.GetData("valuables").data) {
					drDebug.Log(d.Key + " : " + d.Value);	
				}
			},
			delegate { dsFetch = HealthTest.Status.Failure; }
		);
		
		yield return drDataStore.Unset(ns, new string[] { "beat Hip Ship", "health" },
			delegate { dsUnset = HealthTest.Status.Success; },
			delegate { dsUnset = HealthTest.Status.Failure; }
		);
		
		// this will set the data to a blank document
		yield return drDataStore.Replace(ns, new Hashtable(),
			delegate { dsReplace = HealthTest.Status.Success; },
			delegate { dsReplace = HealthTest.Status.Failure; }
		);
	}
}

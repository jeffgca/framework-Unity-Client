using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavascriptCommTest
{
	protected static GameObject _JSListener; 
	
	public static void OnGUI () {
		
		// Dynamically allocate a new GameObject with a Listener listener
		if (_JSListener == null) {
			_JSListener = new GameObject("JSListener"); 
			_JSListener.AddComponent("JSListener");
			_JSListener.GetComponent<JSListener>().Log("JSListener Created.");
		}
		
		if (GUILayout.Button("Register Callback for bucket: FB.Friends")) {
			drDebug.Log("Registering FB.Friends");
			
			Application.ExternalCall( "unityRegisterBucketCallback", "FB.Friends", "JSListener", "Log");
		}
		
		if (GUILayout.Button("Register Callback for bucket: FB.AppFriends")) {
			drDebug.Log("Registering FB.AppFriends");
			Application.ExternalCall( "unityRegisterBucketCallback", "FB.AppFriends", "JSListener", "Log");			
		}

		if (GUILayout.Button("Register Callback for bucket: Test.TestBucket")) {
			drDebug.Log("Registering Test.TestBucket");
			Application.ExternalCall( "unityRegisterBucketCallback", "Test.TestBucket", "JSListener", "Log");			
		}		
	}
}
using UnityEngine;
using System.Collections;

/**
 * A simple listener that logs everything it gets back. This is used 
 * to bind functionality to the JS tester. 
 */
public class JSListener : MonoBehaviour {
	public void Log( string data ) {
		drDebug.Log(data);		
	}
}

// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using UnityEditor;
using UnityEngine;

/// <summary>
/// Displays menus for dimeRocker functionality.
/// </summary>
static class drMenus
{
	/// <summary>
	/// Adds menu item named "Create dimeRocker Object" to the dimeRocker menu, which places an instance of the dimeRocker prefab into the scene.
	/// </summary>
	[MenuItem("dimeRocker/Create dimeRocker Object")]
	static void CreateObject ()
	{
		ScriptableWizard.DisplayWizard("Create dimeRocker Object", typeof(drCreateObject), "Create", "Cancel");
	}

	/// <summary>
	/// Validates the "Create dimeRocker Object" menu item, disabling it if a dimeRocker prefab instance already exists in the scene.
	/// </summary>
	/// <returns>Whether or not the menu is enabled.</returns>
	[MenuItem("dimeRocker/Create dimeRocker Object", true)]
	static bool ValidateCreateObject ()
	{
		GameObject drPrefab = GameObject.Find("dimeRocker");
		return drPrefab == null;
	}
}

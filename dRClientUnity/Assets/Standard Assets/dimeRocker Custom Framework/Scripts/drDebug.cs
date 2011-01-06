// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A console window for logging messages and errors.
/// </summary>
public static class drDebug
{
	/// <summary>
	/// A simple struct for holding message information.
	/// </summary>
	struct Message
	{
		public string text;
		public LogType type;
	}
	
	/// <summary>
	/// Whether or not the console window is visible.
	/// </summary>
	public static bool show;

	/// <summary>
	/// Whether or not to condense repeated messages into a single one.
	/// </summary>
	public static bool collapse;
	
	/// <summary>
	/// The logged messages.
	/// </summary>
	static List<Message> log = new List<Message>();
	
	const int windowId = 9999;
	const float windowMargin = 0.1f;
	static Vector2 scrollPos;
	static Rect windowRect = new Rect(windowMargin, windowMargin, Screen.width - (2 * windowMargin), Screen.height - (2 * windowMargin));

	static GUIContent clearLabel    = new GUIContent("Clear",    "Clear the console log.");
	static GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	internal static void OnGUI ()
	{
		// Console will not be visible if the game is not in development mode
		if (!show || !dimeRocker.instance.devMode) {
			return;
		}

		windowRect = GUILayout.Window(windowId, windowRect, ConsoleWindow, "Console");
	}

	/// <summary>
	/// A window displaying a list of debug messages.
	/// </summary>
	/// <param name="windowID">The window ID.</param>
	static void ConsoleWindow (int windowID)
	{
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		ShowMessages();
		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();

			if (GUILayout.Button(clearLabel)) {
				Clear();
			}

			collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

		GUILayout.EndHorizontal();

		// Set the window to be draggable by the top title bar
		GUI.DragWindow(new Rect(0, 0, 10000, 20));
	}
	
	/// <summary>
	/// Displays the messages logged so far.
	/// </summary>y>
	public static void ShowMessages ()
	{
		// Go through each entry in the log
		for (int i = 0; i < log.Count; i++) {
			Message entry = log[i];

			switch (entry.type) {
				case LogType.Warning:
					GUI.contentColor = Color.yellow;
					break;

				case LogType.Error:
				case LogType.Exception:
					GUI.contentColor = Color.red;
					break;

				default:
					GUI.contentColor = Color.white;
					break;
			}

			// If this message is the same as the last one and the collapse feature is chosen, don't both displaying it
			if (collapse && i > 0 && entry.text == log[i - 1].text) {
				continue;
			}

			GUILayout.Label(entry.text);
		}

		GUI.contentColor = Color.white;
	}

	/// <summary>
	/// Logs a message to the dimeRocker console.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Log (object message)
	{
		LogMessage(message, LogType.Log);
	}
	
	/// <summary>
	/// Logs a warning to the dimeRocker console.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void LogWarning (object message)
	{
		LogMessage(message, LogType.Warning);
	}
	
	/// <summary>
	/// Logs an error to the dimeRocker console.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void LogError (object message)
	{
		LogMessage(message, LogType.Error);
	}
	
	/// <summary>
	/// Logs a message to the dimeRocker console.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="type">The type of message.</param>
	static void LogMessage (object message, LogType type)
	{
		if (message == null) {
			return;
		}
		
		Message msg = new Message();
		msg.text = message.ToString();
		msg.type = type;
		
		log.Add(msg);
		// Scroll to the bottom
		scrollPos = new Vector2(0, 99999);
	}
	
	/// <summary>
	/// Clears the console log.
	/// </summary>
	public static void Clear ()
	{
		log.Clear();
	}

	/// <summary>
	/// Warns the developer of a deprecated function.
	/// </summary>
	/// <param name="function">The name of the deprecated function.</param>
	/// <param name="replacement">The function to use instead.</param>
	internal static void DeprecationWarning (string function, string replacement)
	{
		LogWarning("The function " + function + " has been deprecated. Please use " + replacement + " instead.");
	}
}

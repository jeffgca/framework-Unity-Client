using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletsTest
{
	static WalletsTest _instance;
	static WalletsTest instance {
		get {
			if (_instance == null) {
				_instance = new WalletsTest();
			}
			
			return _instance;
		}
	}
	
	static Vector2 scrollPos;
	
	static string walletName = "RMT";
	static int amount = 5;
	
	public static void OnGUI ()
	{
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				GUILayout.Label("Wallet");
				GUILayout.Label("Amount");
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				walletName = GUILayout.TextField(walletName);
				
				try {
					amount = int.Parse(GUILayout.TextField(amount.ToString()));
				} catch {
					amount = 5;
				}
				
				if (GUILayout.Button("Fetch Balance")) {
					drWallets.FetchBalance(walletName, delegate {
						drWallets.Wallet wallet = drWallets.GetWallet(walletName);
						Debug.Log(wallet);
					});
				}
				
				if (GUILayout.Button("Credit")) {
					drWallets.Credit(walletName, amount, delegate {
						drWallets.Wallet wallet = drWallets.GetWallet(walletName);
						Debug.Log(wallet);
					});
				}
		
				if (GUILayout.Button("Debit")) {
					drWallets.Debit(walletName, amount, delegate {
						drWallets.Wallet wallet = drWallets.GetWallet(walletName);
						Debug.Log(wallet);
					});
				}
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}

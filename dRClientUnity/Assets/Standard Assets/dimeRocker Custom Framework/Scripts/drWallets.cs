// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using Procurios.Public; // JSON parser
using UnityEngine;

/// <summary>
/// Functionality for wallets.
/// </summary>
public class drWallets
{
	/// <summary>
	/// A basic class for holding the wallet's name and balance.
	/// </summary>
	public class Wallet
	{
		/// <summary>
		/// The name of the wallet.
		/// </summary>
		public readonly string name;
		
		int _balance;
		/// <summary>
		/// The wallet's balance.
		/// </summary>
		public int balance {
			get { return _balance; }
			internal set { _balance = balance; }
		}
		
		/// <summary>
		/// Initializes a new instance of the Wallet class.
		/// </summary>
		/// <param name="name">The name of the wallet.</param>
		internal Wallet (string name)
		{
			this.name = name;
		}
		
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="drWallets.Wallet"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="drWallets.Wallet"/>.</returns>
		public override string ToString ()
		{
			return "wallet: " + name + "; balance: " + balance;
		}
	}
	
	/// <summary>
	/// Wallets that have been downloaded from the server.
	/// </summary>
	static List<Wallet> fetchedWallets = new List<Wallet>();
	
	drWallets () {}
	
	/// <summary>
	/// Fetches the current Wallet value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	public static Coroutine FetchBalance (string name)
	{
		return FetchBalance(name, null, null);
	}
	
	/// <summary>
	/// Fetches the current Wallet value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="success">Callback triggers on successful fetch.</param>
	public static Coroutine FetchBalance (string name, dimeRocker.SuccessHandler success)
	{
		return FetchBalance(name, success, null);
	}
	
	/// <summary>
	/// Fetches the current Wallet value for one or more users.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="success">Callback triggers on successful fetch.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine FetchBalance (string name, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.walletBalance, name);
		
		www.OnSuccess += delegate {
			int result = 0;
			int.TryParse(www.result.ToString(), out result);
			StoreReturn(name, result);
			drDebug.Log("Fetched wallet " + name + " balance");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error fetching wallet " + name + ": " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Credits the wallet's value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to increment.</param>
	public static Coroutine Credit (string name, int amount)
	{
		return Credit(name, amount, null, null, null);
	}
	
	/// <summary>
	/// Credits the wallet's value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	public static Coroutine Credit (string name, int amount, dimeRocker.SuccessHandler success)
	{
		return Credit(name, amount, null, success, null);
	}
	
	/// <summary>
	/// Credits the wallet's value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Credit (string name, int amount, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return Credit(name, amount, null, success, error);
	}
	
	/// <summary>
	/// Credits the Wallet value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="description">A description of the transaction.</param>
	public static Coroutine Credit (string name, int amount, string description)
	{
		return Credit(name, amount, description, null, null);
	}
	
	/// <summary>
	/// Credits the Wallet value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="description">A description of the transaction.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	public static Coroutine Credit (string name, int amount, string description, dimeRocker.SuccessHandler success)
	{
		return Credit(name, amount, description, success, null);
	}
	
	/// <summary>
	/// Credits the Wallet value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to increment.</param>
	/// <param name="description">A description of the transaction.</param>
	/// <param name="success">Callback triggers on successful increment.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Credit (string name, int amount, string description, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.walletCredit, name);
		www.AddField("amount", amount);
		
		if (description != null && description != "") {
			www.AddField("description", description);
		}
		
		www.OnSuccess += delegate {
			int result = 0;
			int.TryParse(www.result.ToString(), out result);
			StoreReturn(name, result);
			drDebug.Log("Credited wallet " + name);
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error crediting wallet " + name + ": " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Debits the wallet's value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to Debit.</param>
	public static Coroutine Debit (string name, int amount)
	{
		return Debit(name, amount, null, null, null);
	}
	
	/// <summary>
	/// Debits the wallet's value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to Debit.</param>
	/// <param name="success">Callback triggers on successful debit.</param>
	public static Coroutine Debit (string name, int amount, dimeRocker.SuccessHandler success)
	{
		return Debit(name, amount, null, success, null);
	}
	
	/// <summary>
	/// Debits the wallet's value.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to Debit.</param>
	/// <param name="success">Callback triggers on successful debit.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Debit (string name, int amount, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		return Debit(name, amount, null, success, error);
	}
	
	/// <summary>
	/// Debits the Wallet value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to Debit.</param>
	/// <param name="description">A description of the transaction.</param>
	public static Coroutine Debit (string name, int amount, string description)
	{
		return Debit(name, amount, description, null, null);
	}
	
	/// <summary>
	/// Debits the Wallet value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to Debit.</param>
	/// <param name="description">A description of the transaction.</param>
	/// <param name="success">Callback triggers on successful debit.</param>
	public static Coroutine Debit (string name, int amount, string description, dimeRocker.SuccessHandler success)
	{
		return Debit(name, amount, description, success, null);
	}
	
	/// <summary>
	/// Debits the Wallet value(s) for one or more users.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="amount">The amount to Debit.</param>
	/// <param name="description">A description of the transaction.</param>
	/// <param name="success">Callback triggers on successful debit.</param>
	/// <param name="error">Callback triggers on error.</param>
	public static Coroutine Debit (string name, int amount, string description, dimeRocker.SuccessHandler success, dimeRocker.ErrorHandler error)
	{
		drWWW www = GetWWW(drAPI.walletDebit, name);
		www.AddField("amount", amount);
		
		if (description != null && description != "") {
			www.AddField("description", description);
		}
		
		www.OnSuccess += delegate {
			int result = 0;
			int.TryParse(www.result.ToString(), out result);
			StoreReturn(name, result);
			drDebug.Log("Debited wallet");
		}; www.OnSuccess += success;
		
		www.OnError += delegate (string errorMessage) {
			drDebug.LogError("Error debiting wallet " + name + ": " + errorMessage);
		}; www.OnError += error;
		
		return www.Fetch();
	}
	
	/// <summary>
	/// Gets the wallet with the given name.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <returns>The wallet.</returns>
	public static Wallet GetWallet (string name)
	{
		foreach (Wallet wallet in fetchedWallets) {
			if (wallet.name == name) {
				return wallet;
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Stores the returned wallet information.
	/// </summary>
	/// <param name="name">The name of the wallet.</param>
	/// <param name="balance">The wallet's balance.</param>
	static void StoreReturn (string name, int balance)
	{
		Wallet wallet = GetWallet(name);
		
		if (wallet == null) {
			wallet = new Wallet(name);
			fetchedWallets.Add(wallet);
		}
		
		wallet.balance = balance;
	}
	
	/// <summary>
	/// Creates a WWW object prepopulated with fields common to all calls.
	/// </summary>
	/// <param name="api">The call's API.</param>
	/// <param name="name">The name of the wallet.</param>
	/// <returns>The WWW object.</returns>
	static drWWW GetWWW (drAPI.API api, string name)
	{
		drWWW www = new drWWW(api);
		www.AddField("name", name);
		return www;
	}
}

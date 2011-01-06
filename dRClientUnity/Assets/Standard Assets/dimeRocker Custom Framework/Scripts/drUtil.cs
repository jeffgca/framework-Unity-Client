// Copyright 2010 OverInteractive Media Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Utility functions.
/// </summary>
public static class drUtil
{
	/// <summary>
	/// Creates a SHA-1 hash from a string.
	/// </summary>
	/// <remarks>Algorithm from http://en.wikipedia.org/wiki/Sha1#SHA-1_pseudocode</remarks>
	/// <param name="message">The string to hash.</param>
	/// <returns>The SHA-1 hash.</returns>
	public static string SHA1Hash (string message)
	{
		// Initialize variables
		uint h0 = 0x67452301;
		uint h1 = 0xEFCDAB89;
		uint h2 = 0x98BADCFE;
		uint h3 = 0x10325476;
		uint h4 = 0xC3D2E1F0;
		
		int msgLength = message.Length;
		List<uint> words = new List<uint>();
		
		// Pre-processing
		
		for (int i = 0; i < msgLength - 3; i += 4) {
			uint j = (uint)message[i] << 24 | (uint)message[i + 1] << 16 | (uint)message[i + 2] << 8 | (uint)message[i + 3];
			words.Add(j);
		}
		
		// Append the bit '1' to the message
		uint end = 0;
		
		switch (msgLength % 4) {
			case 0:
				end = 0x080000000;
			break;
			case 1:
				end = (uint)message[msgLength - 1] << 24 | 0x0800000;
			break;
			case 2:
				end = (uint)message[msgLength - 2] << 24 | (uint)message[msgLength - 1] << 16 | 0x08000;
			break;
			case 3:
				end = (uint)message[msgLength - 3] << 24 | (uint)message[msgLength - 2] << 16 | (uint)message[msgLength - 1] << 8 | 0x80;
			break;
		}
		
		words.Add(end);
		
		// Append 0 ≤ k < 512 bits '0', so that the resulting message length (in bits) is congruent to 448 ≡ −64 (mod 512)
		while (words.Count % 16 != 14) {
			words.Add(0);
		}
		
		// Append length of message (before pre-processing), in bits, as 64-bit big-endian integer
		words.Add((uint)msgLength >> 29);
		words.Add((uint)msgLength << 3);
		
		uint[] w = new uint[80];
		uint a, b, c, d, e, f, k, temp;
		
		// Process the message in successive 512-bit chunks
		for (int chunk = 0; chunk < words.Count; chunk += 16) {
			// Break chunk into sixteen 32-bit big-endian words
			for (int i = 0; i < 16; i++) {
				w[i] = words[chunk + i];
			}
			
			// Extend the sixteen 32-bit words into eighty 32-bit words
			for (int i = 16; i < 80; i++) {
				w[i] = RotateLeft(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);
			}
			
			// Initialize hash value for this chunk
			a = h0;
			b = h1;
			c = h2;
			d = h3;
			e = h4;
			
			// Main loop
			for (int i = 0; i < 80; i++) {
				if (i < 20) {
					f = (b & c) | (~b & d);
					k = 0x5A827999;
				} else if (i < 40) {
					f = b ^ c ^ d;
					k = 0x6ED9EBA1;
				} else if (i < 60) {
					f = (b & c) | (b & d) | (c & d);
					k = 0x8F1BBCDC;
				} else {
					f = b ^ c ^ d;
					k = 0xCA62C1D6;
				}
				
				temp = RotateLeft(a, 5) + f + e + k + w[i];
				e = d;
				d = c;
				c = RotateLeft(b, 30);
				b = a;
				a = temp;
			}
			
			// Add this chunk's hash to result so far
			h0 = h0 + a;
			h1 = h1 + b;
			h2 = h2 + c;
			h3 = h3 + d;
			h4 = h4 + e;
		}
		
		// Produce the final hash value (big-endian)
		return h0.ToString("x8") + h1.ToString("x8") + h2.ToString("x8") + h3.ToString("x8") + h4.ToString("x8");
	}
	
	/// <summary>
	/// Performs a circular / bitwise shift left on an unsigned int.
	/// </summary>
	/// <remarks>Algorithm from http://en.wikipedia.org/wiki/Circular_shift#Implementing_circular_shifts</remarks>
	/// <param name="value">The number to shift.</param>
	/// <param name="shift">The amount to shift.</param>
	/// <returns>The left rotated int.</returns>
	static uint RotateLeft (uint value, int shift)
	{
		if ((shift &= 31) == 0) {
			return value;
		}
		
		return (value << shift) | (value >> (32 - shift));
	}

	/// <summary>
	/// Returns the date from a hash table item, or a minimum date if the object is null.
	/// </summary>
	/// <param name="hashItem">The hash table item.</param>
	/// <returns>The parsed date.</returns>
	public static DateTime GetDateFromHashItem (object hashItem)
	{
		if (hashItem == null) {
			return DateTime.MinValue;
		} else {
			return DateTime.Parse(hashItem.ToString());
		}
	}
	
	/// <summary>
	/// Converts a Unix timestamp to a DateTime object.
	/// </summary>
	/// <param name="timestamp">The Unix timestamp.</param>
	/// <returns>The DateTime object.</returns>
	public static DateTime ConvertFromUnixTimestamp (double timestamp)
	{
		DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return origin.AddSeconds(timestamp);
	}
	
	/// <summary>
	/// Converts a DateTime object to a to Unix timestamp.
	/// </summary>
	/// <param name="date">The DateTime object.</param>
	/// <returns>The Unix timestamp.</returns>
	public static int ConvertToUnixTimestamp (DateTime date)
	{
		DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		TimeSpan diff = date - origin;
		return (int)Math.Floor(diff.TotalSeconds);
	}
	
	/// <summary>
	/// Builds a GET parameters string suitable for appending to a URL.
	/// </summary>
	/// <param name="parameters">The parameters.</param>
	/// <returns>The GET parameters string.</returns>
	public static string BuildGETParametersString (IDictionary<string, object> parameters)
	{
		StringBuilder str = new StringBuilder();
		
		foreach (KeyValuePair<string, object> kvp in parameters) {
			if (kvp.Value == null) {
				continue;
			}
			
			str.Append("&");
			str.Append(kvp.Key);
			str.Append("=");
			str.Append(kvp.Value.ToString());
		}
		
		str.Replace('&', '?', 0, 2); // Replace first occurrence of '&' with '?'
		
        return Uri.EscapeUriString(str.ToString());
	}
	
	/// <summary>
	/// Builds a WWWForm object suitable for sending along with a WWW object.
	/// </summary>
	/// <param name="parameters">The parameters.</param>
	/// <returns>The POST parameters form.</returns>
	public static WWWForm BuildPOSTParametersForm (IDictionary<string, object> parameters)
	{
		WWWForm form = new WWWForm();
		
		foreach (KeyValuePair<string, object> kvp in parameters) {
			if (kvp.Value == null) {
				continue;
			}
			
			form.AddField(kvp.Key, kvp.Value.ToString());
		}

		return form;
	}
}

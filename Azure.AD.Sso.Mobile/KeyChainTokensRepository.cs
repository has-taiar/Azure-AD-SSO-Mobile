using MonoTouch.Security;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Text;
using System;

namespace Azure.AD.Sso.Mobile
{
	public interface ITokensRepository 
	{
		bool SaveKey (string key, string val, string keyDescription);
		string GetKey(string key, string defaultValue);
		bool SaveKeys (Dictionary<string,string> secrets);
	}

	public class TokensRepository : ITokensRepository
	{
		private const string _keyChainAccountName = "myService";
		public bool SaveKey (string key, string val, string keyDescription)
		{
			// Store a password.
			var setResult = KeychainHelpers.SetPasswordForUsername ( key, val, _keyChainAccountName, SecAccessible.WhenUnlockedThisDeviceOnly, false );
			return setResult == SecStatusCode.Success;
//			// Delete a password
//			KeychainHelpers.DeletePasswordForUsername ("hallo@example.com", "myService", false);


		}

		public string GetKey (string key, string defaultValue)
		{
			// Retrieve a password
			return KeychainHelpers.GetPasswordForUsername (key, _keyChainAccountName, false) ?? defaultValue;

		}

		public TokensRepository ()
		{
		}

		public bool SaveKeys(Dictionary<string,string> secrets)
		{
			var result = true;
			foreach (var key in secrets.Keys) 
			{
				result = result && SaveKey (key, secrets [key], string.Empty);
			}

			return result;
		}
	}
}
	
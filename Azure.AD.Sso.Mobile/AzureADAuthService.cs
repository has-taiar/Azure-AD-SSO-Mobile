using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MonoTouch.UIKit;
using System.Linq;

namespace Azure.AD.Sso.Mobile
{
	public class AzureADAuthService : DelegatingHandler
	{
		static AzureADAuthService instance = new AzureADAuthService ();
		// todo: update with your values 
		const string applicationURL = @"https://yourAzureMobileServiceUrl.azure-mobile.net/";
		const string applicationKey = @"YourAzureMobileServiceKey";
		const string authority = "https://login.windows.net/YourAccountAuthroity.onmicrosoft.com";
		const string resourceId1 = "Your-First-Resource-Id";
		const string resourceId2 = "Your-Second-Resource-Id";
		const string clientId = "Your-Native-Mobile-App-Client-ID-From-Azure";
		const string redirectUrl = "Your-Native-Mobile-App-Redirect-Id-From-AAD";

		// this table referece would be used to store tokens on AzureMobileServices to share tokens across mobile apps
		IMobileServiceTable<Tokens> tokensTable;
		UIViewController _controller;
		MobileServiceClient client;
		private AuthenticationContext _authContext;
		private ITokensRepository _repository;

		AzureADAuthService ()
		{
			// these two lines are very important to setting up the AzureMobileServices lib and the ADAL lib
			CurrentPlatform.Init ();
			AdalInitializer.Initialize ();
		}

		public async Task AsyncInit(UIViewController controller, ITokensRepository repository)
		{
			_controller = controller;
			_repository = repository;

			_authContext = new AuthenticationContext (authority);
		
			// Initialize the Mobile Service client with your URL and key
			client = new MobileServiceClient (applicationURL, applicationKey, this);
			tokensTable = client.GetTable<Tokens> ();

		}
	
		public async Task<string> RefreshTokensLocally()
		{
			var refreshToken = _repository.GetKey (Constants.CacheKeys.RefreshToken, string.Empty);
			var authorizationParameters = new AuthorizationParameters (_controller);

			var result = "Refreshed an existing Token";
			bool hasARefreshToken = true;

			if (string.IsNullOrEmpty(refreshToken)) 
			{

				var localAuthResult = await _authContext.AcquireTokenAsync (
					resourceId1, clientId, new Uri (redirectUrl), authorizationParameters, UserIdentifier.AnyUser, null);

				refreshToken = localAuthResult.RefreshToken;
				_repository.SaveKey (Constants.CacheKeys.WebService1Token, localAuthResult.AccessToken, null);


				hasARefreshToken = false;
				result = "Acquired a new Token"; 
			} 

			var refreshAuthResult = await _authContext.AcquireTokenByRefreshTokenAsync(refreshToken, clientId, resourceId2);
			_repository.SaveKey (Constants.CacheKeys.WebService2Token, refreshAuthResult.AccessToken, null);

			if (hasARefreshToken) 
			{
				// this will only be called when we try refreshing the tokens (not when we are acquiring new tokens. 
				refreshAuthResult = await _authContext.AcquireTokenByRefreshTokenAsync (refreshAuthResult.RefreshToken, clientId, resourceId1);
				_repository.SaveKey (Constants.CacheKeys.WebService1Token, refreshAuthResult.AccessToken, null);
			}

			_repository.SaveKey (Constants.CacheKeys.RefreshToken, refreshAuthResult.RefreshToken, null);

			return result;
		}

		public async Task<string> RefreshTokensInAzureTable()
		{
			var tokensListOnAzure = await tokensTable.ToListAsync ();
			var tokenEntry = tokensListOnAzure.FirstOrDefault();
			var authorizationParameters = new AuthorizationParameters (_controller);

			var result = "Refreshed an existing Token";
			bool hasARefreshToken = true;

			if (tokenEntry == null) 
			{
				var localAuthResult = await _authContext.AcquireTokenAsync (
					resourceId1, clientId, new Uri (redirectUrl), authorizationParameters, UserIdentifier.AnyUser, null);

				tokenEntry = new Tokens {
					WebApi1AccessToken = localAuthResult.AccessToken,
					RefreshToken = localAuthResult.RefreshToken,
					Email = localAuthResult.UserInfo.DisplayableId,
					ExpiresOn = localAuthResult.ExpiresOn
				};
				hasARefreshToken = false;
				result = "Acquired a new Token"; 
			} 
				
			var refreshAuthResult = await _authContext.AcquireTokenByRefreshTokenAsync(tokenEntry.RefreshToken, clientId, resourceId2);
			tokenEntry.WebApi2AccessToken = refreshAuthResult.AccessToken;
			tokenEntry.RefreshToken = refreshAuthResult.RefreshToken;
			tokenEntry.ExpiresOn = refreshAuthResult.ExpiresOn;

			if (hasARefreshToken) 
			{
				// this will only be called when we try refreshing the tokens (not when we are acquiring new tokens. 
				refreshAuthResult = await _authContext.AcquireTokenByRefreshTokenAsync (refreshAuthResult.RefreshToken, clientId, resourceId1);
				tokenEntry.WebApi1AccessToken = refreshAuthResult.AccessToken;
				tokenEntry.RefreshToken = refreshAuthResult.RefreshToken;
				tokenEntry.ExpiresOn = refreshAuthResult.ExpiresOn;
			}


			if (hasARefreshToken)
				await tokensTable.UpdateAsync (tokenEntry);
			else
				await tokensTable.InsertAsync (tokenEntry);


			return result;
		}
			
		public static AzureADAuthService DefaultService {
			get {
				return instance;
			}
		}

		public async Task<Tokens> GetTokensFromAzureMobileService ()
		{
			var tokensList = await tokensTable.ToListAsync();
			return tokensList.FirstOrDefault ();
		}
	}
}


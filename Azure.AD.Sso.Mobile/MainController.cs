
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Azure.AD.Sso.Mobile
{
	public partial class MainController : UIViewController
	{
		private ITokensRepository _repository;
		private AzureADAuthService _azureADAuthService;

		public MainController () : base ("MainController", null)
		{
			_repository = new TokensRepository ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// async init the service
			_azureADAuthService = AzureADAuthService.DefaultService;
			await _azureADAuthService.AsyncInit (this, _repository);

			RefreshTokensLocalButton.TouchUpInside += HandleRefreshTokensLocally;
			RefreshTokensInAzureButton.TouchUpInside += HandleRefreshTokensInAzure;

			DisplayLocalTokensButtons.TouchUpInside += HandleDisplayLocalTokensFromKeyChain;
			DisplayTokensInAzureButton.TouchUpInside += HandleDisplayTokensFromAzureMobileService;
		}

		async void HandleRefreshTokensInAzure (object sender, EventArgs e)
		{
			var result = await _azureADAuthService.RefreshTokensInAzureTable ();
			DemoTextView.Text = result;
		}

		async void HandleRefreshTokensLocally (object sender, EventArgs e)
		{
			var result = await _azureADAuthService.RefreshTokensLocally ();
			DemoTextView.Text = result;
		}

		async void HandleDisplayLocalTokensFromKeyChain(object s, EventArgs e)
		{
			var refreshToken = _repository.GetKey (Constants.CacheKeys.RefreshToken, string.Empty);
			var token1 = _repository.GetKey (Constants.CacheKeys.WebService1Token, string.Empty);
			var token2 = _repository.GetKey (Constants.CacheKeys.WebService2Token, string.Empty);

			if (string.IsNullOrEmpty(refreshToken)) 
			{
				DemoTextView.Text = "No Existing Tokens";
				return;
			}

			DemoTextView.Text = "Local Tokens (in KeyChain):"+ Environment.NewLine;
			DemoTextView.Text += "RefreshToken: " + refreshToken.Substring(0, 10) + Environment.NewLine;
			DemoTextView.Text += "Token1: " + token1.Substring(0, 10) + Environment.NewLine;
			DemoTextView.Text += "Token2: " + token2.Substring(0, 10) + Environment.NewLine;
		}

		async void HandleDisplayTokensFromAzureMobileService(object s, EventArgs e)
		{
			var tokens = await _azureADAuthService.GetTokensFromAzureMobileService ();

			if (tokens == null) 
			{
				DemoTextView.Text = "No Existing Tokens";
				return;
			}

			DemoTextView.Text = "Remote Tokens (in Azure):"+ Environment.NewLine;
			DemoTextView.Text += "RefreshToken: " + tokens.RefreshToken.Substring(0, 10) + Environment.NewLine;
			DemoTextView.Text += "Token1: " + tokens.WebApi1AccessToken.Substring(0, 10) + Environment.NewLine;
			DemoTextView.Text += "Token2: " + tokens.WebApi2AccessToken.Substring(0, 10) + Environment.NewLine;

		}
	}
}


// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Azure.AD.Sso.Mobile
{
	[Register ("MainController")]
	partial class MainController
	{
		[Outlet]
		MonoTouch.UIKit.UITextView DemoTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DisplayLocalTokensButtons { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DisplayTokensInAzureButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton RefreshTokensInAzureButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton RefreshTokensLocalButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DisplayTokensInAzureButton != null) {
				DisplayTokensInAzureButton.Dispose ();
				DisplayTokensInAzureButton = null;
			}

			if (DisplayLocalTokensButtons != null) {
				DisplayLocalTokensButtons.Dispose ();
				DisplayLocalTokensButtons = null;
			}

			if (RefreshTokensInAzureButton != null) {
				RefreshTokensInAzureButton.Dispose ();
				RefreshTokensInAzureButton = null;
			}

			if (RefreshTokensLocalButton != null) {
				RefreshTokensLocalButton.Dispose ();
				RefreshTokensLocalButton = null;
			}

			if (DemoTextView != null) {
				DemoTextView.Dispose ();
				DemoTextView = null;
			}
		}
	}
}

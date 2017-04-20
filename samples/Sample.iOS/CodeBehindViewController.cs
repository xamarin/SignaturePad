using System;
using CoreGraphics;
using UIKit;

using Xamarin.Controls;

namespace Sample.iOS
{
	public partial class CodeBehindViewController : UIViewController
	{
		private SignaturePadView signaturepad;
		private UIButton btnSave;
		private UIButton btnLoad;
		private CGPoint[] points;

		public CodeBehindViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;

			// the Signature Pad
			signaturepad = new SignaturePadView ();
			signaturepad.Caption.Font = UIFont.FromName ("Marker Felt", 16f);
			signaturepad.CaptionText = "Authorization Signature";
			signaturepad.SignaturePromptText = "☛";
			signaturepad.SignaturePrompt.Font = UIFont.FromName ("Helvetica", 32f);
			signaturepad.BackgroundColor = UIColor.FromRGB (255, 255, 200); // a light yellow.
			signaturepad.BackgroundImageView.Image = UIImage.FromBundle ("logo-galaxy-black-64.png");
			signaturepad.BackgroundImageView.Alpha = 0.0625f;
			signaturepad.BackgroundImageView.ContentMode = UIViewContentMode.ScaleToFill;
			signaturepad.BackgroundImageView.Frame = new System.Drawing.RectangleF (20, 20, 256, 256);
			signaturepad.Layer.ShadowOffset = new System.Drawing.SizeF (0, 0);
			signaturepad.Layer.ShadowOpacity = 1f;
			View.AddSubviews (signaturepad);

			// the buttons
			btnSave = UIButton.FromType (UIButtonType.RoundedRect);
			btnSave.SetTitle ("Save", UIControlState.Normal);
			btnSave.TouchUpInside += (sender, e) =>
			{
				if (signaturepad.IsBlank)
				{
					new UIAlertView ("", "No signature to save.", null, "OK", null).Show ();
				}
				else
				{
					points = signaturepad.Points;
					new UIAlertView ("", "Vector Saved.", null, "OK", null).Show ();
				}
			};
			btnLoad = UIButton.FromType (UIButtonType.RoundedRect);
			btnLoad.SetTitle ("Load Last", UIControlState.Normal);
			btnLoad.TouchUpInside += (sender, e) =>
			{
				if (points != null)
				{
					signaturepad.LoadPoints (points);
				}
			};
			View.AddSubviews (btnSave, btnLoad);
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			var navHeight = NavigationController?.NavigationBar?.Bounds.Height ?? 0;
			navHeight += UIApplication.SharedApplication.StatusBarFrame.Height;

			signaturepad.Frame = new CGRect (10, 20 + navHeight, View.Bounds.Width - 20, View.Bounds.Height - 80 - navHeight);

			btnSave.Frame = new CGRect (10, View.Bounds.Height - 47, 120, 37);
			btnLoad.Frame = new CGRect (View.Bounds.Width - 130, View.Bounds.Height - 47, 120, 37);
		}
	}
}

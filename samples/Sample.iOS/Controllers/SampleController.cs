//
// SampleController.cs: View Controller to display a sample of the SignaturePadView.
//
// Author:
//   Timothy Risi (timothy.risi@gmail.com)
//
// Copyright (C) 2012 Timothy Risi
//
using System;
using MonoTouch.UIKit;

namespace Sample {
	public class SampleController : UIViewController {
		SampleView view;

		public SampleController ()
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			view = new SampleView();
			View = view;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (new Version (MonoTouch.Constants.Version) >= new Version (7, 0)) {
				UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
			}

			if (true) { // Customization activated.
				view.Signature.CaptionLabel.Text = "Authorization Signature";
				view.Signature.CaptionLabel.Font = UIFont.FromName ("Marker Felt", 16f);
				view.Signature.SignaturePromptLabel.Text = "☛";
				view.Signature.SignaturePromptLabel.Font = UIFont.FromName ("Helvetica", 32f);
				view.Signature.BackgroundColor = UIColor.FromRGB (255, 255, 200); // a light yellow.

				view.Signature.BackgroundImageView.Image = UIImage.FromBundle ("logo-galaxy-black-64.png");
				view.Signature.BackgroundImageView.Alpha = 0.0625f;
				view.Signature.BackgroundImageView.ContentMode = UIViewContentMode.ScaleToFill;
				view.Signature.BackgroundImageView.Frame = new System.Drawing.RectangleF (20, 20, 256, 256);

				// Modify shadow
				view.Signature.Layer.ShadowOffset = new System.Drawing.SizeF (0, 0);
				view.Signature.Layer.ShadowOpacity = 1f;
			}
		}
	}
}


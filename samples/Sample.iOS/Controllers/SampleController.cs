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

			// Customize the font.
			view.Signature.Caption.Text = "Authorization Signature";
			view.Signature.Caption.Font = UIFont.FromName ("Marker Felt", 16f);
		}
	}
}


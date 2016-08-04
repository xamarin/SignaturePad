//
// SampleViewController.cs
//
// Author:
//   Matthew Leibowitz (matthew.leibowitz@xamarin.com)
//
// Copyright (C) 2016 Xamarin Inc.
//

using System;
using CoreGraphics;
using UIKit;

namespace Sample.iOS
{
	public partial class SampleViewController : UIViewController
	{
		CGPoint[] points;
		UIImage savedImage;

		public SampleViewController()
			: base("SampleViewController", null)
		{
		}

		public SampleViewController(IntPtr handle)
			: base(handle)
		{
		}

		private bool ShowImageView
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad; }
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			saveButton.TouchUpInside += OnSaveTapped;
			loadButton.TouchUpInside += OnLoadTapped;

			signaturePad.Caption.Font = UIFont.FromName("Marker Felt", 16f);
			signaturePad.SignaturePrompt.Font = UIFont.FromName("Helvetica", 32f);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			imageView.Hidden = !ShowImageView;
			separator.Constant = ShowImageView ? 12f : -View.Frame.Height * 0.3f;
		}

		void OnSaveTapped(object sender, EventArgs e)
		{
			if (signaturePad.IsBlank)
			{
				new UIAlertView("Signature Pad", "No signature to save.", null, "OK", null).Show();
			}
			else
			{
				points = signaturePad.Points;
				if (ShowImageView)
				{
					imageView.Image = signaturePad.GetImage();
				}
				else
				{
					new UIAlertView("Signature Pad", "Vector Saved.", null, "OK", null).Show();
				}
			}
		}

		void OnLoadTapped(object sender, EventArgs e)
		{
			if (points != null)
			{
				signaturePad.LoadPoints(points);
			}
		}
	}
}

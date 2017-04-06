using System;
using CoreGraphics;
using UIKit;

namespace Sample.iOS
{
	public partial class SampleViewController : UIViewController
	{
		private CGPoint[] points;

		public SampleViewController ()
			: base ("SampleViewController", null)
		{
		}

		public SampleViewController (IntPtr handle)
			: base (handle)
		{
		}

		private bool ShowImageView => UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			saveButton.TouchUpInside += OnSaveTapped;
			loadButton.TouchUpInside += OnLoadTapped;

			signaturePad.Caption.Font = UIFont.FromName ("Marker Felt", 16f);
			signaturePad.SignaturePrompt.Font = UIFont.FromName ("Helvetica", 32f);
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			imageView.Hidden = !ShowImageView;
			separator.Constant = ShowImageView ? 12f : -View.Frame.Height * 0.3f;
		}

		private void OnSaveTapped (object sender, EventArgs e)
		{
			if (signaturePad.IsBlank)
			{
				new UIAlertView ("Signature Pad", "No signature to save.", null, "OK", null).Show ();
			}
			else
			{
				points = signaturePad.Points;
				if (ShowImageView)
				{
					imageView.Image = signaturePad.GetImage (UIColor.Black, UIColor.White, 1f);
				}
				else
				{
					new UIAlertView ("Signature Pad", "Vector Saved.", null, "OK", null).Show ();
				}
			}
		}

		private void OnLoadTapped (object sender, EventArgs e)
		{
			if (points != null)
			{
				signaturePad.LoadPoints (points);
			}
		}
	}
}

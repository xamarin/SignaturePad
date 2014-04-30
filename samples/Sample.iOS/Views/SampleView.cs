//
// SampleView.cs: View to display a sample of the SignaturePadView.
//
// Author:
//   Timothy Risi (timothy.risi@gmail.com)
//
// Copyright (C) 2012 Timothy Risi
//
using System;
using System.Drawing;
using MonoTouch.UIKit;

using SignaturePad;

namespace Sample {
	public class SampleView : UIView {
		public SignaturePadView Signature { get; set; }
		UIImageView imageView;
		UIButton btnSave, btnLoad;
		PointF [] points;

		public SampleView ()
		{
			BackgroundColor = UIColor.White;

			Frame = UIScreen.MainScreen.ApplicationFrame;

			var padding = 10f;
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
				padding = 84f;

			Signature = new SignaturePadView ();
			Signature.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			Signature.Frame = new RectangleF(padding, padding + 20, Frame.Width - (2 * padding), Frame.Height - 46 - (2 * padding));


			//Create the save button
			btnSave = UIButton.FromType (UIButtonType.RoundedRect);
			btnSave.Frame = new RectangleF (30, Frame.Height - 40, 100f, 44f);
			btnSave.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleTopMargin;
			btnSave.SetTitle ("Save", UIControlState.Normal);

			//Create the load button
			btnLoad = UIButton.FromType (UIButtonType.RoundedRect);
			btnLoad.Frame = new RectangleF (Frame.Width - 130, Frame.Height - 40, 100f, 44f);
			btnLoad.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin;
			btnLoad.SetTitle ("Load Last", UIControlState.Normal);
			btnLoad.TouchUpInside += (sender, e) => {
				if (points != null)
					Signature.LoadPoints (points);
			};		

			//Using different layouts for the iPhone and iPad, so setup device specific requirements here.
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {

				//iPhone version simply saves the vector of points in an instance variable.
				btnSave.TouchUpInside += (sender, e) => {
					if (Signature.IsBlank)
						new UIAlertView ("", "No signature to save.", null, "Okay", null).Show ();
					else {
						points = Signature.Points;
						new UIAlertView ("", "Vector Saved.", null, "Okay", null).Show ();
					}
				};
			} else {

				//iPad version saves the vector of points as well as retrieving the UIImage to display
				//in a UIImageView.
				btnSave.TouchUpInside += (sender, e) => {
					//if (signature.IsBlank)
					//	new UIAlertView ("", "No signature to save.", null, "Okay", null).Show ();
					imageView.Image = Signature.GetImage ();
					points = Signature.Points;
				};

				//Create the UIImageView to display a saved signature.
				imageView = new UIImageView();
				imageView.Frame = Signature.Frame;

				AddSubview(imageView);
			}
			TranslatesAutoresizingMaskIntoConstraints = false;

			//Add the subviews.
			AddSubview (Signature);
			AddSubview (btnSave);
			AddSubview (btnLoad);
		}
	}
}


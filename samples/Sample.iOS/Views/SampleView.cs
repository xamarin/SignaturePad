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
		SignaturePadView signature { get; set; }
		UIImageView imageView;
		UIButton btnSave, btnLoad;
		PointF [] points;

		public SampleView ()
		{
			BackgroundColor = UIColor.White;

			//Create the save button
			btnSave = UIButton.FromType (UIButtonType.RoundedRect);
			btnSave.SetTitle ("Save", UIControlState.Normal);

			//Create the load button
			btnLoad = UIButton.FromType (UIButtonType.RoundedRect);
			btnLoad.SetTitle ("Load Last", UIControlState.Normal);
			btnLoad.TouchUpInside += (sender, e) => {
				if (points != null)
					signature.LoadPoints (points);
			};

			signature = new SignaturePadView ();

			//Using different layouts for the iPhone and iPad, so setup device specific requirements here.
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				//iPhone uses Landscape, but ApplicationFrame still returns Portrait values, so 
				//reverse them for creating the view's frame.
				Frame = new RectangleF (0, 20, UIScreen.MainScreen.ApplicationFrame.Height, 
				                        UIScreen.MainScreen.ApplicationFrame.Width);

				//iPhone version simply saves the vector of points in an instance variable.
				btnSave.TouchUpInside += (sender, e) => {
					if (signature.IsBlank)
						new UIAlertView ("", "No signature to save.", null, "Okay", null).Show ();
					else {
						points = signature.Points;
						new UIAlertView ("", "Vector Saved.", null, "Okay", null).Show ();
					}
				};
			} else {
				Frame = UIScreen.MainScreen.ApplicationFrame;

				//iPad version saves the vector of points as well as retrieving the UIImage to display
				//in a UIImageView.
				btnSave.TouchUpInside += (sender, e) => {
					//if (signature.IsBlank)
					//	new UIAlertView ("", "No signature to save.", null, "Okay", null).Show ();
					imageView.Image = signature.GetImage ();
					points = signature.Points;
				};

				//Create the UIImageView to display a saved signature.
				imageView = new UIImageView();
				AddSubview(imageView);
			}

			//Add the subviews.
			AddSubview (signature);
			AddSubview (btnSave);
			AddSubview (btnLoad);
		}

		public override void LayoutSubviews ()
		{
			///Using different layouts for the iPhone and iPad, so setup device specific requirements here.
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				signature.Frame = new RectangleF (10, 10, Bounds.Width - 20, Bounds.Height - 60);
			else {
				signature.Frame = new RectangleF (84, 84, Bounds.Width - 168, Bounds.Width / 2);
				imageView.Frame = new RectangleF (84, signature.Frame.Height + 168,
				                                   Frame.Width - 168, Frame.Width / 2);
			}

			//Button locations are based on the Frame, so must have their own frames set after the view's
			//Frame has been set.
			btnSave.Frame = new RectangleF (10, Bounds.Height - 40, 120, 37);
			btnLoad.Frame = new RectangleF (Bounds.Width - 130, Bounds.Height - 40, 120, 37);
		}
	}
}


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
			view = new SampleView();
			View = view;
		}
	}
}


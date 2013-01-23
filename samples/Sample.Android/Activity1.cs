using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

using SignaturePad;

namespace Sample.Android {
	[Activity (Label = "Sample.Android", MainLauncher = true)]
	public class Activity1 : Activity {
		System.Drawing.PointF [] points;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			SignaturePadView signature = FindViewById<SignaturePadView> (Resource.Id.signatureView);

			// Get our button from the layout resource,
			// and attach an event to it
			Button btnSave = FindViewById<Button> (Resource.Id.btnSave);
			btnSave.Click += delegate {
				if (signature.IsBlank)
				{//Display the base line for the user to sign on.
					AlertDialog.Builder alert = new AlertDialog.Builder (this);
					alert.SetMessage ("No signature to save.");
					alert.SetNeutralButton ("Okay", delegate { });
					alert.Create ().Show ();
				}
				points = signature.Points;
			};
			btnSave.Dispose ();

			Button btnLoad = FindViewById<Button> (Resource.Id.btnLoad);
			btnLoad.Click += delegate {
				if (points != null)
					signature.LoadPoints (points);
			};
			btnLoad.Dispose ();
		}
	}
}



using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Photos;
using UIKit;

using Xamarin.Controls;

namespace Sample.iOS
{
	public partial class ViewController : UIViewController
	{
		private CGPoint[] points;

		public ViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			signatureView.Layer.BorderColor = UIColor.FromRGBA (184, 134, 11, 255).CGColor;
			signatureView.Layer.BorderWidth = 1f;

			signatureView.StrokeCompleted += (sender, e) => UpdateControls ();
			signatureView.Cleared += (sender, e) => UpdateControls ();

			UpdateControls ();
		}

		private void UpdateControls ()
		{
			btnSave.Enabled = !signatureView.IsBlank;
			btnSaveImage.Enabled = !signatureView.IsBlank;
			btnLoad.Enabled = points != null;
		}

		partial void SaveVectorClicked (UIButton sender)
		{
			points = signatureView.Points;
			UpdateControls ();

			ShowToast ("Vector signature saved to memory.");
		}

		partial void LoadVectorClicked (UIButton sender)
		{
			signatureView.LoadPoints (points);
		}

		async partial void SaveImageClicked (UIButton sender)
		{
			UIImage image;
			using (var bitmap = await signatureView.GetImageStreamAsync (SignatureImageFormat.Png, UIColor.Black, UIColor.White, 1f))
			using (var data = NSData.FromStream (bitmap))
			{
				image = UIImage.LoadFromData (data);
			}

			var status = await PHPhotoLibrary.RequestAuthorizationAsync ();
			if (status == PHAuthorizationStatus.Authorized)
			{
				image.SaveToPhotosAlbum ((i, error) =>
				{
					image.Dispose ();

					if (error == null)
						ShowToast ("Raster signature saved to the photo library.");
					else
						ShowToast ("There was an error saving the signature: " + error.LocalizedDescription);
				});
			}
			else
			{
				ShowToast ("Permission to save to the photo library was denied.");
			}
		}

		private async void ShowToast (string message)
		{
			var toast = UIAlertController.Create (null, message, UIAlertControllerStyle.Alert);
			await PresentViewControllerAsync (toast, true);
			await Task.Delay (1000);
			await toast.DismissViewControllerAsync (true);
		}
	}
}

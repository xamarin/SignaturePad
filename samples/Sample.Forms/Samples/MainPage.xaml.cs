using System;
using System.Linq;
using Xamarin.Forms;

namespace Samples
{
	public partial class MainPage : ContentPage
	{
		private Point[] points;

		public MainPage ()
		{
			InitializeComponent ();

			UpdateControls ();
		}

		private void UpdateControls ()
		{
			btnSave.IsEnabled = !signatureView.IsBlank;
			btnSaveImage.IsEnabled = !signatureView.IsBlank;
			btnLoad.IsEnabled = points != null;
		}

		private void SaveVectorClicked (object sender, EventArgs e)
		{
			points = signatureView.Points.ToArray ();
			UpdateControls ();

			DisplayAlert ("Signature Pad", "Vector signature saved to memory.", "OK");
		}

		private void LoadVectorClicked (object sender, EventArgs e)
		{
			signatureView.Points = points;
		}

		private async void SaveImageClicked (object sender, EventArgs e)
		{
			//var storageFolder = await KnownFolders.GetFolderForUserAsync (null, KnownFolderId.PicturesLibrary);
			//var file = await storageFolder.CreateFileAsync ("signature.png", CreationCollisionOption.ReplaceExisting);

			//using (var bitmap = await signatureView.GetImageStreamAsync (SignatureImageFormat.Png, Colors.Black, Colors.White, 1f))
			//using (var stream = await file.OpenAsync (FileAccessMode.ReadWrite))
			//using (var dest = stream.AsStreamForWrite ())
			//{
			//	await bitmap.CopyToAsync (dest);
			//}

			await DisplayAlert ("Signature Pad", "Raster signature saved to the photo library.", "OK");
		}

		private void SignatureChanged (object sender, EventArgs e)
		{
			UpdateControls ();
		}
	}
}

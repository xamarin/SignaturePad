using System;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Xamarin.Controls;

namespace Sample.UWP
{
	public sealed partial class MainPage : Page
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

		private void SaveVectorClicked (object sender, RoutedEventArgs e)
		{
			points = signatureView.Points;
			UpdateControls ();

			var flyout = new Flyout
			{
				Content = new TextBlock
				{
					Text = "Vector signature saved to memory."
				}
			};
			flyout.ShowAt (btnSave);
		}

		private void LoadVectorClicked (object sender, RoutedEventArgs e)
		{
			signatureView.LoadPoints (points);
		}

		private async void SaveImageClicked (object sender, RoutedEventArgs e)
		{
			var storageFolder = await KnownFolders.GetFolderForUserAsync (null, KnownFolderId.PicturesLibrary);
			var file = await storageFolder.CreateFileAsync ("signature.png", CreationCollisionOption.ReplaceExisting);

			using (var bitmap = await signatureView.GetImageStreamAsync (SignatureImageFormat.Png, Colors.Black, Colors.White, 1f))
			using (var stream = await file.OpenAsync (FileAccessMode.ReadWrite))
			using (var dest = stream.AsStreamForWrite ())
			{
				await bitmap.CopyToAsync (dest);
			}

			var flyout = new Flyout
			{
				Content = new TextBlock
				{
					Text = "Raster signature saved to the photo library."
				}
			};
			flyout.ShowAt (btnSaveImage);
		}

		private void SignatureChanged (object sender, EventArgs e)
		{
			UpdateControls ();
		}
	}
}

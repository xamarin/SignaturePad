using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Xamarin.Controls;

namespace Sample.WindowsPhone81
{
	public sealed partial class MainPage : Page
	{
		private Point[] points;

		public MainPage ()
		{
			InitializeComponent ();

			NavigationCacheMode = NavigationCacheMode.Required;
		}

		private void btnSave_Click (object sender, RoutedEventArgs e)
		{
			points = signatureView.Points;

			new MessageDialog ("Vector saved!").ShowAsync ();
		}

		private void btnLoad_Click (object sender, RoutedEventArgs e)
		{
			signatureView.LoadPoints (points);
		}

		private async void btnSaveImage_Click (object sender, RoutedEventArgs e)
		{
			var storageFolder = KnownFolders.PicturesLibrary;
			var file = await storageFolder.CreateFileAsync ("signature.png", CreationCollisionOption.ReplaceExisting);

			using (var bitmap = await signatureView.GetImageStreamAsync (SignatureImageFormat.Png, Colors.Black, Colors.White, 1f))
			using (var stream = await file.OpenAsync (FileAccessMode.ReadWrite))
			using (var dest = stream.AsStreamForWrite ())
			{
				await bitmap.CopyToAsync (dest);
			}

			await new MessageDialog ("Picture saved to photo library!").ShowAsync ();
		}
	}
}

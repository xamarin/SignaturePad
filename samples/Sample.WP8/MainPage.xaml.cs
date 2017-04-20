using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;

namespace Sample.WP8
{
	public partial class MainPage : PhoneApplicationPage
	{
		private Point[] points;

		public MainPage ()
		{
			InitializeComponent ();
		}

		private void btnSave_Click (object sender, RoutedEventArgs e)
		{
			points = signatureView.Points;

			MessageBox.Show ("Vector saved!");
		}

		private void btnLoad_Click (object sender, RoutedEventArgs e)
		{
			signatureView.LoadPoints (points);
		}

		private void btnSaveImage_Click (object sender, RoutedEventArgs e)
		{
			var bitmap = signatureView.GetImage (Colors.Black, Colors.White, 1f);

			using (var stream = new MemoryStream ())
			{
				bitmap.SaveJpeg (stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
				stream.Seek (0, SeekOrigin.Begin);

				using (MediaLibrary mediaLibrary = new MediaLibrary ())
				{
					mediaLibrary.SavePicture ("signature.jpg", stream);
				}
			}

			MessageBox.Show ("Picture saved to photo library");
		}
	}
}

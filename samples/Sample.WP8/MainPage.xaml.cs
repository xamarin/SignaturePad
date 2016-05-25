using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace Sample.WP8
{
	public partial class MainPage : PhoneApplicationPage
	{
		Point [] points;

		// Constructor
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
			WriteableBitmap bitmap = signatureView.GetImage ();

			using (MemoryStream stream = new MemoryStream ()) {
				bitmap.SaveJpeg (stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
				stream.Seek (0, SeekOrigin.Begin);

				using (MediaLibrary mediaLibrary = new MediaLibrary ())
					mediaLibrary.SavePicture ("signature.jpg", stream);
			}
			MessageBox.Show ("Picture saved to photo library");
		}
	}
}
using System;
using System.Linq;
using Xamarin.Forms;

using SignaturePad.Forms;

namespace Samples.Views
{
	public partial class SignatureXamlView : ContentPage
	{
		public SignatureXamlView ()
		{
			InitializeComponent ();
		}

		private async void OnChangeTheme (object sender, EventArgs e)
		{
			var action = await DisplayActionSheet ("Change Theme", "Cancel", null, "White", "Black", "Aqua");
			switch (action)
			{
				case "White":
					padView.BackgroundColor = Color.White;
					padView.StrokeColor = Color.Black;
					padView.ClearTextColor = Color.Black;
					padView.ClearText = "Clear Markers";
					break;

				case "Black":
					padView.BackgroundColor = Color.Black;
					padView.StrokeColor = Color.White;
					padView.ClearTextColor = Color.White;
					padView.ClearText = "Clear Chalk";
					break;

				case "Aqua":
					padView.BackgroundColor = Color.Aqua;
					padView.StrokeColor = Color.Red;
					padView.ClearTextColor = Color.Black;
					padView.ClearText = "Clear The Aqua";
					break;
			}
		}

		private async void OnGetStats (object sender, EventArgs e)
		{
			using (var image = await padView.GetImageStreamAsync (SignatureImageFormat.Png))
			{
				var imageSize = (image?.Length ?? 0) / 1000;

				var points = padView.Points.ToArray ();
				var pointCount = points.Count ();
				var linesCount = points.Count (p => p == Point.Zero) + (points.Length > 0 ? 1 : 0);

				image?.Dispose ();

				await DisplayAlert ("Stats", $"The signature has {linesCount} lines or {pointCount} points, and is {imageSize:#,###.0}KB (in memory) when saved as a PNG.", "Cool");
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using SignaturePad.Forms;

namespace Samples
{
	public class BindingViewModel : BindableObject
	{
		private IEnumerable<Point> currentSignature;
		private Point[] savedSignature;

		public BindingViewModel (Func<SignatureImageFormat, ImageConstructionSettings, Task<Stream>> getImageDelegate)
		{
			GetImageStreamAsync = getImageDelegate;

			SaveVectorCommand = new Command (OnSaveVector);
			LoadVectorCommand = new Command (OnLoadVector);
			SaveImageCommand = new Command (OnSaveImage);
		}

		public IEnumerable<Point> CurrentSignature
		{
			get => currentSignature;
			set
			{
				currentSignature = value;
				OnPropertyChanged ();
			}
		}

		public Point[] SavedSignature
		{
			get => savedSignature;
			set
			{
				savedSignature = value;
				OnPropertyChanged ();
				OnPropertyChanged (nameof (HasSavedSignature));
			}
		}

		public bool HasSavedSignature => SavedSignature?.Length > 0;

		public ICommand SaveVectorCommand { get; }

		public ICommand LoadVectorCommand { get; }

		public ICommand SaveImageCommand { get; }

		private Func<SignatureImageFormat, ImageConstructionSettings, Task<Stream>> GetImageStreamAsync { get; }

		private void OnSaveVector ()
		{
			SavedSignature = CurrentSignature.ToArray ();

			DisplayAlert ("Vector signature saved to memory.");
		}

		private void OnLoadVector ()
		{
			CurrentSignature = SavedSignature;
		}

		private async void OnSaveImage ()
		{
			var settings = new ImageConstructionSettings
			{
				StrokeColor = Color.Black,
				BackgroundColor = Color.White,
				StrokeWidth = 1f
			};

			using (var bitmap = await GetImageStreamAsync (SignatureImageFormat.Png, settings))
			{
				var saved = await App.SaveSignature (bitmap, "signature.png");

				if (saved)
					DisplayAlert ("Raster signature saved to the photo library.");
				else
					DisplayAlert ("There was an error saving the signature.");
			}
		}

		private void DisplayAlert (string message)
		{
			Application.Current.MainPage.DisplayAlert ("Signature Pad", message, "OK");
		}
	}
}

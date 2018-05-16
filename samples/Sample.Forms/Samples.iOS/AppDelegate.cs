using System.IO;
using System.Threading.Tasks;
using Foundation;
using Photos;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Samples.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Forms.Init ();

			LoadApplication (new App (OnSaveSignature));

			return base.FinishedLaunching (app, options);
		}

		private async Task<bool> OnSaveSignature (Stream bitmap, string filename)
		{
			var tcs = new TaskCompletionSource<bool> ();

			UIImage image;
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

					tcs.TrySetResult (error == null);
				});
			}
			else
			{
				tcs.TrySetResult (false);
			}

			return await tcs.Task;
		}
	}
}

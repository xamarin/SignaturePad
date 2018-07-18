// <copyright file="AppDelegate.cs" company="Vladislav Antonyuk">
//     Vladislav Antonyuk. All rights reserved.
// </copyright>
// <author>Vladislav Antonyuk</author>

namespace Samples.MacOS
{
	using System.IO;
	using System.Threading.Tasks;
	using AppKit;
    using Foundation;
	using Photos;
	using Xamarin.Forms;
    using Xamarin.Forms.Platform.MacOS;

    [Register(nameof(AppDelegate))]
    public class AppDelegate : FormsApplicationDelegate
    {
        public AppDelegate()
        {
            MainWindow = UiService.MainWindow;
        }

        public override NSWindow MainWindow { get; }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(new App (OnSaveSignature));
            base.DidFinishLaunching(notification);
        }
		private async Task<bool> OnSaveSignature (Stream bitmap, string filename)
		{
			var tcs = new TaskCompletionSource<bool> ();

			NSImage image = NSImage.FromStream(bitmap);

			var status = await PHPhotoLibrary.RequestAuthorizationAsync ();
			if (status == PHAuthorizationStatus.Authorized)
			{
				//image.SaveToPhotosAlbum ((i, error) =>
				//{
				//	image.Dispose ();

				//	tcs.TrySetResult (error == null);
				//});
			}
			else
			{
				tcs.TrySetResult (false);
			}

			return await tcs.Task;
		}
	}
}

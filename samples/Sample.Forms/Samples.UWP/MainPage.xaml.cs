using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Samples.UWP
{
	public sealed partial class MainPage
	{
		public MainPage ()
		{
			InitializeComponent ();

			LoadApplication (new Samples.App (OnSaveSignature));
		}

		private async Task<bool> OnSaveSignature (Stream bitmap, string filename)
		{
			var storageFolder = await KnownFolders.GetFolderForUserAsync (null, KnownFolderId.PicturesLibrary);
			var file = await storageFolder.CreateFileAsync (filename, CreationCollisionOption.ReplaceExisting);

			using (var stream = await file.OpenAsync (FileAccessMode.ReadWrite))
			using (var dest = stream.AsStreamForWrite ())
			{
				await bitmap.CopyToAsync (dest);
			}

			return true;
		}
	}
}

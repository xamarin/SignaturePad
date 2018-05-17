using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]

namespace Samples
{
	public partial class App : Application
	{
		private readonly Func<Stream, string, Task<bool>> saveSignatureDelegate;

		public App (Func<Stream, string, Task<bool>> saveSignature)
		{
			InitializeComponent ();

			saveSignatureDelegate = saveSignature;

			MainPage = new NavigationPage (new MainPage ());
		}

		public static Task<bool> SaveSignature (Stream bitmap, string filename)
		{
			return ((App)Application.Current).saveSignatureDelegate (bitmap, filename);
		}
	}
}

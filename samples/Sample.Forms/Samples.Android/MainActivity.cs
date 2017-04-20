using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Samples.Droid
{
	[Activity (Label = "@string/app_name", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			// set the layout resources first
			ToolbarResource = Resource.Layout.toolbar;
			TabLayoutResource = Resource.Layout.tabs;

			// then call base.OnCreate and the Xamarin.Forms methods
			base.OnCreate (bundle);
			Forms.Init (this, bundle);
			LoadApplication (new App ());
		}
	}
}

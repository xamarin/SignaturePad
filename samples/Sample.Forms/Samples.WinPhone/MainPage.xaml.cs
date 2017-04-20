using Xamarin.Forms;

namespace Samples.WinPhone
{
	public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
	{
		public MainPage ()
		{
			InitializeComponent ();

			Forms.Init ();
			this.LoadApplication (new Samples.App ());
		}
	}
}

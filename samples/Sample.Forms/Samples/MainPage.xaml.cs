using Xamarin.Forms;

namespace Samples
{
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();

			BindingContext = new MainViewModel (signaturePadView.GetImageStreamAsync);
		}
	}
}

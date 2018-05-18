using Xamarin.Forms;

namespace Samples
{
	public partial class BindingPage : ContentPage
	{
		public BindingPage ()
		{
			InitializeComponent ();

			BindingContext = new BindingViewModel (signaturePadView.GetImageStreamAsync);
		}
	}
}

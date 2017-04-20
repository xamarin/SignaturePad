using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public ICommand DataBindingCommand { get; } = new Command (() => App.NavigateTo<SignaturePadConfigViewModel> ());

		public ICommand XamlCodeBehindCommand { get; } = new Command (() => App.NavigateTo<SignatureXamlViewModel> ());
	}
}

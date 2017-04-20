using System;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

using Samples.ViewModels;
using Samples.Views;

namespace Samples
{
	public class App : Application
	{
		public App ()
		{
			var navigationPage = new NavigationPage (new MainView ());

			navigationPage.Popped += (sender, args) =>
			{
				(args.Page.BindingContext as BaseViewModel)?.OnDisappearing ();
			};
			navigationPage.Pushed += (sender, args) =>
			{
				(args.Page.BindingContext as BaseViewModel)?.OnAppearing ();
			};

			MainPage = navigationPage;
		}

		public static new App Current => (App)Application.Current;

		public static Task<string> DisplayActions (string cancel, string destruction, string buttons)
		{
			var currentPage = ((NavigationPage)Current.MainPage).CurrentPage;
			return currentPage.DisplayActionSheet ("Signature Pad", cancel, destruction, buttons);
		}

		public static Task<bool> DisplayAlert (string message, string yes = "OK", string no = null)
		{
			var currentPage = ((NavigationPage)Current.MainPage).CurrentPage;
			if (no == null)
			{
				return currentPage.DisplayAlert ("Signature Pad", message, yes).ContinueWith (task => true);
			}
			else
			{
				return currentPage.DisplayAlert ("Signature Pad", message, yes, no);
			}
		}

		public static Task NavigateTo<T> ()
			where T : BaseViewModel
		{
			// do some very simple navigation/lookups
			// basically, just remove the "Model" part of the VM and that is the page
			// "converntion-based" :)
			var viewModelName = typeof (T).Name;
			var pageType = typeof (MainView);
			var pageNamespace = pageType.Namespace;
			var pageAssembly = pageType.GetTypeInfo ().Assembly;
			var newPageName = viewModelName.Substring (0, viewModelName.Length - "Model".Length);
			var newPageType = pageAssembly.GetType ($"{pageNamespace}.{newPageName}");

			var newPage = Activator.CreateInstance (newPageType) as Page;
			var currentPage = ((NavigationPage)Current.MainPage).CurrentPage;
			return currentPage.Navigation.PushAsync (newPage);
		}
	}
}

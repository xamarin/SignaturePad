using System;
using SignaturePad.Forms;
using Xamarin.Forms;

namespace Samples.Xam.Forms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as MainViewModel;
            if (vm != null)
            {
                var stream = await SignaturePad.GetImageStreamAsync(SignatureImageFormat.Png);
                vm.SaveSignature(stream);
            }
        }
    }
}

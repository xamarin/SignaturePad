using System;
using System.IO;
using System.Linq;
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

        private async void SaveSignatureClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as MainViewModel;
            if (vm != null)
            {
                var stream = await SignaturePad.GetImageStreamAsync(SignatureImageFormat.Png);
                vm.SaveSignature(stream);
            }
        }

        private void SavePointsClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as MainViewModel;
            if (vm != null)
            {
                vm.StoredStoredPoints = SignaturePad.Points.ToArray();
            }
        }

        private void LoadPointsClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as MainViewModel;
            if (vm != null)
            {
                SignaturePad.Points = vm.StoredStoredPoints;
            }
        }
    }
}

using Xamarin.Forms;

namespace Samples.WinPhone
{

    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            var t = typeof(SignaturePad.Forms.WindowsPhone.SignaturePadRenderer);

            Forms.Init();
            this.LoadApplication(new Samples.App());
        }
    }
}

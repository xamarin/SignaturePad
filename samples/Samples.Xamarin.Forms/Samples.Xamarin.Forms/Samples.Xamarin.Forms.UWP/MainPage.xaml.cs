using Xamarin.Forms;

namespace Samples.Xam.Forms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            DependencyService.Register<IFileSystem, FileSystem>();

            LoadApplication(new Xam.Forms.App());
        }
    }
}
